﻿using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;
using Ambev.DeveloperEvaluation.Application.Products.GetAllCategories;
using Ambev.DeveloperEvaluation.Application.Products.GetAllProducts;
using Ambev.DeveloperEvaluation.Application.Products.GetByProduct;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.GetByIdProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.UpdateProduct;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class ProductsController : ControllerBase
{
    //private readonly IProductService _productService;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductsController> _logger;
    private readonly IMediator _mediator;

    public ProductsController( IMapper mapper, ILogger<ProductsController> logger, IMediator mediator) //IProductService productService
    {
        //_productService = productService;
        _mapper = mapper;
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "Admin, Manager, Customer")]
    public async Task<ActionResult<GetAllProductsResponse>> GetAll(
        [FromQuery] int _page = 1,
        [FromQuery] int _size = 10,
        [FromQuery] string _order = "id asc")
    {
        try
        {
            // Extract filters from request object (if needed)
            var filtersExtract = Request.Query
                .Where(q => !q.Key.StartsWith("_")) // Exclude pagination and order keys
                .ToDictionary(q => q.Key, q => q.Value.ToString());

            // Send command via MediatR
            var query = new GetAllProductsQuery(_page, _size, _order, filtersExtract);
            var result = await _mediator.Send(query);


            var response = _mapper.Map<GetAllProductsResponse>(result);

            return new OkObjectResult(response);

            //return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching products.");
            return StatusCode(500, "Internal Server Error");
        }
    }


    [HttpGet("{id}")]
    [Authorize(Roles = "Admin, Manager, Customer")]
    public async Task<ActionResult<GetByIdProductResponse>> GetById(int id)
    {
        // Send command via MediatR
        var command = new GetProductCommand(id);
        var result = await _mediator.Send(command);

        if (result == null)
        {
            _logger.LogWarning("Product {id} not found", id);
            return NotFound("Product not found");
        }

        var response = _mapper.Map<GetByIdProductResponse>(result);

        return Ok(new ApiResponseWithData<GetByIdProductResponse>
        {
            Success = true,
            Message = "Product retrieved successfully",
            Data = response
        });
    }

    // return all categories 
    [HttpGet("categories")]
    [Authorize(Roles = "Admin, Manager, Customer")]
    public async Task<ActionResult<IEnumerable<string>>> GetAllCategories()
    {
        try
        {
            // Send command via MediatR
            var query = new GetAllCategoriesQuery();
            var result = await _mediator.Send(query);

            return Ok(new ApiResponseWithData<IEnumerable<string>>
            {
                Success = true,
                Message = "Get all categories successfully",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching categories.");
            return StatusCode(500, "Internal Server Error");
        }

    }

    // return products with a determined category
    [HttpGet("category/{category}")]
    [Authorize(Roles = "Admin, Manager, Customer")]
    public async Task<ActionResult<object>> GetByCategory(
    string category)
    {
        // Send command via MediatR
        var command = new GetByCategoryCommand(category);
        var result = await _mediator.Send(command);

        if (result == null)
        {
            _logger.LogWarning("Category {category} not found", category);
            return NotFound("Category not found");
        }

        return Ok(new ApiResponseWithData<object>
        {
            Success = true,
            Message = "Product by category retrieved successfully",
            Data = result
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<ActionResult<CreateProductResponse>> Create(CreateProductRequest request)
    {
        // Validating
        var validator = new CreateProductRequestValidator();
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed when creating product: {request}", request);
            return BadRequest(validationResult.Errors);
        }

        // Send command via MediatR
        var command = _mapper.Map<CreateProductCommand>(request);
        var createdProduct = await _mediator.Send(command);

        if (createdProduct == null)
        {
            _logger.LogError("MediatR returned null for CreateProductCommand");
            return StatusCode(500, "Failed to create product.");
        }

        // Map response model
        var response = _mapper.Map<CreateProductResponse>(createdProduct);

        if (response == null)
        {
            _logger.LogError("AutoMapper failed to map CreateProductResponse.");
            return StatusCode(500, "Failed to create product response.");
        }

        //return Ok(response);
        return Created(string.Empty, new ApiResponseWithData<CreateProductResponse>
        {
            Success = true,
            Message = "Product created successfully",
            Data = response
        });

    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<ActionResult<UpdateProductResponse>> Update(int id, UpdateProductRequest request)
    {
        // Validating
        var validator = new UpdateProductRequestValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed when updating product: {request}", request);
            return BadRequest(validationResult.Errors);
        }

        
        // Send command via MediatR
        var command = _mapper.Map<UpdateProductCommand>(request);
        command.Id = id;

        var updatedProduct = await _mediator.Send(command);

        if (updatedProduct == null)
        {
            _logger.LogError("MediatR returned null for UpdatedProductCommand");
            return StatusCode(500, "Failed to update product.");
        }

        // Map response model
        var response = _mapper.Map<UpdateProductResponse>(updatedProduct);

        if (response == null)
        {
            _logger.LogError("AutoMapper failed to map UpdatedProductResponse.");
            return StatusCode(500, "Failed to update product response.");
        }

        return Created(string.Empty, new ApiResponseWithData<UpdateProductResponse>
        {
            Success = true,
            Message = "Product updated successfully",
            Data = response
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        // Send command via MediatR
        var command = new DeleteProductCommand(id);
        var result = await _mediator.Send(command);

        if (!result)
        {
            _logger.LogWarning("Product {id} not found", id);
            return NotFound("Product not found");
        }

        return Created(string.Empty, new ApiResponse
        {
            Success = true,
            Message = "Product deleted successfully"
        });
    }
}
