using Microsoft.AspNetCore.Mvc;
using PhoneStore.Data;
using PhoneStore.Models;
using Microsoft.EntityFrameworkCore;

namespace PhoneStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Dữ liệu tĩnh mẫu
            var categories = new List<Category>
            {
                new Category { category_id = 1, category_name = "Điện thoại", description = "Các loại điện thoại thông minh", status = 1, created_at = DateTime.Now.AddMonths(-6), updated_at = DateTime.Now.AddDays(-10) },
                new Category { category_id = 2, category_name = "Laptop", description = "Máy tính xách tay", status = 1, created_at = DateTime.Now.AddMonths(-5), updated_at = DateTime.Now.AddDays(-8) },
                new Category { category_id = 3, category_name = "Tablet", description = "Máy tính bảng", status = 1, created_at = DateTime.Now.AddMonths(-4), updated_at = DateTime.Now.AddDays(-5) },
                new Category { category_id = 4, category_name = "Phụ kiện", description = "Phụ kiện điện thoại, laptop", status = 1, created_at = DateTime.Now.AddMonths(-3), updated_at = DateTime.Now.AddDays(-3) },
                new Category { category_id = 5, category_name = "Tai nghe", description = "Tai nghe không dây, có dây", status = 1, created_at = DateTime.Now.AddMonths(-2), updated_at = DateTime.Now.AddDays(-1) }
            };
            
            return View(categories);
        }

        // Xóa danh mục (Chức năng tạm thời - Dữ liệu tĩnh)
        [HttpPost]
        public IActionResult Delete(int id)
        {
            // Không thực hiện xóa thật sự - chỉ trả về thông báo
            return Json(new { success = true, message = "Chức năng đang phát triển - Dữ liệu tĩnh" });
        }
    }
}
