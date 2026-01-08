using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<FoodInfoEntity> FoodInfoEntities { get; set; }
        public DbSet<AnalysisItemEntity> AnalysisItemEntities { get; set; }
        public DbSet<FoodEntity> FoodEntities { get; set; }
        public DbSet<FoodAnalysisItemEntity> FoodAnalysisItemEntities { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ?? FoodAnalysisItemEntity ?????
            modelBuilder.Entity<FoodAnalysisItemEntity>()
                .HasOne(fai => fai.Food)
                .WithMany(f => f.FoodAnalysisItems)
                .HasForeignKey(fai => fai.FoodId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FoodAnalysisItemEntity>()
                .HasOne(fai => fai.AnalysisItem)
                .WithMany(ai => ai.FoodAnalysisItems)
                .HasForeignKey(fai => fai.AnalysisItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // ?????????????????????????????
            modelBuilder.Entity<FoodAnalysisItemEntity>()
                .HasIndex(fai => new { fai.FoodId, fai.AnalysisItemId })
                .IsUnique()
                .HasDatabaseName("IX_FoodAnalysisItems_FoodId_AnalysisItemId");

            // ?????????????
            modelBuilder.Entity<FoodEntity>()
                .HasIndex(f => f.IntegratedNumber)
                .IsUnique()
                .HasDatabaseName("IX_Foods_IntegratedNumber");

            modelBuilder.Entity<FoodEntity>()
                .HasIndex(f => f.SampleName)
                .HasDatabaseName("IX_Foods_SampleName");

            modelBuilder.Entity<AnalysisItemEntity>()
                .HasIndex(ai => new { ai.DataCategory, ai.Name })
                .IsUnique()
                .HasDatabaseName("IX_AnalysisItems_DataCategory_Name");

            modelBuilder.Entity<FoodAnalysisItemEntity>()
                .HasIndex(fai => fai.CreatedAt)
                .HasDatabaseName("IX_FoodAnalysisItems_CreatedAt");

            modelBuilder.Entity<FoodAnalysisItemEntity>()
                .HasIndex(fai => fai.IsActive)
                .HasDatabaseName("IX_FoodAnalysisItems_IsActive");
        }
    }
}