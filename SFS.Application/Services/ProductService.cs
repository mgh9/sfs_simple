using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using SFS.Application.Abstractions.Repositories;
using SFS.Application.Abstractions.Services;
using SFS.Domain.Dtos;
using SFS.Domain.Models;

namespace SFS.Application.Services;

public class ProductService : BaseApplicationService, IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IMemoryCache _cache;

    public ProductService(IProductRepository productRepository,
                            IUserRepository userRepository,
                            IOrderRepository orderRepository,
                            IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
        : base(unitOfWork, mapper)
    {
        _productRepository = productRepository;
        _userRepository = userRepository;
        _orderRepository = orderRepository;
        _cache = cache;
    }

    public async Task<int> AddAsync(ProductDto model, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(model.Title) || model.Title.Length > 40)
        {
            throw new ArgumentException("Product title must be less than 40 characters");
        }

        if (await _productRepository.IsProductTitleAlreadyExistAsync(model.Title, cancellationToken))
        {
            throw new ArgumentException("Product title must be unique.");
        }

        var product = Mapper.Map<Product>(model);
        await _productRepository.AddAsync(product, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return product.Id;
    }

    public async Task IncreaseInventoryAsync(int id, int amount, CancellationToken cancellationToken)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount should be greater than zero");
        }

        var product = await _productRepository.GetAsync(id, cancellationToken)
                        ?? throw new KeyNotFoundException("Product not found");

        product.InventoryCount += amount;
        await UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        if (!_cache.TryGetValue(GetProductCacheKey(id), out ProductDto? theProduct))
        {
            var theProductEntity = await _productRepository.GetAsync(id, cancellationToken: cancellationToken);
            if (theProductEntity is null)
            {
                return null;
            }

            theProduct = Mapper.Map<ProductDto>(theProductEntity);

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(1));
            _cache.Set(GetProductCacheKey(theProductEntity.Id), theProduct, cacheEntryOptions);
        }

        var priceWithDiscount = CalculateDiscount(theProduct!.Price, theProduct.Discount);
        theProduct.Price = priceWithDiscount;

        return theProduct;
    }

    public async Task<int> BuyAsync(int id, int buyerId, CancellationToken cancellationToken)
    {
        var theProduct = await _productRepository.GetAsync(id, cancellationToken)
                        ?? throw new KeyNotFoundException("Product not found");
        if (theProduct.InventoryCount <= 0)
        {
            throw new InvalidOperationException("Insufficient inventory.");
        }

        var user = await _userRepository.GetAsync(buyerId, cancellationToken)
                    ?? throw new KeyNotFoundException("User (Buyer) not found");

        theProduct.InventoryCount--;

        var order = new Order { Product = theProduct, Buyer = user };

        await _orderRepository.AddAsync(order, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return order.Id;
    }

    private static string GetProductCacheKey(int productId)
    {
        return $"products:{productId}";
    }

    private static decimal CalculateDiscount(decimal price, double discount)
    {
        return price * (1 - (decimal)(discount / 100));
    }
}