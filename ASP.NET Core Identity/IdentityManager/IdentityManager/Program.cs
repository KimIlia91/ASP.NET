using IdentityManager;
using IdentityManager.Authorize;
using IdentityManager.Data;
using IdentityManager.Models;
using IdentityManager.Services;
using IdentityManager.Services.IServices;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(opt => 
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.AddScoped<IAuthorizationHandler, FirstNameHandler>();
builder.Services.AddScoped<INumberOfDaysForAccount, NumberOfDaysForAccount>();
builder.Services.AddTransient<IEmailSender, EmailSenderService>();
builder.Services.AddTransient<IMailService, MailService>();

builder.Services.Configure<IdentityOptions>(opt =>
{
    opt.Password.RequiredLength = 5;
    opt.Password.RequireLowercase = true;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(30);
    opt.Lockout.MaxFailedAccessAttempts = 2;
});

var facebook = builder.Configuration.GetSection("Facebook").Get<FacebookOptions>();

builder.Services.AddAuthentication().AddFacebook(opt =>
{
    opt.AppId = facebook.AppId;
    opt.AppSecret = facebook.AppSecret;
});

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    opt.AddPolicy("UserAndAdmin", policy => policy.RequireRole("Admin").RequireRole("User"));
    opt.AddPolicy("AdminCreateAccess", policy => policy.RequireAssertion(context =>
    AuthorizedHandler.AdminCreateAccessHandler(context)));

    opt.AddPolicy("AdminCreateEditDeleteAccess", policy => policy.RequireRole("Admin")
    .RequireClaim("create", "True")
    .RequireClaim("edit", "True")
    .RequireClaim("delete", "True"));

    opt.AddPolicy("AdminCreateEditDeleteOrSuperAdminAccess", policy => policy.RequireAssertion(context => 
    AuthorizedHandler.AuthorizedAdminWithClaimsOrSuperAdmin(context)));

    opt.AddPolicy("OnlySuperAdminChacker", policy => policy.Requirements.Add(new OnlySuperAdminChacker()));

    opt.AddPolicy("AdminWhenMore1000Days", policy => policy.Requirements.Add(new AdminWhenMore1000Days(1000)));

    opt.AddPolicy("FirstNameAuth", policy => policy.Requirements.Add(new FirstNameRequrment("billy")));
});

//builder.Services.ConfigureApplicationCookie(opt =>
//{
//    opt.AccessDeniedPath = new PathString("/Home/AccessDenied");
//});

builder.Services.AddRazorPages();
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
