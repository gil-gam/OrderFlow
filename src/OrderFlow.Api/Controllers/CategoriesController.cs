using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.Commands.CreateCategory;
using OrderFlow.Application.Commands.DeleteCategory;
using OrderFlow.Application.Commands.UpdateCategory;
using OrderFlow.Application.DTOs;
using OrderFlow.Application.Queries.GetCategoriesList;
using OrderFlow.Application.Queries.GetCategoryById;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderFlow.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/{version:apiVersion}/categories")]
[Produces("application/json")]
[SwaggerTag("Category management — CRUD for product categories")]
public sealed class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Creates a new category.
    /// </summary>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Creates a category",
        Description = "Creates a new product category.")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryDto>> Create(
        [FromBody] CreateCategoryCommand command,
        CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Gets a category by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [SwaggerOperation(
        Summary = "Gets a category by ID",
        Description = "Returns the details of a specific category.")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCategoryByIdQuery(id), ct);
        if (result is null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Lists all categories.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Lists categories",
        Description = "Returns a list of categories, optionally filtered by active status.")]
    [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CategoryDto>>> GetList(
        [FromQuery] bool? activeOnly,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCategoriesListQuery(activeOnly), ct);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing category.
    /// </summary>
    [HttpPut("{id:guid}")]
    [SwaggerOperation(
        Summary = "Updates a category",
        Description = "Updates the details of an existing category.")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> Update(
        Guid id,
        [FromBody] UpdateCategoryCommand command,
        CancellationToken ct)
    {
        if (id != command.Id) return BadRequest("Route ID does not match.");
        var result = await _mediator.Send(command, ct);
        if (result is null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Deletes a category.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(
        Summary = "Deletes a category",
        Description = "Removes a category from the system.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        var success = await _mediator.Send(new DeleteCategoryCommand(id), ct);
        if (!success) return NotFound();
        return NoContent();
    }
}