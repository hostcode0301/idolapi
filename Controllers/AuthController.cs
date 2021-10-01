using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using idolapi.DB.DTOs;
using idolapi.DB.Models;
using idolapi.Helper.Authorization;
using idolapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace idolapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJWTGenerator _jWTGenerator;
        private readonly IMapper _mapper;

        public AuthController(IUserService userService, IJWTGenerator jWTGenerator, IMapper mapper)
        {
            _userService = userService;
            _jWTGenerator = jWTGenerator;
            _mapper = mapper;
        }

        /// <summary>
        /// Register function to allow user create new account
        /// </summary>
        /// <param name="user">User from body</param>
        /// <returns>SC: 200 / SC:400 / SC: 409</returns>
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            // Check model state
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // Map registerDTo to user model
            var user = _mapper.Map<User>(registerDTO);

            // Hash password & add user role
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Roles = new string[] { "user" };

            // Insert user to db
            var rs = await _userService.InsertUser(user);

            if (rs != 1) { return Conflict(new ResponseDTO(409, "User is already exist")); }

            return Created("", new ResponseDTO(201, "Insert done"));
        }

        /// <summary>
        /// Login method to check for user to login
        /// </summary>
        /// <param name="user">User data from Body</param>
        /// <returns>UserDTO / SC: 404 (Not found) / SC: 400 (Model)</returns>
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login([FromBody] LoginDTO loginDTO)
        {
            // Check valid model
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // Try to find user in db with username
            var existedUser = await _userService.GetUserByUsername(loginDTO.Username);

            if (existedUser == null)
            {
                return NotFound(new ResponseDTO(404, "Login fail"));
            }

            // Verify user password
            bool verified = BCrypt.Net.BCrypt.Verify(loginDTO.Password, existedUser.Password);

            if (verified == false)
            {
                return NotFound(new ResponseDTO(404, "Login fail"));
            }

            // Add role to claim list
            var claims = new List<Claim>{
                new Claim(ClaimTypes.Name, existedUser.Username)
            };

            foreach (var role in existedUser.Roles)
            {
                claims.Add(new Claim(role, role));
            }

            // Generate accessToken
            string accessToken = _jWTGenerator.GenerateAccessToken(claims);

            if (accessToken == null)
            {
                return NotFound(new ResponseDTO(404, "Login fail"));
            }

            // Map existed user to user DTO
            var userDTO = _mapper.Map<UserDTO>(existedUser);
            userDTO.AccessToken = accessToken;

            // Return UserDTO
            return Ok(userDTO);
        }
    }
}