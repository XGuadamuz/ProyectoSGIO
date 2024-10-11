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
    // Configuraci�n de las pol�ticas de contrase�a
    options.Password.RequireDigit = true;                    // Requiere al menos un n�mero
    options.Password.RequiredLength = 8;                     // Longitud m�nima de la contrase�a
    options.Password.RequireNonAlphanumeric = true;          // Requiere al menos un s�mbolo
    options.Password.RequireUppercase = true;                // Requiere al menos una letra may�scula
    options.Password.RequireLowercase = true;                // Requiere al menos una letra min�scula
    options.Password.RequiredUniqueChars = 1;                // N�mero de caracteres �nicos requeridos

    // Configuraci�n adicional de bloqueo si se desea
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Tiempo de bloqueo si fallan los intentos de login
    options.Lockout.MaxFailedAccessAttempts = 5;             // N�mero m�ximo de intentos fallidos
    options.Lockout.AllowedForNewUsers = true;

    // Configuraci�n de usuario
    options.User.RequireUniqueEmail = true;                 // Requiere un email �nico por usuario
    options.SignIn.RequireConfirmedAccount = true;          // Requiere confirmaci�n de cuenta por correo electr�nico
})
.AddEntityFrameworkStores<AppDBContext>()                    // Habilita Entity Framework para Identity
.AddDefaultTokenProviders();                                 // Para habilitar la generaci�n de tokens para recuperaci�n de contrase�as, etc.

// Configuraci�n de autenticaci�n con cookies
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
    // The default HSTS value es 30 d�as. Puedes cambiarlo si es necesario para producci�n
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();  // Aseg�rate de que est� en el orden correcto
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

