using AutoMapper;
using DatingApp.Model.Entity;
using DatingApp.Model.User;
using DatingApp.Repository.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DateingApp.API.Controllers
{
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
        public async Task<ActionResult> GetUsers()
        {
            IEnumerable<User> users = await _userRepository.GetAllUsers();
            var result = _mapper.Map<IEnumerable<User>, IEnumerable<UserListDto>>(users);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUser(int id)
        {
            User user = await _userRepository.GetUser(id);
            var result = _mapper.Map<User, UserDataDto>(user);

            return Ok(result);
        }
    }
}
