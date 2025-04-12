using System.ComponentModel.DataAnnotations;
using OrderApi.Commands;

namespace OrderApi.Dto;

public class OrderDto {
    public Guid Id { get; set; }
    public decimal TotalPrice { get; set; }
}
