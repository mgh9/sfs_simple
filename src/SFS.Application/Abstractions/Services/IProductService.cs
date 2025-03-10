﻿using SFS.Domain.Dtos;

namespace SFS.Application.Abstractions.Services;

public interface IProductService
{
    Task<ProductDto> AddAsync(ProductDto productDto, CancellationToken cancellationToken);
    Task IncreaseInventoryAsync(int id, int amount, CancellationToken cancellationToken);
    Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<OrderDto> BuyAsync(int id, int buyerId, CancellationToken cancellationToken);
}

