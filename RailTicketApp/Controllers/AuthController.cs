using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RailTicketApp.Models;
using RailTicketApp.Models.Dto;
using RailTicketApp.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RailTicketApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IConfiguration _configuration;

        public AuthController(IUserService _userService, IConfiguration configuration)
        {
            userService = _userService;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRegistrationDto registrationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var registrationResult = userService.RegisterUser(registrationDto);

            if (registrationResult)
            {
                return Ok(new { message = "Registration successful" });
            }
            else
            {
                return Conflict(new { message = "A user with this email already exists" });
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto loginDto)
        {
            var user = AuthenticateUser(loginDto);

            if (user != null)
            {
                var token = GenerateJwtToken(user);
                return Ok(new { token });
            }

            return Unauthorized();
        }

        private User AuthenticateUser(UserLoginDto loginDto)
        {
            return userService.AuthenticateUser(loginDto);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
