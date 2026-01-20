using Microsoft.EntityFrameworkCore;
using PhoneStore.Data;
using PhoneStore.Helper;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

//Ket noi database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection")));

// Đăng ký DatabaseHelper
builder.Services.AddScoped<DatabaseHelper>();

// Thêm dịch vụ Session (quan trọng cho giỏ hàng)
builder.Services.AddSession();

// Thêm SignalR chat
builder.Services.AddSignalR();


var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

//  DÙNG CÁI NÀY THAY CHO MapStaticAssets
app.UseStaticFiles();

app.UseRouting();

// Kich hoat Session (PHẢI TRƯỚC UseAuthorization)
app.UseSession();

app.UseAuthorization();
// Areas routing
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map SignalR hub
app.MapHub<ChatHub>("/chatHub");

app.Run();
