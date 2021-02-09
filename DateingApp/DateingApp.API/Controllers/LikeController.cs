using Dating.Model.Entity;
using Dating.Repository.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DateingApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LikeController : ControllerBase
    {

        private readonly ILikeRepository _likeRepository;

        public LikeController(ILikeRepository likeRepository)
        {
            _likeRepository = likeRepository;
        }

        

        [HttpPost("{id}/[action]/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {

            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            Like like = await _likeRepository.GetLike(id, recipientId);

            if (like != null)
            {
                return BadRequest("You already like this user");
            }

            like = new Like()
            {
                LikerId = id,
                LikeeId = recipientId
            };

            if (await _likeRepository.Insert(like))
            {
                return Ok();
            }

            return BadRequest("Failed to like user.");
        }

    }
}
