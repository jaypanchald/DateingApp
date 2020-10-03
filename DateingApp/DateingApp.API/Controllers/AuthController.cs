using AutoMapper;
using DatingApp.Model.Entity;
using DatingApp.Model.User;
using DatingApp.Repository.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DateingApp.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        //IAuthRepository
        public IAuthRepository _authRepository { get; }
        public IConfiguration _config { get; }
        private IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository authRepository,
            IUserRepository userRepository,
            IConfiguration config,
            IMapper mapper)
        {
            _authRepository = authRepository;
            _userRepository = userRepository;
            _config = config;
            _mapper = mapper;
        }



        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userModel)
        {

            if (await _authRepository.UserExits(userModel.UserName))
            {
                return BadRequest("User aready exist.");
            }

            var user = _mapper.Map<User>(userModel);
            var createdUser = await _authRepository.Register(user, userModel.Password);
            var userToReturn = _mapper.Map<UserDataDto>(createdUser);

            //return CreatedAtRoute("GetUser", new { controller = "User", id = createdUser.Id}, userToReturn);
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

            var userView = _mapper.Map<UserListDto>(user);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                userPhoto = user?.Photos?.FirstOrDefault(f => f.IsMain)?.Url,
                user = userView
            });

        }

        [HttpGet]
        [Route("[action]")]
        public ActionResult Test()
        {
            var userData = _userRepository.GetAll();

            return Ok(userData);
        }
    }
}