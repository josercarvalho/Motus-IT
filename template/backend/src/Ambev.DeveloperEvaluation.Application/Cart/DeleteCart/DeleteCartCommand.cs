using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;

public class DeleteCartCommand : IRequest<bool>
{
    public int CartId { get; }

    public DeleteCartCommand(int cartId)
    {
        CartId = cartId;
    }
}
