﻿using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetAllSales;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetAllSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSaleById;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        public readonly IMapper _mapper;
        public readonly ILogger<SalesController> _logger;
        private readonly IMediator _mediator;

        public SalesController(IMapper mapper, ILogger<SalesController> logger, IMediator mediator)
        {
            _mapper = mapper;
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<ActionResult> CreateSale(CreateSaleRequest saleDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is not valid when creating {saleDto}", saleDto);
                return BadRequest();
            }

            try
            {
            
                var sale = _mapper.Map<Sale>(saleDto);
                //var createdSale = await _service.CreateSale(sale);
                var command = _mapper.Map<CreateSaleCommand>(sale);
                var createdSale = await _mediator.Send(command);

                var responseMap = _mapper.Map<CreateSaleResponse>(createdSale);

                return Created(string.Empty, new ApiResponseWithData<CreateSaleResponse>
                {
                    Success = true,
                    Message = "Sale created successfully",
                    Data = responseMap
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating sales.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<ActionResult<List<GetAllSalesResponse>>> GetAllSales()
        {
            //var sales = await _service.GetAllSales();
            var query = new GetAllSalesQuery();
            var result = await _mediator.Send(query);

            var response = _mapper.Map<List<GetAllSalesResponse>>(result);

            return Ok(new ApiResponseWithData<List<GetAllSalesResponse>>
            {
                Success = true,
                Message = "Get all sales successfully",
                Data = response
            });
        }

        [HttpGet("{saleNumber}")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<ActionResult<GetSaleByIdResponse>> GetSaleById(int saleNumber)
        {
            //var sale = await _service.GetSaleById(saleNumber);
            var query = new GetSaleQuery(saleNumber);
            var sale = await _mediator.Send(query);

            if (sale == null) return NotFound();

            var responseMap = _mapper.Map<GetSaleByIdResponse>(sale);

            return Ok(new ApiResponseWithData<GetSaleByIdResponse>
            {
                Success = true,
                Message = "Sale retrieved successfully",
                Data = responseMap
            });
        }

        [HttpPut("{saleNumber}")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<ActionResult> UpdateSale(int saleNumber, UpdateSaleRequest saleDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is not valid when creating {saleDto}", saleDto);
                return BadRequest();
            }

            try
            {
            
                var command = _mapper.Map<UpdateSaleCommand>(saleDto);
                command.SaleNumber = saleNumber;
                var createdSale = await _mediator.Send(command);

                var response = _mapper.Map<UpdateSaleResponse>(createdSale);

                return Created(string.Empty, new ApiResponseWithData<UpdateSaleResponse>
                {
                    Success = true,
                    Message = "Sale updated successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating sales.");
                return StatusCode(500, "Internal Server Error");
            }


        }

        [HttpDelete("{saleNumber}")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<ActionResult> DeleteSale(int saleNumber)
        {
            try
            {
                var query = new GetSaleQuery(saleNumber);
                var sale = await _mediator.Send(query);

                if (sale == null)
                {
                    _logger.LogWarning("Sale {saleNumber} wasn't found", saleNumber);
                    return NotFound();
                }

                var command = new DeleteSaleCommand(saleNumber);
                await _mediator.Send(command);

                return Created(string.Empty, new ApiResponse
                {
                    Success = true,
                    Message = "Sale deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting sales.");
                return StatusCode(500, "Internal Server Error");
            }

        }
    }
}
