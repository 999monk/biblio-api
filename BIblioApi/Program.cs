using System.Text;
using BIblioApi.data;
using BIblioApi.services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Hangfire;
using BIblioApi.Configuration;
using BIblioApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configuration
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddOpenApi();
builder.Services.AddTransient<SeedDb>();
builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer("name=LocalConnection"));
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<ILibroService, LibroService>();
builder.Services.AddScoped<ISociosService, SociosService>();
builder.Services.AddScoped<IPrestamoService, PrestamoService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IEmailSender, MailKitEmailSender>();

//Cfg Hangfire
builder.Services.AddHangfire(config => config
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("LocalConnection")));
builder.Services.AddHangfireServer();


// Cfg JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!)),
            ValidateIssuer = false, 
            ValidateAudience = false 
        };
    });

builder.Services.AddAuthorization(); 

var app = builder.Build();

SeedData(app);
static void SeedData(WebApplication app)
{
    IServiceScopeFactory? scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using IServiceScope scope = scopedFactory!.CreateScope();
    SeedDb? service = scope.ServiceProvider.GetService<SeedDb>();
    service!.SeedAsync().Wait();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseErrorHandlingMiddleware();

app.UseHangfireDashboard();

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }