using System.ComponentModel.DataAnnotations;

namespace GameApi_ASM_NguyenTriThinh.Models
{
    // Bang Accounts: mot account co nhieu nhan vat
    public class Account
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        // Luu thoi gian UTC
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Character> Characters { get; set; } = new List<Character>();
    }
}
