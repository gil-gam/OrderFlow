using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Api.Requests;
using OrderFlow.Application.Commands.CreateOrder;
using OrderFlow.Application.DTOs;
using OrderFlow.Application.Queries.GetOrderById;

namespace OrderFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] 
[Produces("application/json")]
public sealed class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
        => _mediator = mediator;

    /// <summary>
    ///     Create a new order
    /// </summary>
    /// <param name="request">Order request</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>order created ID</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateOrderRequest request,
        CancellationToken ct)
    {
        var command = new CreateOrderCommand(
            request.CustomerId,
            request.Street,
            request.City,
            request.State,
            request.ZipCode,
            request.Country,
            request.Items.Select(i => new CreateOrderItemDto(
                i.ProductId,
                i.ProductName,
                i.Quantity,
                i.UnitPrice,
                i.Currency ?? "USD")).ToList());

        var orderId = await _mediator.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id = orderId }, orderId);
    }

    /// <summary>
    ///     Get an order by ID
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Order details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDetailDto>> GetById(
        Guid id,
        CancellationToken ct)
    {
        var query = new GetOrderByIdQuery(id);
        var result = await _mediator.Send(query, ct);

        if (result is null)
            return NotFound(new { message = $"Order {id} not found." });

        return Ok(result);
    }
}