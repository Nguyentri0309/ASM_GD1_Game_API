using GameApi_ASM_NguyenTriThinh.Data;
using GameApi_ASM_NguyenTriThinh.DTOs;
using GameApi_ASM_NguyenTriThinh.Models;
using Microsoft.EntityFrameworkCore;

namespace GameApi_ASM_NguyenTriThinh.Services
{
    public class CharacterService
    {
        private readonly AppDbContext _context;

        public CharacterService(AppDbContext context)
        {
            _context = context;
        }

        // Tao nhan vat: AccountId do controller lay tu JWT
        public async Task<CharacterResponse> CreateAsync(int accountId, CreateCharacterRequest request)
        {
            var character = new Character
            {
                Name = request.Name,
                Class = request.Class,
                Level = 1,
                Hp = 100,
                AccountId = accountId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Characters.Add(character);
            await _context.SaveChangesAsync();

            return ToResponse(character);
        }

        // Sua nhan vat: chi sua duoc nhan vat cua chinh account do
        public async Task<CharacterResponse?> UpdateAsync(int accountId, int characterId, UpdateCharacterRequest request)
        {
            var character = await _context.Characters
                .FirstOrDefaultAsync(c => c.Id == characterId && c.AccountId == accountId);

            if (character == null)
            {
                return null;
            }

            character.Name = request.Name;
            character.Class = request.Class;
            character.Level = request.Level!.Value;
            character.Hp = request.Hp!.Value;

            await _context.SaveChangesAsync();

            return ToResponse(character);
        }

        // Lay danh sach nhan vat cua account hien tai (JWT)
        public async Task<List<CharacterResponse>> GetMineAsync(int accountId)
        {
            return await _context.Characters
                .Where(c => c.AccountId == accountId)
                .OrderBy(c => c.Id)
                .Select(c => new CharacterResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Class = c.Class,
                    Level = c.Level,
                    Hp = c.Hp,
                    AccountId = c.AccountId
                })
                .ToListAsync();
        }

        private static CharacterResponse ToResponse(Character character)
        {
            return new CharacterResponse
            {
                Id = character.Id,
                Name = character.Name,
                Class = character.Class,
                Level = character.Level,
                Hp = character.Hp,
                AccountId = character.AccountId
            };
        }
    }
}
