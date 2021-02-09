using AutoMapper;
using DateingApp.API.Helper;
using Dating.Model.Entity;
using Dating.Model.Helper;
using Dating.Model.User;
using Dating.Repository.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DateingApp.API.Controllers
{

    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult> GetUsers([FromQuery]UserParams param)
        {
            param.UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (string.IsNullOrWhiteSpace(param.Gender))
            {
                var userFromRepo = await _userRepository.GetUser(param.UserId);
                param.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            }
            

            var users = await _userRepository.GetFilterUser(param);
            var result = _mapper.Map<IEnumerable<User>, IEnumerable<UserListDto>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount,
                users.TotalPages);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUser(int id)
        {
            User user = await _userRepository.GetUser(id);
            UserDataDto result = _mapper.Map<User, UserDataDto>(user);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var userFromRepo = await _userRepository.GetUser(id);
            _mapper.Map(userForUpdateDto, userFromRepo);

            if (await _userRepository.Update(userFromRepo))
            {
                return NoContent();
            }

            throw new System.Exception($"updating user {id} faild.");
        }
    }
}
