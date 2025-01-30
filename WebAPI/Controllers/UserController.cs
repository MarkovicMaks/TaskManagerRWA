using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using WebAPI.DTOs;
using TM.BL.Models;
using WebAPI.Security;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TaskMgmtContext _context;

        public UserController(IConfiguration configuration, TaskMgmtContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        
        [HttpPost("[action]")]
        public ActionResult<UserDto> Register(UserDto userDto)
        {
            try
            {
                // Check if there is such a username in the database already
                var trimmedUsername = userDto.Username.Trim();
                if (_context.Users.Any(x => x.Username.Equals(trimmedUsername)))
                    return BadRequest($"Username {trimmedUsername} already exists");

                // Hash the password
                var b64salt = PasswordHashProvider.GetSalt();
                var b64hash = PasswordHashProvider.GetHash(userDto.Password, b64salt);

                // Create user from DTO and hashed password
                var user = new User
                {
                    Id = userDto.Id,
                    Username = userDto.Username,
                    PwdHash = b64hash,
                    PwdSalt = b64salt,
                    Role = userDto.Role,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Email = userDto.Email,
                    Phone = userDto.Phone,
                };

                // Add user and save changes to the database
                _context.Add(user);
                _context.SaveChanges();

                // Update DTO Id to return it to the client
                userDto.Id = user.Id;

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, innerException);
            }
        }
        [HttpPost("[action]")]
        public ActionResult Login(UserLoginDto loginDto)
        {
            try
            {
                var genericLoginFail = "Incorrect username or password";

                // Try to get a user from database
                var existingUser = _context.Users.FirstOrDefault(x => x.Username == loginDto.Username);
                if (existingUser == null)
                    return Unauthorized(genericLoginFail);

                // Check is password hash matches
                var b64hash = PasswordHashProvider.GetHash(loginDto.Password, existingUser.PwdSalt);
                if (b64hash != existingUser.PwdHash)
                    return Unauthorized(genericLoginFail);

                var secureKey = _configuration["JWT:SecureKey"];
                var serializedToken = JwtTokenProvider.CreateToken(secureKey, 60, loginDto.Username);

                return Ok(serializedToken);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
