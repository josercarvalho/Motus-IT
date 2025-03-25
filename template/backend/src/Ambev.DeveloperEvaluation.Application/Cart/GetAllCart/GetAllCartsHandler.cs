using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetAllCarts;

public class GetAllCartsHandler : IRequestHandler<GetAllCartsQuery, GetAllCartsPagedResponse<GetAllCartsResponse>>
{
    private readonly ICartRepository _repo;
    private readonly ILogger<GetAllCartsHandler> _logger;

    public GetAllCartsHandler(ILogger<GetAllCartsHandler> logger, ICartRepository repo)
    {
        _logger = logger;
        _repo = repo;
    }

    public async Task<GetAllCartsPagedResponse<GetAllCartsResponse>> Handle(GetAllCartsQuery request, CancellationToken cancellationToken)
    {
        var (carts, totalItems) = await _repo.GetFilteredAndOrderedCartsAsync(
            request.Page, request.Size, request.Order, request.Filters);

        var cartDtos = carts.Select(cart => new GetAllCartsResponse
        {
            Id = cart.Id,
            UserId = cart.UserId,
            Date = cart.Date,
            CartProductsList = cart.CartProductsList.Select(product => new GetAllCartsProductResponse
            {
                ProductId = product.ProductId,
                Quantity = product.Quantity
            }).ToList()
        }).ToList();

        return new GetAllCartsPagedResponse<GetAllCartsResponse>(cartDtos, totalItems, request.Page, request.Size);
    }
}
