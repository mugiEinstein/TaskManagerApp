using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using TaskManagerApp.Models;

namespace TaskManagerApp.Data
{
    /// <summary>
    /// EF Core DbContext，使用 SQLite 存储
    /// </summary>
    public class AppDbContext : DbContext
    {
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // 默认将 SQLite 数据库文件放在 LocalApplicationData\TaskManagerApp\tasks.db
                string folder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "TaskManagerApp");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                string dbPath = Path.Combine(folder, "tasks.db");

                optionsBuilder.UseSqlite($"Data Source={dbPath}");

                // 可选：调试时输出 SQL 日志到 Debug 窗口
                optionsBuilder.LogTo(msg => System.Diagnostics.Debug.WriteLine(msg),
                                     Microsoft.Extensions.Logging.LogLevel.Information);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Category 配置
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            // TaskItem 配置
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Status).HasDefaultValue(TaskState.Pending);
                entity.Property(e => e.Priority).HasDefaultValue(PriorityLevel.Medium);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(e => e.Category)
                      .WithMany()
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.HasIndex(e => e.DueDate);
            });
        }
    }
}