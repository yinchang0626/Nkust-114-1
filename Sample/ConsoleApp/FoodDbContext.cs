using Microsoft.EntityFrameworkCore;

namespace ConsoleApp
{
    /// <summary>
    /// 食品資料庫上下文類別
    /// </summary>
    public class FoodDbContext : DbContext
    {
        /// <summary>
        /// 食品資訊資料表
        /// </summary>
        public DbSet<FoodInfoEntity> FoodInfos { get; set; }

        /// <summary>
        /// 無參數建構函式
        /// </summary>
        public FoodDbContext()
        {
        }

        /// <summary>
        /// 帶選項的建構函式
        /// </summary>
        /// <param name="options">DbContext 選項</param>
        public FoodDbContext(DbContextOptions<FoodDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// 設定資料庫連線
        /// </summary>
        /// <param name="optionsBuilder">選項建構器</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // 使用 LocalDB 連線字串
                optionsBuilder.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=FoodNutritionDb;Trusted_Connection=True;MultipleActiveResultSets=true"
                );
            }
        }

        /// <summary>
        /// 設定模型
        /// </summary>
        /// <param name="modelBuilder">模型建構器</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 設定索引以提升查詢效能
            modelBuilder.Entity<FoodInfoEntity>()
                .HasIndex(f => f.IntegratedNumber)
                .HasDatabaseName("IX_FoodInfos_IntegratedNumber");

            modelBuilder.Entity<FoodInfoEntity>()
                .HasIndex(f => f.SampleName)
                .HasDatabaseName("IX_FoodInfos_SampleName");

            modelBuilder.Entity<FoodInfoEntity>()
                .HasIndex(f => f.AnalysisCategory)
                .HasDatabaseName("IX_FoodInfos_AnalysisCategory");
        }
    }
}
