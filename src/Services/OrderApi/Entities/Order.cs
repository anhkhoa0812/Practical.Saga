using System.ComponentModel.DataAnnotations;

namespace OrderApi.Entities;

public class Order
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public List<Guid> ProductIds { get; set; }
    [Required]
    [Range(1, Double.MaxValue)]
    public decimal TotalPrice { get; set; }
    
}