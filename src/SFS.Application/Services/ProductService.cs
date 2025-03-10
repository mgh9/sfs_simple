﻿using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository productRepository,
                            IUserRepository userRepository,
                            IOrderRepository orderRepository,
                            IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache, ILogger<ProductService> logger)
        : base(unitOfWork, mapper)
    {
        _productRepository = productRepository;
        _userRepository = userRepository;
        _orderRepository = orderRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ProductDto> AddAsync(ProductDto model, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(model.Title) || model.Title.Length > 40)
        {
            throw new ArgumentException("Product title must be less than 40 characters");
        }

        if (await IsTitleAlreadyExistsAsync(model.Title, cancellationToken))
        {
            throw new ArgumentException("Product title must be unique.");
        }

        var product = Mapper.Map<Product>(model);
        await _productRepository.AddAsync(product, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product successfully added in the Inventory");

        return Mapper.Map<ProductDto>(product);
    }

    private async Task<bool> IsTitleAlreadyExistsAsync(string title, CancellationToken cancellationToken)
    {
        return await _productRepository.GetByTitleAsync(title, cancellationToken) is not null;
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

        _logger.LogInformation("The Product (id: `{id}`) inventory increased by `{amount}`", id, amount);
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

            var priceWithDiscount = CalculateDiscount(theProduct!.Price, theProduct.Discount);
            theProduct.Price = priceWithDiscount;

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(1));
            _cache.Set(GetProductCacheKey(theProductEntity.Id), theProduct, cacheEntryOptions);
        }

        return theProduct;
    }

    public async Task<OrderDto> BuyAsync(int id, int buyerId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buying the Product (id: `{id}`) by user Id: `{userId}`", id, buyerId);

        var theProduct = await _productRepository.GetAsync(id, cancellationToken)
                        ?? throw new KeyNotFoundException("Product not found");
        if (theProduct.InventoryCount <= 0)
        {
            throw new InvalidOperationException("Insufficient inventory.");
        }

        var user = await _userRepository.GetAsync(buyerId, cancellationToken)
                    ?? throw new KeyNotFoundException("User (Buyer) not found");

        theProduct.InventoryCount--;

        var order = new Order { Product = theProduct, Buyer = user, Price = theProduct.Price, Discount = theProduct.Discount };

        await _orderRepository.AddAsync(order, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Mapper.Map<OrderDto>(order);
    }

    private static string GetProductCacheKey(int productId)
    {
        return $"products:{productId}";
    }

    private static decimal CalculateDiscount(decimal price, double discount)
    {
        var priceDiscountApplied = price * (1 - (decimal)(discount / 100));
        return Math.Round(priceDiscountApplied);
    }
}