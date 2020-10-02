using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DateingApp.FileStorage;
using DatingApp.Model.Entity;
using DatingApp.Model.Photo;
using DatingApp.Repository.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace DateingApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

    public class PhotosController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPhotoRepository _photoRepository;
        private readonly IFileHelper _fileHelper;
        private readonly IMapper _mapper;

        public PhotosController(IUserRepository userRepository,
            IPhotoRepository photoRepository,
            IFileHelper fileHelper,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _photoRepository = photoRepository;
            _fileHelper = fileHelper;
            _mapper = mapper;
        }

        [HttpGet("{id}", Name = "[action]")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photo = await _photoRepository.GetbyId(id);
            var result = _mapper.Map<PhotoForReturnDto>(photo);

            return Ok(result);
        }


        [HttpPost("{userId}")]
        public async Task<ActionResult> AddPhotoForUser(int userId,
            [FromForm] PhotoForCreationDto bindingModel)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var user = await _userRepository.GetUser(userId);

            var file = bindingModel.File;

            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation()
                        .Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _fileHelper.upload(uploadParams);
                }
            }

            bindingModel.Url = uploadResult.Url.ToString();
            bindingModel.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(bindingModel);

            if (user.Photos == null || !user.Photos.Any(a => a.IsMain))
            {
                photo.IsMain = true;
            }
            if (photo.UserId == 0)
            {
                photo.UserId = userId;
            }

            if (await _photoRepository.Insert(photo))
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
            }

            return BadRequest("could not add photo");
        }

        [HttpPost("{userId}/{id}/setMain")]
        public async Task<IActionResult> setMain(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            bool photoExist = await _photoRepository.ExistAnyPhoto(userId, id);


            if (!photoExist)
            {
                return Unauthorized();
            }

            List<Photo> photos = await _photoRepository.GetListOfPhotosOfUser(userId);

            if (photos.FirstOrDefault(f => f.Id == id).IsMain)
            {
                return BadRequest("This is aready the meain photo");
            }

            photos.Where(f => f.IsMain).ToList().ForEach(x => x.IsMain = false);
            photos.FirstOrDefault(f => f.Id == id).IsMain = true;

            if (await _photoRepository.UpdateAll(photos))
            {
                return NoContent();
            }
            return BadRequest("could not set main image.");

        }


        [HttpDelete("{userId}/{id}/DeletePhoto")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            bool photoExist = await _photoRepository.ExistAnyPhoto(userId, id);


            if (!photoExist)
            {
                return Unauthorized();
            }

            List<Photo> photos = await _photoRepository.GetListOfPhotosOfUser(userId);

            if (photos.FirstOrDefault(f => f.Id == id).IsMain)
            {
                return BadRequest("You can not delete main photo.");
            }

            Photo photo = photos.FirstOrDefault(f => f.Id == id);

            if (photo != null && !string.IsNullOrEmpty(photo.PublicId))
            {
                if (await _fileHelper.DeleteFile(photo.PublicId))
                {

                }
            }

            if (await _photoRepository.Delete(photo))
            {
                return Ok();
            }

            return BadRequest("failed to delete photo.");
        }
    }
}
