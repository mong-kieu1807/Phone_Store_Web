using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneStore.Data;
using PhoneStore.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneStore.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        //NTNguyen - API: Thêm đánh giá (chỉ rating hoặc cả rating + comment)
        [HttpPost]
        public async Task<IActionResult> ThemDanhGia(int productId, int rating, string? comment = null)
        {
            try
            {
                // Debug log
                Console.WriteLine($"ThemDanhGia called - ProductId: {productId}, Rating: {rating}, Comment: {comment ?? "null"}");

                // Kiểm tra đăng nhập
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null || userId <= 0)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để đánh giá!" });
                }

                int userIdValue = userId.Value;

                // Kiểm tra productId hợp lệ
                if (productId <= 0)
                {
                    return Json(new { success = false, message = "Sản phẩm không hợp lệ!" });
                }

                // Kiểm tra rating hợp lệ
                if (rating < 1 || rating > 5)
                {
                    return Json(new { success = false, message = "Đánh giá phải từ 1 đến 5 sao!" });
                }

                // Kiểm tra sản phẩm tồn tại
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại!" });
                }

                // Kiểm tra khách hàng đã mua sản phẩm này chưa
                var hasPurchased = await _context.BillDetails
                    .Include(bd => bd.Bill)
                    .AnyAsync(bd => bd.product_id == productId && 
                                   bd.Bill.user_id == userIdValue && 
                                   bd.Bill.status == 1);

                if (!hasPurchased)
                {
                    return Json(new { success = false, message = "Bạn chỉ có thể đánh giá sản phẩm đã mua!" });
                }

                // Kiểm tra user đã đánh giá sản phẩm này chưa
                var existingReview = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.product_id == productId && r.user_id == userIdValue);

                if (existingReview != null)
                {
                    // Cập nhật đánh giá cũ
                    existingReview.rating = rating;
                    existingReview.comment = string.IsNullOrWhiteSpace(comment) ? "" : comment.Trim();
                    existingReview.updated_at = DateTime.Now;
                    existingReview.status = 1;

                    await _context.SaveChangesAsync();

                    // Cập nhật rating trung bình cho Product
                    await CapNhatDanhGiaSanPham(productId);

                    return Json(new { success = true, message = "Đã cập nhật đánh giá của bạn!" });
                }
                else
                {
                    // Tạo đánh giá mới
                    var newReview = new Review
                    {
                        product_id = productId,
                        user_id = userIdValue,
                        rating = rating,
                        comment = string.IsNullOrWhiteSpace(comment) ? "" : comment.Trim(),
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now,
                        status = 1
                    };

                    _context.Reviews.Add(newReview);
                    await _context.SaveChangesAsync();

                    // Cập nhật rating trung bình cho Product
                    await CapNhatDanhGiaSanPham(productId);

                    return Json(new { success = true, message = "Cảm ơn bạn đã đánh giá!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
        //endNTNguyen

        //NTNguyen - Hàm cập nhật rating trung bình của sản phẩm
        private async Task CapNhatDanhGiaSanPham(int productId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.product_id == productId && r.status == 1)
                .ToListAsync();

            var product = await _context.Products.FindAsync(productId);
            if (product != null && reviews.Any())
            {
                product.rating = (decimal)reviews.Average(r => r.rating);
                await _context.SaveChangesAsync();
            }
        }
        //endNTNguyen

        //NTNguyen - API: Kiểm tra user đã đánh giá sản phẩm chưa??
        [HttpGet]
        public async Task<IActionResult> KiemTraDanhGiaUser(int productId)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null || userId <= 0)
                {
                    return Json(new { hasReviewed = false, rating = 0, comment = "", canReview = false, message = "Vui lòng đăng nhập" });
                }

                // Kiểm tra khách hàng đã mua sản phẩm này chưa?.?
                var hasPurchased = await _context.BillDetails
                    .Include(bd => bd.Bill)
                    .AnyAsync(bd => bd.product_id == productId && 
                                   bd.Bill.user_id == userId.Value && 
                                   bd.Bill.status == 1);

                if (!hasPurchased)
                {
                    return Json(new { hasReviewed = false, rating = 0, comment = "", canReview = false, message = "Bạn chưa mua sản phẩm này" });
                }

                var review = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.product_id == productId && r.user_id == userId.Value && r.status == 1);

                if (review != null)
                {
                    return Json(new { 
                        hasReviewed = true, 
                        rating = review.rating, 
                        comment = review.comment ?? "",
                        canReview = true
                    });
                }

                return Json(new { hasReviewed = false, rating = 0, comment = "", canReview = true });
            }
            catch (Exception ex)
            {
                return Json(new { hasReviewed = false, rating = 0, comment = "", canReview = false, error = ex.Message });
            }
        }
        //endNTNguyen

        //NTNguyen - API: Xóa đánh giá.
        [HttpPost]
        public async Task<IActionResult> XoaDanhGia(int reviewId)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null || userId <= 0)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập!" });
                }

                var review = await _context.Reviews.FindAsync(reviewId);
                if (review == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đánh giá!" });
                }

                // Kiểm tra quyền xóa (chỉ người tạo mới được xóa)
                if (review.user_id != userId.Value)
                {
                    return Json(new { success = false, message = "Bạn không có quyền xóa đánh giá này!" });
                }

                int productId = review.product_id;

                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                // Cập nhật lại rating trung bình
                await CapNhatDanhGiaSanPham(productId);

                return Json(new { success = true, message = "Đã xóa đánh giá!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
        //endNTNguyen
    }
}
