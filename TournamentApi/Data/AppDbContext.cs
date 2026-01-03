// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using TournamentApi.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Tournament> Tournaments { get; set; }
    public DbSet<Bracket> Brackets { get; set; }
    public DbSet<Match> Matches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Turnuva - Katılımcı (Çoka-Çok ilişki olabilir veya senin senaryonda 1-N ise düzenlenebilir)
        modelBuilder.Entity<Tournament>()
            .HasMany(t => t.Participants)
            .WithMany(u => u.Tournaments);
    }
}