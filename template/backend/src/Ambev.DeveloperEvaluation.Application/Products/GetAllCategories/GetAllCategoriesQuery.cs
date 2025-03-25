using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.GetAllCategories
{
    public class GetAllCategoriesQuery : IRequest<IEnumerable<string>>
    {

    }
}