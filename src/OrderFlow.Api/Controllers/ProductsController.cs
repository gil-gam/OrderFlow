using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.Commands.CreateProduct;
using OrderFlow.Application.Commands.DeleteProduct;
using OrderFlow.Application.Commands.UpdateProduct;
using OrderFlow.Application.DTOs;
using OrderFlow.Application.Queries.GetProductById;
using OrderFlow.Application.Queries.GetProductsList;

namespace OrderFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> Create(
        [FromBody] CreateProductCommand command,
        CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id), ct);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProductDto>>> GetList(
        [FromQuery] Guid? categoryId,
        [FromQuery] bool? activeOnly,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProductsListQuery(categoryId, activeOnly), ct);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> Update(
        Guid id,
        [FromBody] UpdateProductCommand command,
        CancellationToken ct)
    {
        if (id != command.Id) return BadRequest("Route ID does not match.");
        var result = await _mediator.Send(command, ct);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        var success = await _mediator.Send(new DeleteProductCommand(id), ct);
        if (!success) return NotFound();
        return NoContent();
    }
}