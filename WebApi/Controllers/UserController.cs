using ClassLibrary.DataContext;
using ClassLibrary.Models;
using ClassLibrary.Utilities;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly MedicalDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public UserController(MedicalDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpGet("[action]")]
        public ActionResult GetJwtToken()
        {
            try
            {
                var secureKey = _configuration["JWT:SecureKey"];
                if (string.IsNullOrEmpty(secureKey))
                {
                    return BadRequest("Secure key is not configured.");
                }
                var serializedToken = JWTProvider.CreateToken(secureKey, 10);

                return Ok(serializedToken);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating JWT token: {ex.Message}");
            }
        }

        [HttpPost("[action]")]
        public ActionResult<UserDto> Register(UserDto userDto)
        {
            try
            {
                var trimmedUsername = userDto.LastName.Trim();
                if (_dbContext.Users.Any(x => x.UserName.Equals(trimmedUsername)))
                {
                    return BadRequest("User already exists.");
                }

                var userRole = _dbContext.Roles.FirstOrDefault(x => x.RoleName == "User");
                if (userRole == null)
                {
                        userRole = new Role { RoleName = "Admin" };
                        _dbContext.Roles.Add(userRole);
                        _dbContext.SaveChanges(); // Save so it gets an ID
                }

                var b64salt = PasswordHashProvider.GetSalt();
                var b64hash = PasswordHashProvider.GetHash(userDto.Password, b64salt);

                var user = new User
                {
                    UserId = userDto.UserId,
                    UserName = userDto.UserName,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    PasswordHash = b64hash,
                    PasswordSalt = b64salt,
                    RoleId = userRole.RoleId,
                    Email = userDto.Email,
                    PhoneNumber = userDto.PhoneNumber
                };

                _dbContext.Add(user);
                _dbContext.SaveChanges();

                userDto.UserId = user.UserId;
                return Ok(userDto);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Error registering user: {ex.Message}");
            }
        }

        [HttpPost("[action]")]
        public ActionResult<UserDto> Login(UserLoginDto userDto)
        {
            try
            {
                var existingUser = _dbContext.Users
                    .FirstOrDefault(x => x.UserName == userDto.UserName);
                if (existingUser == null)
                {
                    return NotFound("User not found.");
                }

                var b64hash = PasswordHashProvider.GetHash(userDto.Password, existingUser.PasswordSalt);
                if (b64hash != existingUser.PasswordHash)
                {
                    return BadRequest("Invalid password.");
                }

                var secureKey = _configuration["Jwt:SecureKey"];
                var serializedToken = JWTProvider.CreateToken(secureKey, 10, userDto.UserName);

                return Ok(serializedToken);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error logging in user: {ex.Message}");
            }
        }

        [HttpPost("[action]")]
        public ActionResult<UserChangePasswordDto> ChangePassword(UserChangePasswordDto userChangePassword)
        {
            try
            {
                var trimmedUsername = userChangePassword.UserName.Trim();
                var existingUser = _dbContext.Users
                    .FirstOrDefault(x => x.UserName == trimmedUsername);
                if (existingUser == null)
                {
                    return BadRequest("User not found.");
                }
                existingUser.PasswordSalt = PasswordHashProvider.GetSalt();
                existingUser.PasswordHash = PasswordHashProvider.GetHash(userChangePassword.Password, existingUser.PasswordSalt);
                
                _dbContext.Update(existingUser);
                _dbContext.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error changing password: {ex.Message}");
            }
        }

        [HttpPost("[action]")]
        public ActionResult<UserDto> UpdateUserRole(UserPromoteDto userPromoteDto)
        {
            try
            {
                var trimmedUsername = userPromoteDto.UserName.Trim();
                var existingUser = _dbContext.Users
                    .FirstOrDefault(x => x.UserName == trimmedUsername);
                if (existingUser == null)
                {
                    return BadRequest("User not found.");
                }
                var userRole = _dbContext.Roles
                    .FirstOrDefault(x => x.RoleName == "Admin");

                if (userRole == null)
                {
                        userRole = new Role { RoleName = "Admin" };
                        _dbContext.Roles.Add(userRole);
                        _dbContext.SaveChanges(); // Save so it gets an ID
                   
                }

                existingUser.RoleId = userRole.RoleId;
                _dbContext.Update(existingUser);
                _dbContext.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating user role: {ex.Message}");
            }
        }
    }
}
