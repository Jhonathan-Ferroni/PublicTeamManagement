using Microsoft.AspNetCore.Localization;
using PublicTeamManagement.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// INJEÇÃO DE DEPENDÊNCIA (SERVICES)
// ==========================================
// Adicionamos os serviços necessários para a lógica de CSV e geração de times
builder.Services.AddScoped<PlayerService>();
builder.Services.AddScoped<GeneratorService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ==========================================
// CONFIGURAÇÃO DE LOCALIZAÇÃO (IMPORTANTE PARA CSV)
// ==========================================
// Como o CSV usa pontos e vírgulas, forçar en-US garante que 
// números decimais (Overall) usem ponto como separador.
var enUS = new CultureInfo("en-US");
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(enUS),
    SupportedCultures = new List<CultureInfo> { enUS },
    SupportedUICultures = new List<CultureInfo> { enUS }
};

app.UseRequestLocalization(localizationOptions);

// ==========================================
// PIPELINE HTTP (ORDEM IMPORTANTE)
// ==========================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Garante que o servidor consiga servir a pasta wwwroot (onde o CSV e as imagens ficam)
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();