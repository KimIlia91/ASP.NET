using MagicVilla_VillaApi;
using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Repository;
using MagicVilla_VillaApi.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MagicVilla_VillaApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddResponseCaching();

builder.Services.AddScoped<IVillaRepository, VillaRepository>();
builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.AddApiVersioning(opt =>
{
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.DefaultApiVersion = new ApiVersion(1, 0);
});

builder.Services.AddVersionedApiExplorer(opt =>
{
    opt.GroupNameFormat = "'v'VVV";
    opt.SubstituteApiVersionInUrl = true; // Use api version from URL
});
 
builder.Services.AddAuthentication(i =>
{
    i.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    i.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(i =>
{
    i.RequireHttpsMetadata = false;
    i.SaveToken = true;
    i.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("ApiSettings:Secret"))),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddControllers(options => {
    //options.ReturnHttpNotAcceptable = true // если в API отправлен запрос не в формате JSOn выдает исключение 406 not acceptable
    options.CacheProfiles.Add("Default30", new CacheProfile // добавить профиль для кеша имя профиля Default30
    {
        Duration = 30
    });
}).AddNewtonsoftJson()
.AddXmlDataContractSerializerFormatters();  // Включает поддержку XML форматирования 

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Magic Villa API v1",
        Version = "v1.0",
        Description = "A sample ASP.NET Core Web API.",
        Contact = new OpenApiContact
        {
            Name = "Ilia Kim",
            Email = "paterfamelias@gmail.com",
            Url = new Uri("https://www.facebook.com/profile.php?id=100014170037618")
        },
        License = new OpenApiLicense
        {
            Name = "License example",
            Url = new Uri("https://www.example.com/license")
        }
    });
    c.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "Magic Villa API v2",
        Version = "v2.0",
        Description = "A sample ASP.NET Core Web API.",
        Contact = new OpenApiContact
        {
            Name = "Ilia Kim",
            Email = "paterfamelias@gmail.com",
            Url = new Uri("https://www.facebook.com/profile.php?id=100014170037618")
        },
        License = new OpenApiLicense
        {
            Name = "License example",
            Url = new Uri("https://www.example.com/license")
        }
    });
    var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization using the Bearer scheme.\r\n " +
        "Enter 'Bearer' and then your token in the text input below. " +
        "Example: \"Bearer 1234asdasd\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
        opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Magic_VillaV1");
        opt.SwaggerEndpoint("/swagger/v2/swagger.json", "Magic_VillaV2");
    });
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
