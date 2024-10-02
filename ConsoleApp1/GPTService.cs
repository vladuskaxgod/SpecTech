using System;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace ConsoleApp1;

public class GPTService
{
    private readonly IConfiguration _configuration;
    private readonly string _systemPrompt;
    private readonly string _textRequest;
    private readonly Dictionary<DateTime, string> _bearerTokens = new Dictionary<DateTime, string>();
    
    public GPTService()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        
        _systemPrompt = File.ReadAllText($"{Directory.GetCurrentDirectory()}\\systemPrompt.txt", Encoding.UTF8);
        _textRequest = File.ReadAllText( $"{Directory.GetCurrentDirectory()}\\InitialRequest.json")
            .Replace("{modelUri}", _configuration["GPTSettings:ModelUri"])
            .Replace("{temperature}", _configuration["GPTSettings:Temperature"])
            .Replace("{maxTokens}", _configuration["GPTSettings:MaxTokens"])
            .Replace("{systemPrompt}", _systemPrompt);
    }

    private async Task<string> _getYandexIAMToken()
    {
        using (var client = new HttpClient())
        {
            var content = new StringContent("{\"yandexPassportOauthToken\": \"{oauthToken}\"}".Replace("{oauthToken}", _configuration["GPTSettings:OAuthToken"]), Encoding.UTF8, "application/json");
            
            var response = await client.PostAsync(_configuration["GPTSettings:RetrieveIAMTokenUri"], content);

            if (response.IsSuccessStatusCode)
            {
                // Чтение ответа
                var responseBody = await response.Content.ReadAsStringAsync();
                return JObject.Parse(responseBody)["iamToken"].ToString();
            }
            else
            {
                throw new Exception("Failed to retrieve IAM token!");
            }
        }
    }

    private async Task<string> _getBearerToken()
    {
        var token = _bearerTokens.FirstOrDefault(x => x.Key > DateTime.Now.AddHours(-2)).Value;

        if (token != null) return token;

        token = await _getYandexIAMToken();
        
        _bearerTokens.Add(DateTime.Now, token);

        return token;
    }

    public async Task<string> AskWithSystemPrompt(string prompt)
    {
        var request = JObject.Parse(_textRequest.Replace("{userPrompt}", prompt));
        
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _getBearerToken());
            var content = new StringContent(request.ToString(), Encoding.UTF8, "application/json");

            var res = "";

            do
            {
                var response = await client.PostAsync(_configuration["GPTSettings:RequestUri"], content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    res =  JObject.Parse(responseBody)["result"]["alternatives"][0]["message"]["text"].ToString();

                    /*var splittedMessage = res.Split("\n").ToList();

                    var entity = new MessageEntity()
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
                    });*/
                }
                else
                {
                    Thread.Sleep(3000);
                }
            } while (string.IsNullOrEmpty(res));

            return res;
        }
    }
}