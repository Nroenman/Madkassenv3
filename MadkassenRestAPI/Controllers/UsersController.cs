using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using ClassLibrary.Model;
using ClassLibrary;
using JWT.Algorithms;
using JWT.Builder;
using MadkassenRestAPI.Models;

namespace MadkassenRestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(ApplicationDbContext context, IConfiguration configuration) : ControllerBase
    {
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> GetAllUsers()
        {
            var users = await context.Users
                .Select(u => new Users
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Email = u.Email,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    Roles = u.Roles
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/Users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Users>> GetUserById(int id)
        {
            var user = await context.Users
                .Where(u => u.UserId == id)
                .Select(u => new Users
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Email = u.Email,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    Roles = u.Roles
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileRequest updateRequest)
        {
            // Extract token from authorization header
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { Message = "No token provided." });
            }

            // Get user details from the token
            var userProfile = await GetUserProfileFromToken(token);
            if (userProfile == null)
            {
                return Unauthorized(new { Message = "Invalid or expired token." });
            }

            // Fetch user from the database
            var user = await context.Users.FindAsync(int.Parse(userProfile.UserId));
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            // Only update if new value is not the placeholder "string"
            if (!string.IsNullOrEmpty(updateRequest.UserName) && updateRequest.UserName != "string" &&
                updateRequest.UserName != user.UserName)
            {
                user.UserName = updateRequest.UserName;
            }

            // Check if the email is being updated
            if (!string.IsNullOrEmpty(updateRequest.Email) && updateRequest.Email != "string" &&
                updateRequest.Email != user.Email)
            {
                var existingUserWithEmail =
                    await context.Users.FirstOrDefaultAsync(u => u.Email == updateRequest.Email);
                if (existingUserWithEmail != null)
                {
                    return BadRequest(new { Message = "Email is already in use." });
                }

                user.Email = updateRequest.Email;
            }

            // Handle password update
            if (!string.IsNullOrEmpty(updateRequest.OldPassword) && updateRequest.OldPassword != "string")
            {
                if (!VerifyPassword(updateRequest.OldPassword, user.PasswordHash))
                {
                    return BadRequest(new { Message = "Incorrect old password." });
                }
            }

            if (!string.IsNullOrEmpty(updateRequest.NewPassword) && updateRequest.NewPassword != "string")
            {
                user.PasswordHash = HashPassword(updateRequest.NewPassword);
            }

            // If nothing was updated, return a message saying that
            if (updateRequest.UserName == "string" && updateRequest.Email == "string" &&
                updateRequest.OldPassword == "string" && updateRequest.NewPassword == "string")
            {
                return BadRequest(new { Message = "No valid data provided to update." });
            }

            // Update the timestamp for last modification
            user.UpdatedAt = DateTime.UtcNow;

            // Save changes to the database
            await context.SaveChangesAsync();

            // Optionally, generate a new JWT token (if the user has updated their profile and you want to issue a new token)
            var newToken = JwtBuilder.Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(configuration["AppSettings:Token"])
                .Subject(user.UserId.ToString())
                .Issuer(configuration["AppSettings:Issuer"])
                .Audience(configuration["AppSettings:Audience"])
                .IssuedAt(DateTimeOffset.Now.DateTime)
                .ExpirationTime(DateTimeOffset.Now.AddHours(1).DateTime)
                .NotBefore(DateTimeOffset.Now.DateTime)
                .Id(Guid.NewGuid().ToString())
                .Encode();

            // Return a success message with the new token
            return Ok(new { Message = "Profile updated successfully.", Token = newToken });
        }


        // GET: api/Users/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { Message = "No token provided." });
            }

            try
            {
                var userProfile = await GetUserProfileFromToken(token);
                if (userProfile == null)
                {
                    return Unauthorized(new { Message = "Invalid or expired token." });
                }

                var user = await context.Users
                    .Where(u => u.UserId.ToString() == userProfile.UserId)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound(new { Message = "User not found." });
                }

                var userDetails = new
                {
                    user.UserId,
                    user.UserName,
                    user.Email,
                    user.CreatedAt,
                    user.UpdatedAt,
                    user.Roles
                };

                return Ok(userDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving user profile", Details = ex.Message });
            }
        }

        private async Task<UserProfile> GetUserProfileFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                var userId = jwtToken?.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

                if (userId == null)
                {
                    return null;
                }

                return new UserProfile
                {
                    UserId = userId
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            if (string.IsNullOrEmpty(user.UserName))
            {
                return BadRequest("UserName is required.");
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                return BadRequest("Email is required.");
            }

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                return BadRequest("Password is required.");
            }

            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                return BadRequest("Email is already in use.");
            }

            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            if (string.IsNullOrEmpty(user.UserName))
            {
                user.UserName = user.Email;
            }

            // Hash password using BCrypt
            user.PasswordHash = HashPassword(user.PasswordHash);

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
    
}