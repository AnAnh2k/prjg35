using Microsoft.EntityFrameworkCore;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Đăng ký IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Thêm cấu hình chống giả mạo (Antiforgery) sử dụng header "X-CSRF-TOKEN"
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

// Lấy connection string từ cấu hình
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Cấu hình DbContext với SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Đăng ký các repository và view component nếu cần
builder.Services.AddScoped<INhomSpRepository, NhomSpRepository>();
builder.Services.AddScoped<ShoppingCartSummaryViewComponent>();

// Cấu hình cache phân tán và Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    // Bạn có thể thiết lập các tùy chọn cho Session nếu cần
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Cấu hình pipeline xử lý request
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Đảm bảo session middleware được gọi trước authorization
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
