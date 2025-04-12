using Microsoft.EntityFrameworkCore;

namespace OrderApi.Entities;

public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    public required DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Order>().ToTable("orders");
        builder.Entity<Order>().HasKey(x => x.Id);
        builder.Entity<Order>().Property(x => x.ProductIds).IsRequired();
        builder.Entity<Order>().Property(x => x.TotalPrice).IsRequired();
        builder.Entity<Order>()
            .Property(x => x.ProductIds)
            .HasColumnType("uuid[]")
            .IsRequired();
    }
}