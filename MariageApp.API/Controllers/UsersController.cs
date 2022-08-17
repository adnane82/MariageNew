using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MariageApp.API.Data;
using MariageApp.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MariageApp.API.Controllers
{
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
        public async Task<IActionResult> GetUsers(){

            var users = await  _repo.GetUsers();
            var usersToReturn=_mapper.Map<IEnumerable<UserForListDto>>(users);


            return Ok(usersToReturn);
        }
            [HttpGet("{id}")]
        public async Task<IActionResult> GetUser( int id){

            var user = await  _repo.GetUser(id);
            var userToReturn=_mapper.Map<UserForDetailsDto>(user);
            return Ok(userToReturn);
        }

    }
}