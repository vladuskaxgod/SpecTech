using Microsoft.AspNetCore.Mvc;
using Application.Services;

namespace WebAPI.Controllers;

[Route("[controller]/[action]")]
[ApiController]

public class MessagesController : Controller
{
    private readonly MessagesService _service;

    public MessagesController(MessagesService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRecords()
    {
        var messages = await _service.GetAllRecordsAsync();
        return Ok(messages);
    }
}