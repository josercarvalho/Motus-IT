﻿using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetCart;

public class GetCartHandler : IRequestHandler<GetCartQuery, Cart>
{
    private readonly ICartRepository _repo;
    private readonly ILogger<GetCartHandler> _logger;
    private readonly IMapper _mapper;

    public GetCartHandler(ICartRepository repo, ILogger<GetCartHandler> logger, IMapper mapper)
    {
        _repo = repo;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Cart> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await _repo.GetCartByIdAsync(request.Id);

        if (cart == null)
        {
            _logger.LogWarning("Cart {CartId} not found", request.Id);
            return null;
        }

        return cart;

        /*
        return new GetCartResponse
        {
            UserId = cart.UserId,
            Date = cart.Date,
            Products = cart.CartProductsList.Select(product => new GetCartProductResponse
            {
                ProductId = product.ProductId,
                Quantity = product.Quantity
            }).ToList()
        };*/
    }

}
