using CompFix;
using CompFix.Models;
using CompFix.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Додавання підключення до бази даних
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Підключення Identity з підтримкою ролей
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>() // Додаємо підтримку ролей ("admin")
.AddEntityFrameworkStores<AppDbContext>();

builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/admin/login"; // Якщо неавторизований іде в /admin, його кидає сюди
});

// Зв'язуємо секцію "CompanyInfo" з класом "CompanySettings"
builder.Services.Configure<CompanyInfo>(builder.Configuration.GetSection("CompanyInfo"));

builder.Services.AddScoped(typeof(ControlBaseDate<>));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();