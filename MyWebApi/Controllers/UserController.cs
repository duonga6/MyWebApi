using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyWebApi.Data;
using MyWebApi.Models;
using MyWebApi.Models.UserModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase 
    {
        private readonly AppSettings _appSetting;
        private readonly MyDbContext _context;
        public UserController(IOptions<AppSettings> appSetting, MyDbContext context)
        {
            _appSetting = appSetting.Value;
            _context = context;
        }

        [HttpPost("/api/Login")]
        public IActionResult Login(LoginModel model)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == model.UserName && u.Password == model.Password);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new ApiResponse
                {
                    Success = false,
                    Message = "Invalid username/password"
                });
            }



            return Ok( new ApiResponse
            {
                Success = true,
                Message = "Authenticate success",
                Data = GenerateToken(user)
            });
        }

        private string GenerateToken(User user)
        {
            var jwtTokenHandle = new JwtSecurityTokenHandler();

            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSetting.SecretKey);

            var tokenDecriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim("UserName", user.UserName)
                }),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandle.CreateToken(tokenDecriptor);

            return jwtTokenHandle.WriteToken(token);
        }
    }
}
