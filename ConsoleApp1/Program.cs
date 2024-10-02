using System.Net.Http.Headers;
using System.Text;
using BLL;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using TL;
using Microsoft.Extensions.Configuration;

namespace ConsoleApp1;

class Program
{
    static WTelegram.UpdateManager Manager;
    static User My;
    private static WTelegram.Client client;
    private static ApplicationContext _context;
    
    public static async Task Main(string[] args)
    {
        var gptService = new GPTService();

        var res = await gptService.AskWithSystemPrompt("Тип техники: полноповоротный колёсный экскаватор\n#Бренд: Cat\n#Модель: 315\n#Объём двигателя: неизвестно\n#Объём ковша: неизвестно\n#Может поднять: неизвестно\n#Дополнительная информация: опытный машинист. Планировочный ковш.\n#Контактное лицо: Артём, 8921 933-15-08");
        
        _context = new ApplicationContext();
        
        client = new WTelegram.Client(15944678, "4e2472261c8a4a89e3e11d8e92f9f93d");
        await DoLogin("+79045181622");

        async Task DoLogin(string loginInfo) // (add this method to your code)
        {
            while (client.User == null)
            switch (await client.Login(loginInfo)) // returns which config is needed to continue login
            {
                case "verification_code": Console.Write("Code: "); loginInfo = Console.ReadLine(); break;
                case "name": loginInfo = "John Doe"; break;    // if sign-up is required (first/last_name)
                case "password": loginInfo = "secret!"; break; // if user has enabled 2FA
                default: loginInfo = null; break;
            }
            Console.WriteLine($"We are logged-in as {client.User} (id {client.User.id})");
        }

        var messages = await _getAllMessages();
        foreach (var message in messages)
        {
            await askGPT(message);
        }

        WTelegram.Helpers.Log = (l, s) => System.Diagnostics.Debug.WriteLine(s);
        using (client)
        {
            Manager = client.WithUpdateManager(Client_OnUpdate/*, "Updates.state"*/);
            My = client.User;
            // Note: on login, Telegram may sends a bunch of updates/messages that happened in the past and were not acknowledged
            Console.WriteLine($"We are logged-in as {My.username ?? My.first_name + " " + My.last_name} (id {My.id})");
            // We collect all infos about the users/chats so that updates can be printed with their names
            var dialogs = await client.Messages_GetAllDialogs(); // dialogs = groups/channels/users
            dialogs.CollectUsersChats(Manager.Users, Manager.Chats);
				
            Console.ReadKey();
        }
    }

    private static async Task<List<string>> _getAllMessages()
    {
        var res = new List<string>();
        var chats = await client.Messages_GetAllChats();
        
        /*foreach (var peer in chats.chats.Select(x => x.Value))
        {*/
            var messages = await client.Messages_GetHistory(chats.chats[1370994047]);
            if (messages.Messages.Length == 0) return new List<string>();
            foreach (var msgBase in messages.Messages)
            {
                var from = messages.UserOrChat(msgBase.From ?? msgBase.Peer); // from can be User/Chat/Channel
                if (msgBase is Message msg)
                {
                    Console.WriteLine($"{from}> {msg.message} {msg.media}");
                    res.Add(msg.message);
                }
                else if (msgBase is MessageService ms)
                    Console.WriteLine($"{from} [{ms.action.GetType().Name[13..]}]");
            }
        //}
        return res;
    }

    private static async Task Client_OnUpdate(Update update)
    {
        switch (update)
        {
            case UpdateNewMessage unm: await HandleMessage(unm.message); break;
            case UpdateEditMessage uem: await HandleMessage(uem.message, true); break;
            // Note: UpdateNewChannelMessage and UpdateEditChannelMessage are also handled by above cases
            case UpdateDeleteChannelMessages udcm: Console.WriteLine($"{udcm.messages.Length} message(s) deleted in {Chat(udcm.channel_id)}"); break;
            case UpdateDeleteMessages udm: Console.WriteLine($"{udm.messages.Length} message(s) deleted"); break;
            case UpdateUserTyping uut: Console.WriteLine($"{User(uut.user_id)} is {uut.action}"); break;
            case UpdateChatUserTyping ucut: Console.WriteLine($"{Peer(ucut.from_id)} is {ucut.action} in {Chat(ucut.chat_id)}"); break;
            case UpdateChannelUserTyping ucut2: Console.WriteLine($"{Peer(ucut2.from_id)} is {ucut2.action} in {Chat(ucut2.channel_id)}"); break;
            case UpdateChatParticipants { participants: ChatParticipants cp }: Console.WriteLine($"{cp.participants.Length} participants in {Chat(cp.chat_id)}"); break;
            case UpdateUserStatus uus: Console.WriteLine($"{User(uus.user_id)} is now {uus.status.GetType().Name[10..]}"); break;
            case UpdateUserName uun: Console.WriteLine($"{User(uun.user_id)} has changed profile name: {uun.first_name} {uun.last_name}"); break;
            case UpdateUser uu: Console.WriteLine($"{User(uu.user_id)} has changed infos/photo"); break;
            default: Console.WriteLine(update.GetType().Name); break; // there are much more update types than the above example cases
        }
    }
    
