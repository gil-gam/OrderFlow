using FluentValidation;

namespace OrderFlow.Application.Commands.DeleteOrder;

public sealed class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
{
    public DeleteOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required.");
    }
}