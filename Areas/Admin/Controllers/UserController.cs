using Microsoft.AspNetCore.Mvc;
using PhoneStore.Data;
using PhoneStore.Models;
using Microsoft.EntityFrameworkCore;

namespace PhoneStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Dữ liệu tĩnh mẫu
            var users = new List<User>
            {
                new User { user_id = 1, username = "admin", password = "hashed_password", full_name = "Quản trị viên", email = "admin@phonestore.com", phone = "0901234567", address = "123 Admin St", role = "Admin", status = 1, created_at = DateTime.Now.AddMonths(-12), updated_at = DateTime.Now.AddDays(-5) },
                new User { user_id = 2, username = "user1", password = "hashed_password", full_name = "Nguyễn Văn A", email = "user1@example.com", phone = "0902345678", address = "456 User St", role = "User", status = 1, created_at = DateTime.Now.AddMonths(-6), updated_at = DateTime.Now.AddDays(-10) },
                new User { user_id = 3, username = "user2", password = "hashed_password", full_name = "Trần Thị B", email = "user2@example.com", phone = "0903456789", address = "789 Customer Ave", role = "User", status = 1, created_at = DateTime.Now.AddMonths(-4), updated_at = DateTime.Now.AddDays(-3) },
                new User { user_id = 4, username = "manager", password = "hashed_password", full_name = "Lê Văn C", email = "manager@phonestore.com", phone = "0904567890", address = "321 Manager Rd", role = "Manager", status = 1, created_at = DateTime.Now.AddMonths(-8), updated_at = DateTime.Now.AddDays(-2) },
                new User { user_id = 5, username = "user3", password = "hashed_password", full_name = "Phạm Thị D", email = "user3@example.com", phone = "0905678901", address = "654 Buyer Ln", role = "User", status = 1, created_at = DateTime.Now.AddMonths(-2), updated_at = DateTime.Now.AddDays(-1) }
            };
            
            return View(users);
        }

        // Xóa người dùng (Chức năng tạm thời - Dữ liệu tĩnh)
        [HttpPost]
        public IActionResult Delete(int id)
        {
            // Không thực hiện xóa thật sự - chỉ trả về thông báo
            return Json(new { success = true, message = "Chức năng đang phát triển - Dữ liệu tĩnh" });
        }
    }
}
