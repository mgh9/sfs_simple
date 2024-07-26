using SFS.Application.Abstractions.Services;
using SFS.Domain.Dtos;
using Swashbuckle.AspNetCore.Annotations;

namespace SFS.Api.Endpoints;

internal static class ProductEndpoints
{
    internal static void MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/products");

        // add a product
        group.MapPost("/",
        [SwaggerOperation(Summary = "Add a Product", Description = "Add a `Product` to the inventory", Tags = ["Products"])]
        [SwaggerResponse(StatusCodes.Status201Created, "returns the created product Id", type: typeof(int))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Product info")]
        async (ProductDto product, IProductService productService , CancellationToken cancellationToken) =>
        {
            try
            {
                var result = await productService.AddAsync(product,cancellationToken);
                return Results.Created($"/{result}", product);
            }
            catch(ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        })
            .WithName("AddProduct")
            .WithOpenApi();

        group.MapPost("/{id}/increase-inventory/{amount}",
        [SwaggerOperation(Summary = "Increase Inventory", Description = "Increase a `product` in the inventory", Tags = ["Products"])]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found")]
        async (int id, int amount, IProductService productService, CancellationToken cancellationToken) =>
        {
            try
            {
                await productService.IncreaseInventoryAsync(id, amount, cancellationToken);
                return Results.Ok();
            }
            catch(ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(ex.Message);
            }
        })
            .WithName("IncreaseInventory")
            .WithOpenApi();

        // get a product by Id, considering discount
        group.MapGet("/{id}",
        [SwaggerOperation(Summary = "Get a product", Description = "Get a `product` by Id, considering its discount", Tags = ["Products"])]
        [SwaggerResponse(StatusCodes.Status200OK, "returns a `product`", type: typeof(ProductDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found")]
        async (int id, IProductService productService , CancellationToken cancellationToken) =>
        {
            try
            {
                var theProduct = await productService.GetByIdAsync(id, cancellationToken);
                if(theProduct is null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(theProduct);
            }
            catch (Exception ex)
            {
                throw;
            }
        })
            .WithName("GetProductById")
            .WithOpenApi();

        // buy a product
        // as far as we don't have an Auth system (and CurrentUser), so I'm passing `buyerUserId` as a parameter to this endpoint
        // but in most cases it's recommended to use the CurrentUser Id in the back-end side and not passing from the client
        group.MapPost("/{id}/buy",
        [SwaggerOperation(Summary = "Buy a product", Description = "Buy a `product`, considering inventory stock", Tags = ["Products"])]
        [SwaggerResponse(StatusCodes.Status200OK, "returns the `Order` Id", type: typeof(int))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Insufficient inventory")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User (Buyer) not found")]
        async (int id, int buyerId, IProductService productService, CancellationToken cancellationToken) =>
        {
            try
            {
                var orderId = await productService.BuyAsync(id,buyerId, cancellationToken);
                return Results.Ok(orderId);
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(ex.Message);
            }
            catch(InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        })
            .WithName("BuyProduct")
            .WithOpenApi();
    }
}

