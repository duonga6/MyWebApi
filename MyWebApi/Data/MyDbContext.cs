using Microsoft.EntityFrameworkCore;

namespace MyWebApi.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.HasKey(o => o.Id);

                entity.Property(o => o.DateCreate)
                .HasDefaultValueSql("getutcdate()");

            });

            modelBuilder.Entity<OrderDetails>(entity =>
            {
                entity.ToTable("OrderDetail");

                entity.HasKey(e => new
                {
                    e.OrderId,
                    e.ProductId,
                });

                entity.HasOne(od => od.Order)
                        .WithMany(o => o.OrderDetails)
                        .HasForeignKey(o => o.OrderId)
                        .HasConstraintName("FK_OrderDetail_Order")
                        .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(od => od.Product)
                        .WithMany(o => o.OrderDetails)
                        .HasForeignKey(o => o.ProductId)
                        .HasConstraintName("FK_OrderDetail_Product")
                        .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.UserName).IsUnique();
            });
        }

        #region DbSet
        public DbSet<Product> Products { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<User> Users { get; set; }
        #endregion
    }
}
