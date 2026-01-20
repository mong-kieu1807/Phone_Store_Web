using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PhoneStore.Helper
{
    /// Attribute để bảo vệ các action chỉ dành cho Admin
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var maNguoiDung = context.HttpContext.Session.GetInt32("UserId");
            var vaiTro = context.HttpContext.Session.GetString("UserRole");

            // Kiểm tra nếu chưa đăng nhập hoặc không phải Admin.NTBinh 19/01
            if (maNguoiDung == null ||
                string.IsNullOrEmpty(vaiTro) ||
                !vaiTro.Trim().Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", new { area = "" });
            }
            base.OnActionExecuting(context);
        }
    }

    /// Attribute để bảo vệ các action yêu cầu đăng nhập cho cả 2

    public class AuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var maNguoiDung = context.HttpContext.Session.GetInt32("UserId");

            // Kiểm tra nếu chưa đăng nhập
            if (maNguoiDung == null)
            {
                // Lưu URL hiện tại để redirect lại sau khi login
                var urlQuayLai = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                
                // Redirect về trang login
                context.Result = new RedirectToActionResult("Login", "Auth", new { area = "", returnUrl = urlQuayLai });
            }

            base.OnActionExecuting(context);
        }
    }
}
