﻿using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, Sale>
{
    private readonly ISaleRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;
    private readonly IMediator _mediator;

    public CreateSaleHandler(ISaleRepository repo, IMapper mapper, ILogger<CreateSaleHandler> logger, IMediator mediator)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<Sale> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        // Buscar o Cart pelo ID
        var cartQuery = new GetCartQuery(command.CartId);
        var cart = await _mediator.Send(cartQuery);

        if (cart == null || cart.CartProductsList == null || !cart.CartProductsList.Any())
        {
            _logger.LogError("Cart is empty or invalid: {CartId}", command.CartId);
            return null;
        }

        var sale = _mapper.Map<Sale>(command);

        if (command.Items == null)
        {
            sale.Items = new List<SaleItem>();
            _logger.LogWarning("saleItems null, new saleItem list was created at {sale}", sale);
        }

        var saleItems = new List<SaleItem>();

        foreach (var cartProduct in cart.CartProductsList)
        {
            var productQuery = new GetProductCommand(cartProduct.ProductId);
            var product = await _mediator.Send(productQuery);

            if (product == null)
            {
                _logger.LogError("Product {ProductId} not found", cartProduct.ProductId);
                return null; 
            }

            var cartCart =  _mapper.Map<Cart>(cart);

            var saleItem = new SaleItem
            {
                ProductId = product.Id,
                ProductItem = product,
                ProductName = product.Title,
                CartItem = cartCart,
                CartItemId = command.CartId,
                UnitPrice = product.Price,
                Quantity = cartProduct.Quantity,
                IsCancelled = false
            };

            saleItem.CalculateDiscountAndValidate();
            saleItem.Total = saleItem.UnitPrice * saleItem.Quantity * (1 - saleItem.Discount);

            saleItems.Add(saleItem);
        }

        sale.Items ??= new List<SaleItem>();
        sale.Items.AddRange(saleItems);

        sale.TotalAmount = sale.Items.Sum(item => item.Total);
        sale.CustomerId = cart.UserId;


        var createdSale = await _repo.CreateSale(sale);
        _logger.LogInformation("Created Sale {Sale}", createdSale);

        return createdSale;
    }
}
