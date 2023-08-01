
using asg_form;
using asg_form.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Security.Policy;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "ASG 赛事官网-后端API文档",
        Version = "V 1.9.6",
        Description = "这是由罗澜使用ASP.NET.Core开发的ASG赛事组后端系统，包括官网和后台管理系统。使用 sqlserver作为数据库，identity框架进行账号控制。",
    });
    var file = Path.Combine(AppContext.BaseDirectory, "asg_form.xml");
    var path = Path.Combine(AppContext.BaseDirectory, file);
    c.IncludeXmlComments(path, true);
    c.OrderActionsBy(o => o.RelativePath);
});




string[] urls = new[] { "https://idvasg.cn", "http://localhost:8080" , "https://admin.idvasg.cn", "https://asgadmin.pages.dev" };
builder.Services.AddCors(options =>
options.AddDefaultPolicy(builder => builder.WithOrigins(urls)
.AllowAnyMethod().AllowAnyHeader().AllowCredentials()));



IServiceCollection services = builder.Services;
services.AddDbContext<IDBcontext>(opt =>
{
    string connStr = @"Server=localhost\SQLEXPRESS;Database=master;Trusted_Connection=True;TrustServerCertificate=true";
    opt.UseSqlServer(connStr);
});
services.AddDataProtection();
services.AddIdentityCore<User>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
});
var idBuilder = new IdentityBuilder(typeof(User), typeof(Role), services);
idBuilder.AddEntityFrameworkStores<IDBcontext>()
    .AddDefaultTokenProviders()
    .AddRoleManager<RoleManager<Role>>()
    .AddUserManager<UserManager<User>>();





builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//add error 处理器
builder.Services.Configure<MvcOptions>(options =>
{
    options.Filters.Add<MyExceptionFilter>();
});




services.Configure<JWTOptions>(builder.Configuration.GetSection("JWT"));
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(x =>
{
    var jwtOpt = builder.Configuration.GetSection("JWT").Get<JWTOptions>();
    byte[] keyBytes = Encoding.UTF8.GetBytes(jwtOpt.SigningKey);
    var secKey = new SymmetricSecurityKey(keyBytes);
    x.TokenValidationParameters = new()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = secKey
    };
});




var app = builder.Build();
app.UseCors();
// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();

app.UseStaticFiles();

//发布时服务器注册静态资源
string fileUpload = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "loge");
if (!Directory.Exists(fileUpload))
{ Directory.CreateDirectory(fileUpload); }
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(fileUpload),
    RequestPath = "/loge"
});



app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();

app.Run();