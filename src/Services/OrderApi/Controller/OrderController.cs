using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Commands;

namespace OrderApi.Controller;

public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;
    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("api/orders")]
    public async Task<ActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {   
        return Ok(await _mediator.Send(command));
    }
    
}