using Microsoft.AspNetCore.Mvc;
using PhoneStore.Data;
using PhoneStore.Models;
using Microsoft.EntityFrameworkCore;

namespace PhoneStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupplierController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Dữ liệu tĩnh mẫu
            var suppliers = new List<Supplier>
            {
                new Supplier { supplier_id = 1, supplier_name = "Công ty TNHH Apple Việt Nam", address = "123 Lê Duẩn, Q1, TP.HCM", phone = "0281234567", email = "apple@vietnam.vn", status = 1 },
                new Supplier { supplier_id = 2, supplier_name = "Samsung Việt Nam", address = "456 Nguyễn Huệ, Q1, TP.HCM", phone = "0282345678", email = "samsung@vn.com", status = 1 },
                new Supplier { supplier_id = 3, supplier_name = "Xiaomi Store Vietnam", address = "789 Hai Bà Trưng, Q3, TP.HCM", phone = "0283456789", email = "xiaomi@vn.com", status = 1 },
                new Supplier { supplier_id = 4, supplier_name = "Oppo Mobile VN", address = "321 Điện Biên Phủ, Q10, TP.HCM", phone = "0284567890", email = "oppo@vietnam.vn", status = 1 },
                new Supplier { supplier_id = 5, supplier_name = "Vivo Vietnam Co., Ltd", address = "654 Lý Thường Kiệt, Q5, TP.HCM", phone = "0285678901", email = "vivo@vn.com", status = 1 }
            };
            
            return View(suppliers);
        }

        // Xóa nhà cung cấp (Chức năng tạm thời - Dữ liệu tĩnh)
        [HttpPost]
        public IActionResult Delete(int id)
        {
            // Không thực hiện xóa thật sự - chỉ trả về thông báo
            return Json(new { success = true, message = "Chức năng đang phát triển - Dữ liệu tĩnh" });
        }
    }
}
