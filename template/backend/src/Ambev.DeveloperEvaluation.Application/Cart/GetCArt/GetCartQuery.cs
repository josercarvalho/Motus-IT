using Ambev.DeveloperEvaluation.Domain.Entities;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetCart;

public class GetCartQuery : IRequest<Cart>
{
    public int Id { get; set; }

    public GetCartQuery(int id)
    {
        Id = id;
    }
}
