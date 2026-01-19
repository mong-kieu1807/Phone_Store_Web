using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneStore.Data;
using PhoneStore.Models;
using PhoneStore.Helper; // Đảm bảo namespace này đúng với file AdminAuthorize của bạn

namespace PhoneStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize] // Bảo vệ: Chỉ Admin mới vào được
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Hiển thị danh sách đánh giá
        public async Task<IActionResult> Index()
        {
            // Kết hợp bảng Reviews + Users + Products để lấy đủ thông tin hiển thị
            var reviews = await _context.Reviews
                .OrderByDescending(r => r.created_at) // Mới nhất lên đầu
                .Join(_context.Users, r => r.user_id, u => u.user_id, (r, u) => new { r, u })
                .Join(_context.Products, ru => ru.r.product_id, p => p.product_id, (ru, p) => new ReviewAdminViewModel
                {
                    ReviewId = ru.r.review_id,
                    UserFullName = ru.u.full_name,
                    ProductName = p.product_name,
                    Rating = ru.r.rating,
                    Comment = ru.r.comment,
                    CreatedAt = ru.r.created_at,
                    Status = ru.r.status // status là byte (1: Hiện, 0: Ẩn)
                })
                .ToListAsync();

            return View(reviews);
        }

        // 2. Chức năng Duyệt (Ẩn/Hiện)
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                // Logic đảo ngược: Nếu đang 1 -> thành 0. Nếu đang 0 -> thành 1.
                // Ép kiểu (byte) vì DB của bạn dùng TINYINT
                review.status = (byte)(review.status == 1 ? 0 : 1);
                review.updated_at = DateTime.Now;

                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // 3. Chức năng Xóa
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}