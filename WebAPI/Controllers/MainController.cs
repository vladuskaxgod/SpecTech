using BLL;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class MainController : Controller
{
    private ApplicationContext _context;

    public MainController(ApplicationContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRecords()
    {
        return Ok(_context.Messages.ToList());
    }
}