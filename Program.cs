var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Thêm d?ch v? Session (quan tr?ng cho gi? hàng)
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
app.UseStaticFiles(); // ?ã s?a l?nh này cho ?úng v?i .NET 8

app.UseRouting();

app.UseAuthorization();

// Kích ho?t Session
app.UseSession();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
// ?ã xóa b? .WithStaticAssets() gây l?i

app.Run();