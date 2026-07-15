using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.Commands.CreateProduct;
using OrderFlow.Application.Commands.DeleteProduct;
using OrderFlow.Application.Commands.UpdateProduct;
using OrderFlow.Application.DTOs;
using OrderFlow.Application.Queries.GetProductById;
using OrderFlow.Application.Queries.GetProductsList;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderFlow.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/{version:apiVersion}/products")]
[Produces("application/json")]
[SwaggerTag("Product management — CRUD for products")]
public sealed class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Creates a new product.
    /// </summary>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Creates a product",
        Description = "Creates a new product record.")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> Create(
        [FromBody] CreateProductCommand command,
        CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Gets a product by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [SwaggerOperation(
        Summary = "Gets a product by ID",
        Description = "Returns the details of a specific product.")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id), ct);
        if (result is null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Lists all products.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Lists products",
        Description = "Returns a list of products, optionally filtered by category or active status.")]
    [ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProductDto>>> GetList(
        [FromQuery] Guid? categoryId,
        [FromQuery] bool? activeOnly,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProductsListQuery(categoryId, activeOnly), ct);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    [HttpPut("{id:guid}")]
    [SwaggerOperation(
        Summary = "Updates a product",
        Description = "Updates the details of an existing product.")]
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

    /// <summary>
    /// Deletes a product.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(
        Summary = "Deletes a product",
        Description = "Removes a product from the system.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        var success = await _mediator.Send(new DeleteProductCommand(id), ct);
        if (!success) return NotFound();
        return NoContent();
    }
}