//NTNguyen
using Microsoft.AspNetCore.Mvc;
using PhoneStore.Data;
using PhoneStore.Models;
using Microsoft.EntityFrameworkCore;

namespace PhoneStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Load dữ liệu từ database
            var orders = await _context.Bills
                .Include(b => b.User)
                .OrderByDescending(b => b.created_at)
                .ToListAsync();

            // Dictionary cho các trạng thái
            ViewBag.StatusInfo = new Dictionary<string, (string Label, string CssClass)>
            {
                { "Chờ xử lý", ("Chờ xử lý", "label-warning") },
                { "Đã xác nhận", ("Đã xác nhận", "label-info") },
                { "Đang giao", ("Đang giao", "label-primary") },
                { "Hoàn thành", ("Hoàn thành", "label-success") },
                { "Đã hủy", ("Đã hủy", "label-danger") }
            };
            
            return View(orders);
        }

        /// Hiển thị trang tạo đơn hàng mới
        // GET: Admin/Order/Create
        public async Task<IActionResult> Create()
        {
            // Load danh sách người dùng đang hoạt động (status = 1)
            var activeUsers = await _context.Users
                .Where(u => u.status == 1)
                .ToListAsync();
            ViewBag.Users = activeUsers;

            // Load danh sách phương thức thanh toán
            var paymentMethods = await _context.Payment_Methods.ToListAsync();
            ViewBag.PaymentMethods = paymentMethods;

            return View();
        }

        /// Xử lý tạo đơn hàng mới
        // POST: Admin/Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("user_id,total_amount,shipping_status,payment_method_id")] Bill bill)
        {
            // Kiểm tra dữ liệu đầu vào hợp lệ
            if (!ModelState.IsValid)
            {
                // Nếu có lỗi, load lại danh sách và hiển thị lại form
                await LoadCreateFormData();
                return View(bill);
            }

            // Thiết lập thông tin mặc định cho đơn hàng mới
            bill.created_at = DateTime.Now;
            bill.updated_at = DateTime.Now;
            bill.status = 1; // Trạng thái hoạt động
            bill.shipping_status = bill.shipping_status ?? "Chờ xử lý"; // Mặc định "Chờ xử lý"

            // Lưu đơn hàng vào database
            _context.Add(bill);
            await _context.SaveChangesAsync();

            // Hiển thị thông báo thành công
            TempData["SuccessMessage"] = "Thêm đơn hàng thành công!";
            return RedirectToAction(nameof(Index));
        }

        /// Hiển thị trang chỉnh sửa đơn hàng
        // GET: Admin/Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Kiểm tra id có được cung cấp không
            if (id == null)
            {
                return NotFound();
            }

            // Tìm đơn hàng theo id
            var bill = await _context.Bills.FindAsync(id);
            
            // Kiểm tra đơn hàng có tồn tại không
            if (bill == null)
            {
                return NotFound();
            }

            // Load danh sách người dùng và phương thức thanh toán để hiển thị trong form
            await LoadEditFormData();

            return View(bill);
        }

        /// Xử lý cập nhật thông tin đơn hàng
        // POST: Admin/Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("order_id,user_id,total_amount,shipping_status,payment_method_id,created_at,updated_at,status")] Bill bill)
        {
            // Kiểm tra id trong URL có khớp với id trong form không
            if (id != bill.order_id)
            {
                return NotFound();
            }

            // Kiểm tra dữ liệu đầu vào hợp lệ
            if (!ModelState.IsValid)
            {
                await LoadEditFormData();
                return View(bill);
            }

            try
            {
                // Cập nhật thời gian chỉnh sửa
                bill.updated_at = DateTime.Now;

                // Cập nhật thông tin đơn hàng
                _context.Update(bill);
                await _context.SaveChangesAsync();

                // Hiển thị thông báo thành công
                TempData["SuccessMessage"] = "Cập nhật đơn hàng thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                // Xử lý trường hợp đơn hàng bị xóa hoặc thay đổi bởi người khác
                if (!BillExists(bill.order_id))
                {
                    return NotFound();
                }
                
                // Nếu lỗi khác, throw lại exception
                throw;
            }
        }
        /// Xóa đơn hàng (AJAX)
        // POST: Admin/Order/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Tìm đơn hàng cần xóa
            var bill = await _context.Bills.FindAsync(id);
            
            // Kiểm tra đơn hàng có tồn tại không
            if (bill == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
            }
                
            try
            {
                // Xóa đơn hàng khỏi database
                _context.Bills.Remove(bill);
                await _context.SaveChangesAsync();

                // Trả về kết quả thành công
                return Json(new { success = true, message = "Xóa đơn hàng thành công!" });
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có exception
                return Json(new { success = false, message = $"Lỗi khi xóa đơn hàng: {ex.Message}" });
            }
        }

        #region Helper Methods

        /// Kiểm tra đơn hàng có tồn tại không
        private bool BillExists(int id)
        {
            return _context.Bills.Any(e => e.order_id == id);
        }

        /// Load dữ liệu cho form tạo đơn hàng
        private async Task LoadCreateFormData()
        {
            ViewBag.Users = await _context.Users.Where(u => u.status == 1).ToListAsync();
            ViewBag.PaymentMethods = await _context.Payment_Methods.ToListAsync();
        }

        /// Load dữ liệu cho form chỉnh sửa đơn hàng
        private async Task LoadEditFormData()
        {
            ViewBag.Users = await _context.Users.Where(u => u.status == 1).ToListAsync();
            ViewBag.PaymentMethods = await _context.Payment_Methods.ToListAsync();
        }

        #endregion
    }
}
//endNTNguyen
