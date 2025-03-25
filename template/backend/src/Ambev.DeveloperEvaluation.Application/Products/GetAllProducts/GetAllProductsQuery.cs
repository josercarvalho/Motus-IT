using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.GetAllProducts
{
    public class GetAllProductsQuery : IRequest<GetAllProductsResponse>
{
    public int Page { get; }
    public int Size { get; }
    public string Order { get; }
    public Dictionary<string, string> Filters { get; }

    public GetAllProductsQuery(int page, int size, string order, Dictionary<string, string> filters)
    {
        Page = page;
        Size = size;
        Order = order;
        Filters = filters;
    }
}
}