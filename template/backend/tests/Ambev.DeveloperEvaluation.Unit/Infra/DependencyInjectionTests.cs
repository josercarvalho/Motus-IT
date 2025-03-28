﻿using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.IoC;
using Ambev.DeveloperEvaluation.ORM;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Infra;

public class DependencyInjectionTests
{
    private readonly IServiceProvider _serviceProvider;

    public DependencyInjectionTests()
    {
        var services = new ServiceCollection();
        var builder = WebApplication.CreateBuilder();

        // Register Database Context with In-Memory Provider (for testing)
        builder.Services.AddDbContext<DefaultContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        builder.RegisterDependencies(); // Register all IoC dependencies
        _serviceProvider = builder.Services.BuildServiceProvider();
    }


    [Fact(DisplayName = "Should Resolve Cart Repository")]
    public void ShouldResolveCartRepository()
    {
        var repository = _serviceProvider.GetService<ICartRepository>();
        Assert.NotNull(repository); // Test passes if CartRepository is correctly registered
    }

    [Fact(DisplayName = "Should Resolve Sale Repository")]
    public void ShouldResolveSaleRepository()
    {
        var repository = _serviceProvider.GetService<ISaleRepository>();
        Assert.NotNull(repository); // Test passes if SaleRepository is correctly registered
    }

    [Fact(DisplayName = "Should Resolve Product Repository")]
    public void ShouldResolveProductRepository()
    {
        var repository = _serviceProvider.GetService<IProductRepository>();
        Assert.NotNull(repository); // Test passes if ProductRepository is correctly registered
    }
}
