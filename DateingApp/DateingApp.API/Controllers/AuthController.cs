using AutoMapper;
using DateingApp.API.Services;
using Dating.Model.Entity;
using Dating.Model.User;
using Dating.Repository.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        public IAuthRepository _authRepository { get; }
        public IConfiguration _config { get; }
        private IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthController(IAuthRepository authRepository,
             IUserRepository userRepository,
             IConfiguration config,
             IMapper mapper,
              ITokenService tokenService,
         UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _authRepository = authRepository;
            _userRepository = userRepository;
            _config = config;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
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

            var result = await _userManager.CreateAsync(user, userModel.Password);


            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            //var createdUser = await _authRepository.Register(user, userModel.Password);
            //var userToReturn = _mapper.Map<UserDataDto>(createdUser);

            //return CreatedAtRoute("GetUser", new { controller = "User", id = createdUser.Id}, userToReturn);
            return StatusCode(201);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login([FromBody] UserForLoginDto userModel)
        {
            var user = await _authRepository.Login(userModel.UserName, userModel.Password);

            if (user == null)
            {
                return Unauthorized("invalid username or password.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync
                (user, userModel.Password, false);

            if (!result.Succeeded)
            {
                return Unauthorized("invalid username or password.");
            }


            var userView = _mapper.Map<UserListDto>(user);

            return Ok(new
            {
                token = _tokenService.CreateToken(user).Result,
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