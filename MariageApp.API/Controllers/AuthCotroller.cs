using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MariageApp.API.Data;
using MariageApp.API.Dtos;
using MariageApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MariageApp.API.Controllers
{
    [Route("api/auth/")]
    [ApiController]
    public class AuthCotroller : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public AuthCotroller(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            _mapper = mapper;
            _config = config;
            _repo = repo;

        }

        [HttpPost("register")]

        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();
            if (await _repo.UserExists(userForRegisterDto.Username)) return BadRequest("L'utilisateur exste déjà");
            var userToCreate = _mapper.Map<User>(userForRegisterDto);
            
            var CreatedUser = await _repo.Register(userToCreate, userForRegisterDto.Password);
               var userToReturn = _mapper.Map<UserForDetailsDto>(CreatedUser);
            return CreatedAtRoute("GetUser",new{controller = "Users",id=CreatedUser.Id},userToReturn);


        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {

            var userFromRepo = await _repo.Login(userForLoginDto.username.ToLower(), userForLoginDto.password);
            if (userFromRepo == null) return Unauthorized();
            var claims = new[]{
             new Claim (ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
             new Claim(ClaimTypes.Name,userFromRepo.Username)


          };
            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(Key, SecurityAlgorithms.HmacSha512);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds

            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var user= this._mapper.Map<UserForListDto>(userFromRepo);
            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user


            });



        }



    }
}