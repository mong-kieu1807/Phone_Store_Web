using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PhoneStore.Models;
using PhoneStore.Data;
using Microsoft.EntityFrameworkCore;

namespace PhoneStore.Controllers;
public class AccountUserController : Controller
{
    private readonly ILogger<AccountUserController> _logger;
    private readonly ApplicationDbContext _context;

    public AccountUserController(ILogger<AccountUserController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {

        
        // Ẩn Search Bar và Navigation
        ViewData["HideSearchAndNav"] = true;
        return View();
    }
}