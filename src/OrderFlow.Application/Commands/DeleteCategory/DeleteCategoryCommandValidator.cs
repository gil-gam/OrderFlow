using FluentValidation;

namespace OrderFlow.Application.Commands.DeleteCategory;

public sealed class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Category ID is required.");
    }
}
