using FluentValidation;

namespace OrderFlow.Application.Commands.DeleteProduct;

public sealed class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Product ID is required.");
    }
}