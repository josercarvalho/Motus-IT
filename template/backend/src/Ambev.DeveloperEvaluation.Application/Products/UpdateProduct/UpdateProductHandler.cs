using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Products.UpdateProduct
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Product>
    {
        private readonly IProductRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateProductHandler> _logger;

        public UpdateProductHandler(IProductRepository repo, IMapper mapper, ILogger<UpdateProductHandler> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Product> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {

            var product = await _repo.GetProductById(command.Id);

            if (product == null)
            {
                _logger.LogWarning("Product {ProductId} not found", command.Id);
                return null;
            }

            // Update product properties
            product.Title = command.Title;
            product.Description = command.Description;
            product.Price = command.Price;
            product.Category = command.Category;
            product.Image = command.Image;
            product.Rating = command.Rating;

            await _repo.UpdateProduct(product);

            _logger.LogInformation("Product {ProductId} updated successfully", command.Id);

            return product;
        }

    }
}