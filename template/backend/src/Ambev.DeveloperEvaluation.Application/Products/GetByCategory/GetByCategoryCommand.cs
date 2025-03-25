using Ambev.DeveloperEvaluation.Domain.Entities;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.GetByProduct
{
    public class GetByCategoryCommand : IRequest<IEnumerable<Product>>
    {
        public string Category { get; }

        public GetByCategoryCommand(string category)
        {
            Category = category;
        }
    }
}