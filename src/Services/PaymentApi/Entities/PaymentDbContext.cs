using Microsoft.EntityFrameworkCore;

namespace PaymentApi.Entities;

public class PaymentDbContext(DbContextOptions<PaymentDbContext> options) : DbContext(options)
{
    public required DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Payment>().ToTable("payments");
        builder.Entity<Payment>().HasKey(x => x.Id);
        builder.Entity<Payment>().Property(x => x.TransactionId).IsRequired();
        builder.Entity<Payment>().Property(x => x.Amount).IsRequired();
        builder.Entity<Payment>().Property(x => x.PaymentDateTime).IsRequired();
        builder.Entity<Payment>().Property(x => x.OrderId).IsRequired();
    } 
}