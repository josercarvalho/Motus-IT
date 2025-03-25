using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

public class UpdateCartValidator : AbstractValidator<UpdateCartCommand>
{
    public UpdateCartValidator()
    {
        RuleFor(cart => cart.CartProductsList).NotEmpty();
        RuleFor(cart => cart.UserId).GreaterThan(-1);
    }
}
