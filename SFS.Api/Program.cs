using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SFS.Data;
using SFS.Domain.Models;
using Swashbuckle.AspNetCore.Annotations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddMemoryCache();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.EnableAnnotations();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// add a product
app.MapPost("/api/products", async (Product product, AppDbContext context, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(product.Title) || product.Title.Length > 40)
    {
        return Results.BadRequest("Product title must be less than 40 characters");
    }

    if (await context.Products.CountAsync(x => x.Title == product.Title, cancellationToken: cancellationToken) > 0)
    {
        return Results.BadRequest("Product title must be unique.");
    }

    await context.Products.AddAsync(product, cancellationToken);
    await context.SaveChangesAsync(cancellationToken);

    return Results.Ok(product);
}).WithName("AddProduct")
.WithTags("Products")
.WithMetadata(new SwaggerOperationAttribute("Add a product", "Add a product to the inventory"))
.WithOpenApi();

app.MapPost("/api/products/{id}/increase-inventory/{amount}", async (int id, int amount, AppDbContext context, CancellationToken cancellationToken) =>
{
    var theProduct = await context.Products.FindAsync([id], cancellationToken: cancellationToken);
    if (theProduct is null)
    {
        return Results.NotFound();
    }

    theProduct.InventoryCount += amount;
    await context.SaveChangesAsync(cancellationToken);

    return Results.Ok(theProduct);
}).WithName("IncreaseInventory")
.WithTags("Products")
.WithMetadata(new SwaggerOperationAttribute("Increase inventory", "Increase a product in the inventory"))
.WithOpenApi();

// get a product considering discount
app.MapGet("/api/products/{id}", async (int id, AppDbContext context, IMemoryCache cache, CancellationToken cancellationToken) =>
{
    if (!cache.TryGetValue(GetProductCacheKey(id), out Product? theProduct))
    {
        theProduct = await context.Products.FindAsync([id], cancellationToken: cancellationToken);
        if (theProduct is null)
        {
            return Results.NotFound();
        }

        var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(1));
        cache.Set(GetProductCacheKey(theProduct.Id), theProduct, cacheEntryOptions);
    }

    var priceWithDiscount = CalculateDiscount(theProduct!.Price, theProduct.Discount);
    theProduct.Price = priceWithDiscount;

    return Results.Ok(theProduct);
}).WithName("GetProductById")
.WithTags("Products")
.WithMetadata(new SwaggerOperationAttribute("Get a product", "Get a product by Id, considering its discount"))
.WithOpenApi();

// buy a product
// as far as we don't have an Auth system (and CurrentUser), so I'm passing `buyerUserId` as a parameter to this endpoint
// but in most cases it's recommended to use the CurrentUser Id in the back-end side and not passing from the client
app.MapPost("/api/products/{id}/buy", async (int id, int buyerUserId, AppDbContext context, CancellationToken cancellationToken) =>
{
    var theProduct = await context.Products.FindAsync([id], cancellationToken: cancellationToken);
    if (theProduct is null)
    {
        return Results.NotFound("Product not found");
    }

    if (theProduct.InventoryCount <= 0)
    {
        return Results.BadRequest("Insufficient inventory.");
    }

    // we assume this is the current user whose not coming from the client-side
    var theUser = await context.Users.FindAsync([buyerUserId], cancellationToken: cancellationToken);
    if (theUser is null)
    {
        return Results.NotFound("User (Buyer) not found");
    }

    theProduct.InventoryCount--;

    var anOrder = new Order { Product = theProduct, Buyer = theUser };
    await context.Orders.AddAsync(anOrder, cancellationToken: cancellationToken);
    await context.SaveChangesAsync(cancellationToken);

    return Results.Ok(anOrder);
}).WithName("BuyProduct")
.WithTags("Products")
.WithMetadata(new SwaggerOperationAttribute("Buy a product", "Buy a product, considering inventory stock"))
.WithOpenApi();

app.MapGet("/api/users", async (AppDbContext context, CancellationToken cancellationToken) =>
{
    return Results.Ok(await context.Users.ToListAsync(cancellationToken: cancellationToken));
}).WithName("GetUsers")
.WithTags("Users")
.WithMetadata(new SwaggerOperationAttribute("Get Users", "Get Users list"))
.WithOpenApi();

static string GetProductCacheKey(int productId)
{
    return $"products:{productId}";
}

static decimal CalculateDiscount(decimal price, double discount)
{
    return price * (1 - (decimal)(discount / 100));
}

app.Run();

