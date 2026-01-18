using Microsoft.AspNetCore.Mvc;
using PhoneStore.Data;
using PhoneStore.Models;

namespace PhoneStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Chạy cái này để hiện form nhập liệu
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // 2. Chạy cái này khi người dùng bấm nút "Xác nhận"
        [HttpPost]
        public IActionResult ForgotPassword(string username, string email, string phone, string newPassword)
        {
            // Tìm user trong Database khớp cả 3 thông tin
            var user = _context.Users.FirstOrDefault(u =>
                u.username == username &&
                u.email == email &&
                u.phone == phone);

            // Nếu không tìm thấy ai
            if (user == null)
            {
                ViewBag.Error = "Thông tin xác minh không chính xác!";
                return View(); // Trả về form để nhập lại
            }

            // Nếu tìm thấy -> Đổi mật khẩu
            user.password = newPassword;
            _context.SaveChanges(); // Lưu xuống SQL

            ViewBag.Success = "Đổi mật khẩu thành công! Mật khẩu mới là: " + newPassword;
            return View();
        }
        public IActionResult InformationUser()
        {
            // Lấy userId từ Session
            var userId = HttpContext.Session.GetInt32("UserId");
            
            // Nếu chưa đăng nhập, redirect về trang đăng nhập
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            
            // Lấy thông tin user từ Database
            var user = _context.Users.FirstOrDefault(u => u.user_id == userId);
            
            if (user == null)
            {
                ViewBag.Error = "Không tìm thấy thông tin người dùng!";
                return View();
            }

            // Ẩn Search Bar và Navigation
            ViewData["HideSearchAndNav"] = true;
            return View(user);
        }

        [HttpPost]
        public IActionResult InformationUser(User model)
        {
            // Lấy userId từ Session
            var userId = HttpContext.Session.GetInt32("UserId");
            
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Lấy user từ Database
            var user = _context.Users.FirstOrDefault(u => u.user_id == userId);
            
            if (user == null)
            {
                ViewBag.Error = "Không tìm thấy thông tin người dùng!";
                return View(user);
            }

            // Cập nhật thông tin (không cập nhật username và password)
            user.full_name = model.full_name;
            user.email = model.email;
            user.phone = model.phone;
            user.address = model.address;
            user.updated_at = DateTime.Now;

            try
            {
                _context.SaveChanges();
                ViewBag.Message = "Cập nhật thông tin thành công!";
                // Cập nhật Session với thông tin mới
                HttpContext.Session.SetString("FullName", user.full_name ?? "User");
                HttpContext.Session.SetString("UserEmail", user.email ?? "");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi khi cập nhật thông tin: " + ex.Message;
            }

            ViewData["HideSearchAndNav"] = true;
            return View(user);
        }
        
    }
}