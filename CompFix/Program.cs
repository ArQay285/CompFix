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
builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireUppercase = false;
})
.AddRoles<IdentityRole>() // Додає підтримку ролей ("admin")
.AddEntityFrameworkStores<AppDbContext>()
.AddErrorDescriber<CustomIdentityErrorDescriber>(); // Підключає українські повідомлення

builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/login"; // Шлях для звичайного входу
    options.AccessDeniedPath = "/login"; // Куди перенаправляти, якщо немає прав
});

// Зв'язує секцію "CompanyInfo" з класом "CompanySettings"
builder.Services.Configure<CompanyInfo>(builder.Configuration.GetSection("CompanyInfo"));

builder.Services.AddScoped(typeof(ControlBaseDate<>));
var app = builder.Build();

// Створює роль admin
using (var scope = app.Services.CreateScope()) { // Створює область для отримання сервісів
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Створює роль Admin, якщо її не існує
    if (!await roleManager.RoleExistsAsync("admin")) {
        await roleManager.CreateAsync(new IdentityRole("admin"));
    }

    // Створює адміністратора
    string adminEmail = "admin@compfix.ua";
    string adminName = "admin"; // Унікальне ім'я для входу
    var adminUser = await userManager.FindByNameAsync(adminName);

    if (adminUser == null) {
        var newAdmin = new ApplicationUser {
            UserName = adminName,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(newAdmin, "admin4321*"); // Встановлює пароль для адміністратора
        if (result.Succeeded) {
            await userManager.AddToRoleAsync(newAdmin, "admin");
        }
    }
}

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