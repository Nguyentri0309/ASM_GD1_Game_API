using System.ComponentModel.DataAnnotations;

namespace GameApi_ASM_NguyenTriThinh.Models
{
    // Bang Characters: moi nhan vat thuoc mot account
    public class Character
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string Class { get; set; } = string.Empty;

        public int Level { get; set; } = 1;

        public int Hp { get; set; } = 100;

        // Luu thoi gian UTC
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Khoa ngoai, khong cho client tu truyen len
        public int AccountId { get; set; }

        public Account? Account { get; set; }
    }
}
