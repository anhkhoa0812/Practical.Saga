namespace Shared.Events;

public class OrderCreatedEvent {
    public List<ProductDto> Products { get; set; }
}