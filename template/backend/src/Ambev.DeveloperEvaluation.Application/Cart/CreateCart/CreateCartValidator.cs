using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Carts.CreateCart;

public class CreateCartValidator : AbstractValidator<CreateCartCommand>
{
    public CreateCartValidator()
    {
        RuleFor(cart => cart.CartProductsList).NotEmpty();
        RuleFor(cart => cart.UserId).GreaterThan(-1);
    }
}
