using Microsoft.AspNetCore.Mvc;
using SpecTech.Domain.Services;

namespace SpecTech.WebAPI.Controllers;

[Route("[controller]/[action]")]
[ApiController]

public class MessagesController : Controller
{
    private readonly TelegramService _service;

    public MessagesController(TelegramService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRecords()
    {
        var messages = await _service.GetAllTelegramMessagesAsync();
        return Ok(messages);
    }
}