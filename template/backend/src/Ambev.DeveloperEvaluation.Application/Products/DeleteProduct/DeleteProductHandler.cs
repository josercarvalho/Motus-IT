using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Products.DeleteProduct
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<DeleteProductHandler> _logger;

        public DeleteProductHandler(IProductRepository productRepository, ILogger<DeleteProductHandler> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductById(request.ProductId);

            if (product == null)
            {
                _logger.LogWarning("Product {ProductId} not found", request.ProductId);
                return false;
            }

            await _productRepository.DeleteProduct(product.Id);

            _logger.LogInformation("Product {ProductId} deleted successfully", request.ProductId);
            return true;
        }
    }
}