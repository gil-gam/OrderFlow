using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.Commands.CreateCustomer;
using OrderFlow.Application.Commands.DeleteCustomer;
using OrderFlow.Application.Commands.UpdateCustomer;
using OrderFlow.Application.DTOs;
using OrderFlow.Application.Queries.GetCustomerById;
using OrderFlow.Application.Queries.GetCustomersList;
using Swashbuckle.AspNetCore.Annotations;

namespace OrderFlow.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/{version:apiVersion}/customers")]
[Produces("application/json")]
[SwaggerTag("Customer management — CRUD for customers")]
public sealed class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Creates a new customer.
    /// </summary>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Creates a customer",
        Description = "Creates a new customer record.")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerDto>> Create(
        [FromBody] CreateCustomerCommand command,
        CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Gets a customer by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [SwaggerOperation(
        Summary = "Gets a customer by ID",
        Description = "Returns the details of a specific customer.")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCustomerByIdQuery(id), ct);
        if (result is null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Lists all customers.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Lists customers",
        Description = "Returns a list of all registered customers.")]
    [ProducesResponseType(typeof(List<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CustomerDto>>> GetList(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCustomersListQuery(), ct);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing customer.
    /// </summary>
    [HttpPut("{id:guid}")]
    [SwaggerOperation(
        Summary = "Updates a customer",
        Description = "Updates the details of an existing customer.")]
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

    /// <summary>
    /// Deletes a customer.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(
        Summary = "Deletes a customer",
        Description = "Removes a customer from the system.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        var success = await _mediator.Send(new DeleteCustomerCommand(id), ct);
        if (!success) return NotFound();
        return NoContent();
    }
}