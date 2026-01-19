using Microsoft.AspNetCore.Mvc;
using PhoneStore.Models;
using PhoneStore.Data;

namespace PhoneStore.Controllers;

public class ContactController : Controller
{
    private readonly ILogger<ContactController> _logger;
    private readonly ApplicationDbContext _context;

    public ContactController(ILogger<ContactController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    // GET: Contact
    public IActionResult Index()
    {
        return View();
    }

    // POST: Contact
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(Contact contact)
    {
        if (ModelState.IsValid)
        {
            contact.created_at = DateTime.Now;
            contact.status = 0; // Chưa xử lý
            
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation($"Contact form submitted by {contact.name} ({contact.email})");
            
            TempData["SuccessMessage"] = "Cảm ơn bạn đã liên hệ! Chúng tôi sẽ phản hồi trong thời gian sớm nhất.";
            return RedirectToAction(nameof(Index));
        }

        return View(contact);
    }
}