using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Models;
using ProductApi.Repositories;

namespace ProductApi.Endpoints;

public class ProductEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1");
        group.MapGet("products/{id}", GetProduct).WithName(nameof(GetProduct));
        group.MapPost("products", CreateProduct).WithName(nameof(CreateProduct));
    }
    
    public async Task<IResult> GetProduct(IProductRepository repository, Guid id)
    {
        var product = await repository.GetProduct(id);
        return Results.Ok(product);
    }
    public async Task<IResult> CreateProduct(IProductRepository repository, [FromBody] Product product)
    {
        if (product == null)
            return Results.Json("Product cannot be null", statusCode: StatusCodes.Status400BadRequest);
        await repository.CreateProduct(product);
        return Results.Json("Product created successfully", statusCode: StatusCodes.Status201Created);
    }
}