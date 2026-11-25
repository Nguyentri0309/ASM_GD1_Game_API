using GameApi_ASM_NguyenTriThinh.DTOs;
using GameApi_ASM_NguyenTriThinh.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameApi_ASM_NguyenTriThinh.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Tags("Auth")]
    [ApiExplorerSettings(GroupName = "Auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // POST api/auth/register
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(request);
            if (!result.Success)
            {
                return Conflict(new { message = result.Message });
            }

            return StatusCode(StatusCodes.Status201Created, result.Account);
        }

        // POST api/auth/login
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(request);
            if (result == null)
            {
                // Sai username hoac mat khau deu chung mot thong bao
                return Unauthorized(new { message = "Sai ten dang nhap hoac mat khau" });
            }

            return Ok(result);
        }
    }
}
