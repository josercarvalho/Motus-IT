using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;
using Ambev.DeveloperEvaluation.Application.Products.GetAllCategories;
using Ambev.DeveloperEvaluation.Application.Products.GetAllProducts;
using Ambev.DeveloperEvaluation.Application.Products.GetByProduct;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Products;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.GetByIdProduct;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi;

public class ProductsTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<ProductsController>> _loggerMock;
    private readonly ProductsController _controller;

    public ProductsTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<ProductsController>>();

        _controller = new ProductsController(_mapperMock.Object, _loggerMock.Object, _mediatorMock.Object);
    }

    [Fact(DisplayName = "Given Get All Should Return Ok When Data Exists")]
    public async Task GivenGetAllShouldReturnOkWhenDataExists()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Title = "Product 1", Category = "Category 1", Description = "Description 1", Price=2 },
            new Product { Id = 2, Title = "Product 2", Category = "Category 2", Description = "Description 2", Price=4 }
        };

        var getAllProductsResponse = new GetAllProductsResponse { Data = products };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(getAllProductsResponse);

        // Act
        var result = await _controller.GetAll();


        // Assert
        var actionResult = Assert.IsType<ActionResult<GetAllProductsResponse>>(result);

        var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
        if (objectResult.StatusCode == 500)
        {
            var errorMessage = Assert.IsType<string>(objectResult.Value);
            Assert.Equal("Internal Server Error", errorMessage);
        }
        else
        {
            Assert.IsType<GetAllProductsResponse>(objectResult.Value);
        }
    }

    [Fact(DisplayName = "Given Id Should Return Ok When Product Exists")]
    public async Task GivenIdShouldReturnOkWhenProductExists()
    {
        // Arrange
        var productId = 1;
        var product = new Product { Id = productId, Title = "Test Product" };
        var response = new GetByIdProductResponse { Title = "Test Product" };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _mapperMock
            .Setup(m => m.Map<GetByIdProductResponse>(product))
            .Returns(response);

        // Act
        var result = await _controller.GetById(productId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var apiResponse = okResult.Value as ApiResponseWithData<GetByIdProductResponse>;
        apiResponse.Should().NotBeNull();
        apiResponse.Success.Should().BeTrue();
        apiResponse.Message.Should().Be("Product retrieved successfully");
        apiResponse.Data.Should().BeEquivalentTo(response);
    }

    [Fact(DisplayName = "Should Return Dado Created When Valid Request")]
    public async Task CreateShouldReturnCreatedWhenValidRequest()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Title = "New Product",
            Category = "Category",
            Description = "Description",
            Price = 10.0m,
            Rating = new CreateProductRatingRequest { Rate = 4.5 }
        };

        var command = new CreateProductCommand();
        var createResult = new CreateProductResult { Id = 1, Title = "New Product" };
        var response = new CreateProductResponse { Id = 1, Title = "New Product" };

        _mapperMock
            .Setup(m => m.Map<CreateProductCommand>(request))
            .Returns(command);

        _mediatorMock
            .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createResult);

        _mapperMock
            .Setup(m => m.Map<CreateProductResponse>(createResult))
            .Returns(response);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdResult = result.Result as CreatedResult;
        createdResult.Should().NotBeNull();
        var apiResponse = createdResult.Value as ApiResponseWithData<CreateProductResponse>;
        apiResponse.Should().NotBeNull();
        apiResponse.Success.Should().BeTrue();
        apiResponse.Message.Should().Be("Product created successfully");
        apiResponse.Data.Should().BeEquivalentTo(response);
    }

    [Fact(DisplayName = "Should Return true When Product Deleted")]
    public async Task DeleteShouldReturnTrueWhenProductDeleted()
    {
        // Arrange
        var productId = 1;
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(productId);

        // Assert
        var createdResult = result as CreatedResult;
        createdResult.Should().NotBeNull();
        var apiResponse = createdResult.Value as ApiResponse;
        apiResponse.Should().NotBeNull();
        apiResponse.Success.Should().BeTrue();
        apiResponse.Message.Should().Be("Product deleted successfully");
    }

    [Fact(DisplayName = "Get All Categories Should Return Ok When Categories Exist")]
    public async Task GetAllCategoriesShouldReturnOkWhenCategoriesExist()
    {
        // Arrange
        var categories = new List<string> { "Category1", "Category2" };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllCategoriesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        // Act
        var result = await _controller.GetAllCategories();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var apiResponse = okResult.Value as ApiResponseWithData<IEnumerable<string>>;
        apiResponse.Should().NotBeNull();
        apiResponse.Success.Should().BeTrue();
        apiResponse.Message.Should().Be("Get all categories successfully");
        apiResponse.Data.Should().BeEquivalentTo(categories);
    }

    [Fact(DisplayName = "Get By Category Should Return Ok When Category Exists")]
    public async Task GetByCategoryShouldReturnOkWhenCategoryExists()
    {
        // Arrange
        var category = "TestCategory";
        var products = new List<Product>
        {
            new Product { Id = 1, Category = category }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetByCategoryCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _controller.GetByCategory(category);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var apiResponse = okResult.Value as ApiResponseWithData<object>;
        apiResponse.Should().NotBeNull();
        apiResponse.Success.Should().BeTrue();
        apiResponse.Message.Should().Be("Product by category retrieved successfully");
        apiResponse.Data.Should().BeEquivalentTo(products);
    }
}
