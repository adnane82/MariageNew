using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MariageApp.API.Data;
using MariageApp.API.Dtos;
using MariageApp.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZwajApp.API.Helpers;

namespace MariageApp.API.Controllers
{ 
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/users/")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMariageRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IMariageRepository repo,IMapper mapper)
        {
            _repo = repo ;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams){
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
               var userFromRepo = await _repo.GetUser(currentUserId);
                userParams.UserId=currentUserId;
                 if(string.IsNullOrEmpty(userParams.Gender)){
                userParams.Gender=userFromRepo.Gender=="Homme"?"Femme":"Homme";
            }

            var users = await  _repo.GetUsers(userParams);
            var usersToReturn=_mapper.Map<IEnumerable<UserForListDto>>(users);
            Response.AddPagination(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages);


            return Ok(usersToReturn);
        }
            [HttpGet("{id}",Name ="GetUser")]
        public async Task<IActionResult> GetUser( int id){

            var user = await  _repo.GetUser(id);
            var userToReturn=_mapper.Map<UserForDetailsDto>(user);
            return Ok(userToReturn);
        }
        [HttpPut("{id}")]
         public async Task<IActionResult> UpdateUser(int id,UserForUpdateDto userForUpdateDto){
             if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
             return Unauthorized();
             var userFromRepo = await _repo.GetUser(id);
             _mapper.Map(userForUpdateDto,userFromRepo);
             if(await _repo.SaveAll())
                 return NoContent();
             

             throw new Exception($"Un Problème est survenu {id}");
             
             
         }

    }
}