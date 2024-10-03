using System.Net.Http.Headers;
using System.Text;
using Infrastructure.Data;
using Newtonsoft.Json.Linq;
using SpecTech.Domain.Extensions;
using SpecTech.Domain.Services;
using TL;

namespace SpecTech.TelegramParser;

class Program
{
    static WTelegram.UpdateManager Manager;
    static User My;
    private static WTelegram.Client client;
    private static ApplicationContext _context;
    private static Services.MessageService _messageService;
    
    public static async Task Main(string[] args)
    {
        var gptService = new GPTService();
        _context = new ApplicationContext();

        //var res = await gptService.AskWithSystemPrompt("Тип техники: полноповоротный колёсный экскаватор\n#Бренд: Cat\n#Модель: 315\n#Объём двигателя: неизвестно\n#Объём ковша: неизвестно\n#Может поднять: неизвестно\n#Дополнительная информация: опытный машинист. Планировочный ковш.\n#Контактное лицо: Артём, 8921 933-15-08");
        
        
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
                await _messageService.UpdateOrInsertTelegramMessage(m);
                Console.WriteLine($"{Peer(m.from_id) ?? m.post_author} in {Peer(m.peer_id)}> {m.message}"); 
                break;
            }
            case MessageService ms: Console.WriteLine($"{Peer(ms.from_id)} in {Peer(ms.peer_id)} [{ms.action.GetType().Name[13..]}]"); break;
        }
    }
    
    private static string User(long id) => Manager.Users.TryGetValue(id, out var user) ? user.ToString() : $"User {id}";
    private static string Chat(long id) => Manager.Chats.TryGetValue(id, out var chat) ? chat.ToString() : $"Chat {id}";
    private static string Peer(Peer peer) => Manager.UserOrChat(peer)?.ToString() ?? $"Peer {peer?.ID}";
}