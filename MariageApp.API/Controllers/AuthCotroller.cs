using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
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
        public AuthCotroller(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;

        }

        [HttpPost("register")]

        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();
            if (await _repo.UserExists(userForRegisterDto.Username)) return BadRequest("L'utilisateur exste déjà");
            var userToCreate = new User
            {
                Username = userForRegisterDto.Username
            };
            var CreatedUser = await _repo.Register(userToCreate, userForRegisterDto.Password);
            return StatusCode(201);


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
            var creds = new SigningCredentials (Key, SecurityAlgorithms.HmacSha512);
            var tokenDescriptor = new SecurityTokenDescriptor {
            Subject= new ClaimsIdentity(claims),
            Expires=DateTime.Now.AddDays(1),
            SigningCredentials =creds

            };
            var tokenHandler= new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new {
                token = tokenHandler.WriteToken(token)
              

            });



        }



    }
}