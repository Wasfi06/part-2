using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.DTOs;
using RaceDay.Middleware;

namespace RaceDay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public UsersController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        [SessionAuthorize]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = (int)HttpContext.Items["UserId"]!;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            return Ok(new
            {
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role,
                user.Phone,
                user.ProfileImageUrl,
                user.CreatedAt,
                user.IsActive
            });
        }

        [HttpPut("me")]
        [SessionAuthorize]
        public async Task<IActionResult> UpdateMyProfile(
            UpdateProfileRequest request)
        {
            var userId = (int)HttpContext.Items["UserId"]!;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound(new
                {
                    message = "User not found."
                });
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Phone = request.Phone;
            user.ProfileImageUrl = request.ProfileImageUrl;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Profile updated successfully."
            });
        }
    }
}
