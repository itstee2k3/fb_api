using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api_bui_xuan_thang.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace api_bui_xuan_thang.Controllers;

public class AuthenticateController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthenticateController(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userExists = await _userManager.FindByNameAsync(model.Username);
        if (userExists != null)
            return StatusCode(StatusCodes.Status400BadRequest, new { Status = false, Message = "User already exists" });

        var user = new User
        {
            UserName = model.Username,
            Email = model.Email,
            Initials = model.Initials
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return StatusCode(StatusCodes.Status500InternalServerError, new { Status = false, Message = "User creation failed: " + errors });
        }

        // Assign Role if Provided
        if (!string.IsNullOrEmpty(model.Role))
        {
            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(model.Role));
            }
            await _userManager.AddToRoleAsync(user, model.Role);
        }

        return Ok(new { Status = true, Message = "User created successfully" });
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            return Unauthorized(new { Status = false, Message = "Invalid username or password" });

        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("userId", user.Id)  // Thêm userId vào claims
        };

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var token = GenerateToken(authClaims);
        return Ok(new { Status = true, Message = "Logged in successfully", Token = token });
    }


    //Thêm chức năng đăng xuất
    //Chỉ cần thông báo đăng xuất thành công ở server, không cần ghi thêm dữ liệu
    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        // Không cần làm gì ở phía server vì JWT là stateless
        return Ok(new { Status = true, Message = "Logged out successfully" });
    }

    
    private string GenerateToken(IEnumerable<Claim> claims)
    {
        var jwtSettings = _configuration.GetSection("JWTKey");
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(24), // Thời gian hết hạn là 1 ngày,
            Issuer = jwtSettings["ValidIssuer"],
            Audience = jwtSettings["ValidAudience"],
            SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
