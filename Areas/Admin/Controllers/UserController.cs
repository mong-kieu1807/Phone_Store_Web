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
        private readonly IWebHostEnvironment _env;

        public UserController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            int totalProducts = await _context.Products.CountAsync();
			int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

			if (totalPages == 0) totalPages = 1;
			if (page < 1) page = 1;
			if (page > totalPages) page = totalPages;
            var users = await _context.Users
                .OrderBy(u => u.user_id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            ViewBag.Page = page;
			ViewBag.TotalPages = totalPages;
            return View(users);
        }

        // GET: AddUser
        public IActionResult AddUser()
        {
            return View();
        }

        // POST: AddUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if username already exists
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.username == user.username);
                    if (existingUser != null)
                    {
                        TempData["ErrorMessage"] = "Tên đăng nhập đã tồn tại.";
                        return View(user);
                    }

                    // Check if email already exists
                    var existingEmail = await _context.Users.FirstOrDefaultAsync(u => u.email == user.email);
                    if (existingEmail != null)
                    {
                        TempData["ErrorMessage"] = "Email đã tồn tại.";
                        return View(user);
                    }

                    user.created_at = DateTime.Now;
                    user.updated_at = DateTime.Now;
                    user.status = 1;

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Thêm người dùng thành công.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                }
            }
            return View(user);
        }

        // GET: EditUser
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // POST: EditUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if username already exists (except current user)
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.username == user.username && u.user_id != user.user_id);
                    if (existingUser != null)
                    {
                        TempData["ErrorMessage"] = "Tên đăng nhập đã tồn tại.";
                        return View(user);
                    }

                    // Check if email already exists (except current user)
                    var existingEmail = await _context.Users.FirstOrDefaultAsync(u => u.email == user.email && u.user_id != user.user_id);
                    if (existingEmail != null)
                    {
                        TempData["ErrorMessage"] = "Email đã tồn tại.";
                        return View(user);
                    }

                    user.updated_at = DateTime.Now;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật người dùng thành công.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                }
            }
            return View(user);
        }

        // Xóa người dùng (Chức năng tạm thời - Dữ liệu tĩnh)
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return Json(new { success = false, message = "Người dùng không tồn tại." });
            }
            _context.Users.Remove(user);
            _context.SaveChanges();
            return Json(new { success = true, message = "Người dùng đã được xóa." });
        }
    }
}
