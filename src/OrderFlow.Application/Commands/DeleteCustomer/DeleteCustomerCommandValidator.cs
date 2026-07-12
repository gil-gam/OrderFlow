using FluentValidation;

namespace OrderFlow.Application.Commands.DeleteCustomer;

public sealed class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Customer ID is required.");
    }
}