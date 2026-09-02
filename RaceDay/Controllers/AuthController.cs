using Microsoft.AspNetCore.Mvc;
using RaceDay.DTOs;
using RaceDay.Services;

namespace RaceDay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly SessionService _sessionService;

        public AuthController(
            AuthService authService,
            SessionService sessionService)
        {
            _authService = authService;
            _sessionService = sessionService;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);

            if (!result.Success)
            {
                return BadRequest(new
                {
                    message = result.Message
                });
            }

            return Ok(new
            {
                message = result.Message,
                userId = result.User!.UserId,
                firstName = result.User.FirstName,
                lastName = result.User.LastName,
                email = result.User.Email,
                role = result.User.Role
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);

            if (!result.Success)
            {
                return Unauthorized(new
                {
                    message = result.Message
                });
            }

            var session = await _sessionService.CreateSessionAsync(result.User!);

            return Ok(new
            {
                message = result.Message,
                sessionId = session.SessionId,
                expiresAt = session.ExpiresAt,
                userId = result.User.UserId,
                firstName = result.User.FirstName,
                lastName = result.User.LastName,
                email = result.User.Email,
                role = result.User.Role
            });
        }

        [HttpPost("logout/{sessionId:guid}")]
        public async Task<IActionResult> Logout(Guid sessionId)
        {
            var revoked = await _sessionService.RevokeSessionAsync(sessionId);

            if (!revoked)
            {
                return NotFound(new
                {
                    message = "Session not found."
                });
            }

            return Ok(new
            {
                message = "Logout successful."
            });
        }
    }
}
