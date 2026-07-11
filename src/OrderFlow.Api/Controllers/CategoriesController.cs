using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.Commands.CreateCategory;
using OrderFlow.Application.Commands.DeleteCategory;
using OrderFlow.Application.Commands.UpdateCategory;
using OrderFlow.Application.DTOs;
using OrderFlow.Application.Queries.GetCategoriesList;
using OrderFlow.Application.Queries.GetCategoryById;

namespace OrderFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/categories")]
public sealed class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryDto>> Create(
        [FromBody] CreateCategoryCommand command,
        CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCategoryByIdQuery(id), ct);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CategoryDto>>> GetList(
        [FromQuery] bool? activeOnly,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCategoriesListQuery(activeOnly), ct);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
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

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        var success = await _mediator.Send(new DeleteCategoryCommand(id), ct);
        if (!success) return NotFound();
        return NoContent();
    }
}