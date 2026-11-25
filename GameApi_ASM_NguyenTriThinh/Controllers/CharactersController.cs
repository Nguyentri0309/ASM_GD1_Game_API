using System.Security.Claims;
using GameApi_ASM_NguyenTriThinh.DTOs;
using GameApi_ASM_NguyenTriThinh.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameApi_ASM_NguyenTriThinh.Controllers
{
    [ApiController]
    [Route("api/characters")]
    [Authorize]
    [Tags("Characters")]
    [ApiExplorerSettings(GroupName = "Characters")]
    public class CharactersController : ControllerBase
    {
        private readonly CharacterService _characterService;

        public CharactersController(CharacterService characterService)
        {
            _characterService = characterService;
        }

        // GET api/characters/mine - danh sach nhan vat cua account hien tai
        [HttpGet("mine")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMine()
        {
            var authError = TryGetAccountId(out int accountId);
            if (authError != null)
            {
                return authError;
            }

            return Ok(await _characterService.GetMineAsync(accountId));
        }

        // POST api/characters - can JWT
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] CreateCharacterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authError = TryGetAccountId(out int accountId);
            if (authError != null)
            {
                return authError;
            }

            var result = await _characterService.CreateAsync(accountId, request);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        // PUT api/characters/{id} - chi sua duoc nhan vat cua chinh minh
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCharacterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authError = TryGetAccountId(out int accountId);
            if (authError != null)
            {
                return authError;
            }

            var result = await _characterService.UpdateAsync(accountId, id, request);
            if (result == null)
            {
                return NotFound(new { message = "Khong tim thay nhan vat" });
            }

            return Ok(result);
        }

        // AccountId luon lay tu claim cua JWT, khong doc tu body
        private IActionResult? TryGetAccountId(out int accountId)
        {
            string? value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(value, out accountId))
            {
                return null;
            }

            return Unauthorized(new { message = "Token khong hop le" });
        }
    }
}
