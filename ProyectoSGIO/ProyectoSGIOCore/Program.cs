using ProyectoSGIOCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProyectoSGIOCore.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using ProyectoSGIOCore.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPasswordRecoveryService, PasswordRecoveryService>();

// Configurar AppDBContext solo una vez
builder.Services.AddDbContext<AppDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSQL"));
});

// Configurar Identity con el modelo de usuario personalizado 'Usuario'
builder.Services.AddIdentity<Usuario, IdentityRole>(options =>
{
    // Configuración de las políticas de contraseña
    options.Password.RequireDigit = true;                    // Requiere al menos un número
    options.Password.RequiredLength = 8;                     // Longitud mínima de la contraseña
    options.Password.RequireNonAlphanumeric = true;          // Requiere al menos un símbolo
    options.Password.RequireUppercase = true;                // Requiere al menos una letra mayúscula
    options.Password.RequireLowercase = true;                // Requiere al menos una letra minúscula
    options.Password.RequiredUniqueChars = 1;                // Número de caracteres únicos requeridos

    // Configuración adicional de bloqueo si se desea
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Tiempo de bloqueo si fallan los intentos de login
    options.Lockout.MaxFailedAccessAttempts = 5;             // Número máximo de intentos fallidos
    options.Lockout.AllowedForNewUsers = true;

    // Configuración de usuario
    options.User.RequireUniqueEmail = true;                 // Requiere un email único por usuario
    options.SignIn.RequireConfirmedAccount = true;          // Requiere confirmación de cuenta por correo electrónico
})
.AddEntityFrameworkStores<AppDBContext>()                    // Habilita Entity Framework para Identity
.AddDefaultTokenProviders();                                 // Para habilitar la generación de tokens para recuperación de contraseñas, etc.

// Configuración de autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Acceso/IniciarSesion";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value es 30 días. Puedes cambiarlo si es necesario para producción
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();  // Asegúrate de que está en el orden correcto
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

