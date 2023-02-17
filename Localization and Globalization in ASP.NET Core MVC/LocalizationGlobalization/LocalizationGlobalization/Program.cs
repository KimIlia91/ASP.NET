using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization(opt => opt.ResourcesPath = "Resources");
builder.Services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();
builder.Services.Configure<RequestLocalizationOptions>(opt =>
{
    var supportedCultures = new[]
                {
                new CultureInfo("en"),
                new CultureInfo("ru"),
                new CultureInfo("es"),
                new CultureInfo("ru-KG")
            };
    opt.DefaultRequestCulture = new RequestCulture(supportedCultures[0]);
    opt.SupportedCultures = supportedCultures;
    opt.SupportedUICultures = supportedCultures;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
 

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//var supportedCultures = new[]
//            {
//                new CultureInfo("en"),
//                new CultureInfo("ru"),
//                new CultureInfo("es"),
//                new CultureInfo("ru-KG")
//            };
//
//var lang = new RequestLocalizationOptions
//{
//    DefaultRequestCulture = new RequestCulture(supportedCultures[0]),
//    SupportedCultures = supportedCultures,
//    SupportedUICultures = supportedCultures
//};
//app.UseRequestLocalization(lang);
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
