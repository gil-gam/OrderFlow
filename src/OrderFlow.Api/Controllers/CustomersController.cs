using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.Commands.CreateCustomer;
using OrderFlow.Application.Commands.DeleteCustomer;
using OrderFlow.Application.Commands.UpdateCustomer;
using OrderFlow.Application.DTOs;
using OrderFlow.Application.Queries.GetCustomerById;
using OrderFlow.Application.Queries.GetCustomersList;

namespace OrderFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/customers")]
public sealed class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerDto>> Create(
        [FromBody] CreateCustomerCommand command,
        CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCustomerByIdQuery(id), ct);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CustomerDto>>> GetList(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCustomersListQuery(), ct);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> Update(
        Guid id,
        [FromBody] UpdateCustomerCommand command,
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
        var success = await _mediator.Send(new DeleteCustomerCommand(id), ct);
        if (!success) return NotFound();
        return NoContent();
    }
}