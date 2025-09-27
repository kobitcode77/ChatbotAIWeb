using ChatbotAI_BE.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatbotAI_BE.Data
{
    public class ChatDBContext : DbContext
    {
        public ChatDBContext(DbContextOptions<ChatDBContext> options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<AppUser> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AppUser>()
                .HasIndex(u => new { u.Provider, u.ProviderId })
                .IsUnique();
        }
    }
}


