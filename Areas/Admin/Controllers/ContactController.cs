//NTBinh 21/01
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneStore.Data; 
using PhoneStore.Models;
using PhoneStore.Helper; 

namespace PhoneStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize] 
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Hiển thị danh sách liên hệ
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách, sắp xếp tin mới nhất lên đầu
            var contacts = await _context.Contacts
                                         .OrderByDescending(c => c.created_at)
                                         .ToListAsync();
            return View(contacts);
        }

        // 2. Xử lý cập nhật trạng thái (Dùng cho Modal hoặc Form)
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            // Cập nhật trạng thái và thời gian cập nhật
            contact.status = status;
            contact.updated_at = DateTime.Now;

            _context.Contacts.Update(contact);
            await _context.SaveChangesAsync();

            // Thông báo nhỏ (nếu bạn có cài TempData alert)
            TempData["Success"] = "Cập nhật trạng thái thành công!";

            return RedirectToAction("Index");
        }

        // 3. Xóa liên hệ
        public async Task<IActionResult> Delete(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa liên hệ!";
            }
            return RedirectToAction("Index");
        }
    }
}
//NTBinh 21/01