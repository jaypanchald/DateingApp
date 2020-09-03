using DatingApp.Model.Entity;
using DatingApp.Model.User;
using DatingApp.Repository.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DateingApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        //IAuthRepository
        public IAuthRepository _authRepository { get; }
        public IConfiguration _config { get; }

        public AuthController(IAuthRepository authRepository, IConfiguration config)
        {
            _authRepository = authRepository;
            _config = config;
        }



        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Register([FromBody]UserTest userModel)
        {
            throw new Exception("It wont work");

            if (await _authRepository.UserExits(userModel.UserName))
            {
                return BadRequest("User aready exist.");
            }

            User user = new User()
            {
                UserName = userModel.UserName
            };

            await _authRepository.Register(user, userModel.Password);

            return StatusCode(201);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login([FromBody]UserForLoginDto userModel)
        {
            var user = await _authRepository.Login(userModel.UserName, userModel.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSetting:Token").Value));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();



            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });

        }
    }

    public class UserTest
    {
        [Required, EmailAddress]
        [MinLength(7), MaxLength(50)]

        public string UserName { get; set; }

        [Required]
        [MinLength(7), MaxLength(10)]
        public string Password { get; set; }
    }
}