using GameApi_ASM_NguyenTriThinh.Models;
using Microsoft.EntityFrameworkCore;

namespace GameApi_ASM_NguyenTriThinh.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Character> Characters => Set<Character>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Username).IsRequired().HasMaxLength(50);
                // Username la duy nhat
                entity.HasIndex(a => a.Username).IsUnique();
                entity.Property(a => a.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(a => a.CreatedAt).IsRequired().HasColumnType("datetime");
            });

            modelBuilder.Entity<Character>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(50);
                entity.Property(c => c.Class).IsRequired().HasMaxLength(30);
                entity.Property(c => c.Level).HasDefaultValue(1);
                entity.Property(c => c.Hp).HasDefaultValue(100);
                entity.Property(c => c.CreatedAt).IsRequired().HasColumnType("datetime");

                // 1 account - nhieu nhan vat; xoa account thi xoa cac nhan vat
                entity.HasOne(c => c.Account)
                      .WithMany(a => a.Characters)
                      .HasForeignKey(c => c.AccountId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
