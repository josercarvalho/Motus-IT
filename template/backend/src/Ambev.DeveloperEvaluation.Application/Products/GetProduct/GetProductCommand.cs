using Ambev.DeveloperEvaluation.Domain.Entities;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProduct
{
    public class GetProductCommand : IRequest<Product>
    {
        public int ProductId { get; set; }

        public GetProductCommand(int productId)
        {
            ProductId = productId;
        }
    }
}