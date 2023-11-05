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
using System.Security.Cryptography;

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

        private TokenModel GenerateToken(User user)
        {
            var jwtTokenHandle = new JwtSecurityTokenHandler();

            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSetting.SecretKey);

            var tokenDecriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("UserId", user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddSeconds(20),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandle.CreateToken(tokenDecriptor);
            var accessToken = jwtTokenHandle.WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            // Lưu token db
            var refreshTokenEntity = new RefreshToken
            { 
                Id = Guid.NewGuid(),
                JwtId = token.Id,
                Token = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                IssuedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddHours(1),
                UserId = user.Id,
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            _context.SaveChanges();

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
            }    
            return Convert.ToBase64String(random);
        }

        [HttpPost("/api/RenewToken")]
        public IActionResult RenewToken(TokenModel model)
        {
            var jwtTokenHandle = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSetting.SecretKey);
            var tokenValidateParams = new TokenValidationParameters()
            {
                // Tự cấp token
                ValidateIssuer = false,
                ValidateAudience = false,

                // Ký vào token
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false // không kiểm tra hết hạn, nếu kiểm tra sẽ xảy ra Exception
            };

            try
            {
                // Check 1: Check format access token
                var tokenInVerification = jwtTokenHandle.ValidateToken(model.AccessToken, tokenValidateParams, out var validatedToken);
                if (tokenInVerification == null)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new ApiResponse
                    {
                        Success = false,
                        Message = "Token is not verified"
                    });
                }


                // Check 2: Check alg
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)
                        return StatusCode(StatusCodes.Status401Unauthorized, new ApiResponse
                        {
                            Success = false,
                            Message = "Token is invalid"
                        });
                }

                // Check 3: Check AccessToken expired
                var utcExpiredDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)!.Value);
                var expiredDate = ConvertUnixTimeToDateTime(utcExpiredDate);
                if (expiredDate >  DateTime.UtcNow)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new ApiResponse
                    {
                        Success = false,
                        Message = "AccessToken is not expired"
                    });
                }

                // Check 4: Check RefreshToken exist in DB
                var storedToken = _context.RefreshTokens.FirstOrDefault(t => t.Token == model.RefreshToken);
                if (storedToken == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ApiResponse
                    {
                        Success = false,
                        Message = "RefreshToken doesn't exist"
                    });
                }    

                // Check 5: Chek RT is used or revoked

                if (storedToken.IsUsed)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new ApiResponse
                    {
                        Success = false,
                        Message = "RefreshToken is used"
                    });
                }

                if (storedToken.IsRevoked)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new ApiResponse
                    {
                        Success = false,
                        Message = "RefreshToken has been revoked"
                    });
                }

                // Check 6: Check AccessToken and JwtId
                var jti = tokenInVerification.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                if (jti == null)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new ApiResponse
                    {
                        Success = false,
                        Message = "Token not found"
                    });
                }  
                
                if (jti != storedToken.JwtId)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new ApiResponse
                    {
                        Success = false,
                        Message = "Token doesn't match"
                    });
                }

                // Check 7: Check UserID in Token
                if (!int.TryParse(tokenInVerification.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out int userId))
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new ApiResponse
                    {
                        Success = false,
                        Message = "Token illegal"
                    });
                }

                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new ApiResponse
                    {
                        Success = false,
                        Message = "Token illegal"
                    });
                }

                // Update Token
                storedToken.IsRevoked = true;
                storedToken.IsUsed = true;
                _context.Update(storedToken);
                _context.SaveChanges();

                // Create new Token
                var token = GenerateToken(user);

                return Ok( new ApiResponse
                {
                    Success = true,
                    Message = "Renewed success",
                    Data = token
                });

            }
            catch
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ApiResponse
                {
                    Success = false,
                    Message = "Something went wrong"
                });
            }

        }

        private DateTime ConvertUnixTimeToDateTime(long utcExpiredDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return dateTimeInterval.AddSeconds(utcExpiredDate);
        }
    }
}
