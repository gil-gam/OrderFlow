using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Api.Requests;
using OrderFlow.Application.Commands.CreateOrder;
using OrderFlow.Application.Commands.DeleteOrder;
using OrderFlow.Application.Commands.UpdateOrder;
using OrderFlow.Application.DTOs;
using OrderFlow.Application.Queries.GetOrderById;
using OrderFlow.Application.Queries.GetOrdersList;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderFlow.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/{version:apiVersion}/[controller]")]
[Authorize]
[Produces("application/json")]
[SwaggerTag("Order management — create, list, update, and cancel orders")]
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
    [SwaggerOperation(
        Summary = "Creates an order",
        Description = "Creates an order with items, calculates totals, and returns the generated ID. Requires JWT authentication.")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateOrderRequest request,
        CancellationToken ct)
    {
        var command = new CreateOrderCommand(
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
    [SwaggerOperation(
        Summary = "Gets an order by ID",
        Description = "Returns the full order details including items, totals, and status.")]
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

    /// <summary>
    ///     Lists all orders with pagination.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Lists orders",
        Description = "Returns a paginated list of orders.")]
    [ProducesResponseType(typeof(PaginatedList<OrderListDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<OrderListDto>>> GetList(
    [FromQuery] int pageIndex = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken ct = default)
    {
        var query = new GetOrdersListQuery(pageIndex, pageSize);
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    /// <summary>
    ///     Updates an existing order.
    /// </summary>
    [HttpPut("{id:guid}")]
    [SwaggerOperation(
        Summary = "Updates an order",
        Description = "Updates an existing order. Only pending orders can be modified.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(
        Guid id,
        [FromBody] UpdateOrderCommand command,
        CancellationToken ct)
    {
        if (id != command.OrderId)
            return BadRequest("Route ID does not match command OrderId.");

        var success = await _mediator.Send(command, ct);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>
    ///     Cancels/deletes an order.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(
        Summary = "Cancels an order",
        Description = "Cancels/removes an order (soft-delete). Only pending orders can be cancelled.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        var success = await _mediator.Send(new DeleteOrderCommand(id), ct);
        if (!success) return NotFound();
        return NoContent();
    }
}