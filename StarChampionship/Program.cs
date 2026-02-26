using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using StarChampionship.Data;
using StarChampionship.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// ===========================
// CONFIGURAÇÃO DA PORTA (RENDER)
// ===========================
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
});

// ===========================
// CONNECTION STRING
// ===========================
var connectionString = builder.Configuration
    .GetConnectionString("StarChampionshipContext");

if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("Connection string não configurada.");
}

builder.Services.AddDbContext<StarChampionshipContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions => mySqlOptions.MigrationsAssembly("StarChampionship")
    ));

// ===========================
// SERVICES
// ===========================
builder.Services.AddScoped<PlayerService>();
builder.Services.AddScoped<GeneratorService>();
builder.Services.AddScoped<SeedingService>();


builder.Services.AddControllersWithViews();

var app = builder.Build();

// ===========================
// SEEDING AUTOMÁTICO
// ===========================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var seedingService = services.GetRequiredService<SeedingService>();
        seedingService.Seed();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro ao executar o seeding");
    }
}

// ===========================
// LOCALIZAÇÃO
// ===========================
var enUS = new CultureInfo("en-US");

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(enUS),
    SupportedCultures = new List<CultureInfo> { enUS },
    SupportedUICultures = new List<CultureInfo> { enUS }
};

app.UseRequestLocalization(localizationOptions);

// ===========================
// PIPELINE HTTP
// ===========================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();