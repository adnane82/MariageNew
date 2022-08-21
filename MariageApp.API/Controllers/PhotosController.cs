using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MariageApp.API.Data;
using MariageApp.API.Dtos;
using MariageApp.API.Helpers;
using MariageApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MariageApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {

        private readonly IMariageRepository _repo;


        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IMariageRepository repo, IOptions<CloudinarySettings> cloudinaryConfig, IMapper mapper)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _repo = repo;
            _mapper = mapper;
            Account acc = new Account(
            _cloudinaryConfig.Value.CloudName,
            _cloudinaryConfig.Value.ApiKey,
            _cloudinaryConfig.Value.ApiSecret

            );
            _cloudinary = new Cloudinary(acc);



        }
        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepository = await _repo.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepository);
            return Ok(photo);
        }
        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm] PhotoForCreateDto photoForCreateDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var userFromRepo = await _repo.GetUser(userId);
            var file = photoForCreateDto.File;
            var uploadResult = new ImageUploadResult();
            if (file != null && file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation()
                       .Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }

            }
            photoForCreateDto.Url = uploadResult.Uri.ToString();
            photoForCreateDto.publicId = uploadResult.PublicId;
            var photo = _mapper.Map<Photo>(photoForCreateDto);
            if (!userFromRepo.Photos.Any(p => p.IsMain))
                photo.IsMain = true;
            userFromRepo.Photos.Add(photo);
            if (await _repo.SaveAll())

            {
                var PhotoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, PhotoToReturn);

            }


            return BadRequest("Problème dans l'ajout de la photo");




        }
         [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId,int id)
        {
             if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var userFromRepo = await _repo.GetUser(userId);
            if(!userFromRepo.Photos.Any(p=>p.Id==id))
            return Unauthorized();
            var DesiredMainPhoto = await _repo.GetPhoto(id);
            if(DesiredMainPhoto.IsMain)
            return BadRequest("C'est la photo principal");
            var CurrentMainPhoto = await _repo.GetMainPhotoForUser(userId);
            CurrentMainPhoto.IsMain=false;
            DesiredMainPhoto.IsMain=true;
            if(await _repo.SaveAll())
            return NoContent();
            return BadRequest("On peut pas modifier la photo principal");
            
        }
        [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePhoto(int userId,int id){
        if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var userFromRepo = await _repo.GetUser(userId);
            if(!userFromRepo.Photos.Any(p=>p.Id==id))
            return Unauthorized();
            var Photo = await _repo.GetPhoto(id);
            if(Photo.IsMain)
            return BadRequest("   Impossible de supprimer la photo principal");
            if(Photo.PublicId!=null){
                var deleteParams = new DeletionParams(Photo.PublicId);
                var result = this._cloudinary.Destroy(deleteParams);
                if(result.Result=="ok"){
                    _repo.Delete(Photo);
                }
            }
            if(Photo.PublicId==null){
                  _repo.Delete(Photo);
            }

            if(await _repo.SaveAll())
            return Ok();
            return BadRequest("  Echec de la suppression de la photo");
            
    }
}
}








