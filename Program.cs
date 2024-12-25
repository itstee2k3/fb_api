using api_bui_xuan_thang.Models;
using api_bui_xuan_thang.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.Password.RequireUppercase = false;  // Bỏ yêu cầu chữ cái in hoa
        options.Password.RequireDigit = false;      // Bỏ yêu cầu số
        options.Password.RequireNonAlphanumeric = false; // Bỏ yêu cầu ký tự không phải chữ hoặc số
        options.Password.RequiredLength = 6;        // Độ dài mật khẩu tối thiểu
        options.Password.RequiredUniqueChars = 1;   // Số ký tự duy nhất tối thiểu
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS: Cross-Origin Resource Sharing, được dịch là “Chia sẻ tài nguyên giữa các nguồn gốc khác nhau
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyAllowOrigins", policy =>
    {
        //Thay bằng địa chỉ localhost khi khởi chạy bên frontend (VSCode)
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

//Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JWTKey");
var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "Bearer";
        options.DefaultChallengeScheme = "Bearer";
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["ValidIssuer"],
            ValidAudience = jwtSettings["ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

//Tạo ra các Role trong ứng dụng để sau này thực hiện phân quyền
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

// Middleware for production error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error"); // Custom error handling endpoint
    app.UseHsts(); // Enforce HTTPS in production
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

// Áp dụng CORS cho các yêu cầu đến API
app.UseCors("MyAllowOrigins");

app.UseAuthentication();
app.UseAuthorization();

// app.MapIdentityApi<User>();

app.MapControllers();

app.Run();