    private static async Task HandleMessage(MessageBase messageBase, bool edit = false)
    {
        if (edit) Console.Write("(Edit): ");
        switch (messageBase)
        {
            case Message m:
            {
                var messageToSend = await askGPT(m.message);
                await client.SendMessageAsync(Manager.Chats[m.peer_id], messageToSend);
                Console.WriteLine($"{Peer(m.from_id) ?? m.post_author} in {Peer(m.peer_id)}> {m.message}"); 
                break;
            }
            case MessageService ms: Console.WriteLine($"{Peer(ms.from_id)} in {Peer(ms.peer_id)} [{ms.action.GetType().Name[13..]}]"); break;
        }
    }
    
    private static string User(long id) => Manager.Users.TryGetValue(id, out var user) ? user.ToString() : $"User {id}";
    private static string Chat(long id) => Manager.Chats.TryGetValue(id, out var chat) ? chat.ToString() : $"Chat {id}";
    private static string Peer(Peer peer) => Manager.UserOrChat(peer)?.ToString() ?? $"Peer {peer?.ID}";
    
    static async Task<string> askGPT(string message)
    {
        string url = "https://llm.api.cloud.yandex.net/foundationModels/v1/completion";
        var lastMsg = message;
        var systemPrompt = "я буду давать тебе сообщение, в котором будет содержаться информация о свободной для использования промышленной технике. это будет техника в духе экскаватор, погрузчик, трактор и тому подобные. " +
                           "ты должен мне в ответ предоставить сообщение в формате (весь текст внутри звездочек нужно заменить на реальные значения. все то, что не указано в сообщении - попытаться найти в интернете. если в интернете нет информации - написать 'неизвестно'. каждыйы пункт обязателен, для тех что нет информации - написать неизвестно. больше ничего не пиши, только пункты, что я перечислил):" +
                           "Тип техники: *экскаватор*" +
                           "Бренд: *Zauberg*" +
                           "Модель: *EX-210CX*" +
                           "Объем двигателя: *5.8л*" +
                           "Объем ковша: *0.9 квм*" +
                           "Может поднять: *2 тонны*" +
                           "Дополнительная информация: *доп. информация*" +
                           "Контактное лицо: *Богдан +79991233434 (если в начале стоит 8 - заменить на +7)*" +
                           "Будет свободен: *06.09.2024 (пятница)*";

        JObject request = null;

        if (request == null)
        {
            try
            {
                request = JObject.Parse("{\n  \"modelUri\": \"gpt://b1gi0pucm0rdvi8llqd5/yandexgpt/latest\",\n  \"completionOptions\": {\n    \"stream\": false,\n    \"temperature\": \"0.1\",\n    \"maxTokens\": \"1000\"\n  },\n  \"messages\": [\n    {\n      \"role\": \"system\",\n      \"text\": \"" + systemPrompt + "\"\n    }, {\n      \"role\": \"user\",\n      \"text\": \"" + lastMsg.Replace("\"", "'") + "\"\n    }\n  ]\n}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        else
        {
            var messages = (JArray)request["messages"];
            messages.Add(new JObject
            {
                { "role", "user" },
                { "text", lastMsg }
            });
        }

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "t1.9euelZrNjMzIjsyLxoqRiZSUz52Nm-3rnpWayZSWj8qMmsqYk5iNyM-Onsjl8_ccPRdJ-e8oR1le_d3z91xrFEn57yhHWV79zef1656VmonPkJSek5uRjZjNnZyUmpGc7_zF656VmonPkJSek5uRjZjNnZyUmpGc.1yvev02rNVSs8lpyCreoeDn_FNwWiuFs10xTuoCJOVjt_tlmdCr6989UN8ZxgNIftMUxokpEwifVncuL8KH2AA");
            var content = new StringContent(request.ToString(), Encoding.UTF8, "application/json");

            var error = false;

            do
            {
                // Отправка POST запроса
                var response = await client.PostAsync(url, content);

                // Проверка статуса ответа
                if (response.IsSuccessStatusCode)
                {
                    // Чтение ответа
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var res = JObject.Parse(responseBody)["result"]["alternatives"][0]["message"]["text"].ToString();

                    var splittedMessage = res.Split("\n").ToList();

                    var entity = new BLL.MessageEntity()
                    {
                        SourceMessage = lastMsg,
                        TechnicType = splittedMessage.TryGet(0),
                        Brand = splittedMessage.TryGet(1),
                        Model = splittedMessage.TryGet(2),
                        Engine = splittedMessage.TryGet(3),
                        Kovsh = splittedMessage.TryGet(4),
                        CanPull = splittedMessage.TryGet(5),
                        AdditionalInfo = splittedMessage.TryGet(6),
                        Contact = splittedMessage.TryGet(7) + " " + splittedMessage.TryGet(8),
                        Available = splittedMessage.TryGet(9)
                    };

                    _context.Messages.Add(entity);
                    await _context.SaveChangesAsync();

                    error = false;
                    
                    return res;

                    var messages = (JArray)request["messages"];
                    messages.Add(new JObject
                    {
                        { "role", "assistant" },
                        { "text", message }
                    });
                }
                else
                {
                    Console.WriteLine("Ошибка: " + response.StatusCode);
                    error = true;
                    Thread.Sleep(3000);
                    return "error";
                }
            } while (error);
        }
    }
}