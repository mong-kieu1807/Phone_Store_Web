using Microsoft.EntityFrameworkCore;
using PhoneStore.Data;
using PhoneStore.Helper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Ket noi database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection")));

// Đăng ký DatabaseHelper
builder.Services.AddScoped<DatabaseHelper>();

// Thêm dịch vụ Session (quan trọng cho giỏ hàng)
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // ?� s?a l?nh n�y cho ?�ng v?i .NET 8

app.UseRouting();

app.UseAuthorization();

// K�ch ho?t Session
app.UseSession();
// Areas routing
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
// ?� x�a b? .WithStaticAssets() g�y l?i

app.Run();