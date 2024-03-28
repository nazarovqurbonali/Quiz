using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<LevelQuestion>();
        modelBuilder.HasPostgresEnum<Topic>();
        
        modelBuilder.Entity<Question>()
            .HasMany(x=>x.Variants)
            .WithOne(x=>x.Question)
            .HasForeignKey(x=>x.QuestionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<QuestionGenerator>()
            .HasKey(qg => new { qg.QuestionId, qg.GeneratorId });

        modelBuilder.Entity<QuestionGenerator>()
            .HasOne(qg => qg.Question)
            .WithMany(s => s.QuestionGenerators)
            .HasForeignKey(qg => qg.QuestionId);

        modelBuilder.Entity<QuestionGenerator>()
            .HasOne(qg => qg.Generator)
            .WithMany(c => c.QuestionGenerators)
            .HasForeignKey(qg => qg.GeneratorId);
        
        
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Question> Questions { get; set; }
    public DbSet<Variant> Variants { get; set; }
    public DbSet<SaveTrial> SaveTrials { get; set; }
    public DbSet<Generator> Generators { get; set; }
    public DbSet<QuestionGenerator> QuestionGenerators { get; set; }
}