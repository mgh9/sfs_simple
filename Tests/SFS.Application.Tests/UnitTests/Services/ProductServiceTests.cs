using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using SFS.Application.Abstractions.Repositories;
using SFS.Application.Abstractions.Services;
using SFS.Application.Services;
using SFS.Domain.Dtos;

namespace SFS.Application.Tests.UnitTests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<IMemoryCache> _mockCache;

    private readonly IProductService _productService;

    public ProductServiceTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockCache = new Mock<IMemoryCache>();
        var mockMapper = new Mock<IMapper>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        _productService = new ProductService(_mockProductRepository.Object
                                                , _mockUserRepository.Object
                                                , _mockOrderRepository.Object
                                                , mockUnitOfWork.Object
                                                , mockMapper.Object
                                                , _mockCache.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldThrowArgumentException_WhenTitleIsTooLong()
    {
        // Arrange
        var model = new ProductDto { Title = new string('x', 41) };

        // Act
        var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _productService.AddAsync(model, default));

        // Assert
        Assert.Equal("Product title must be less than 40 characters", ex.Message);
    }

    [Fact]
    public async void AddAsync_ShouldThrowArgumentException_WhenTitleAlreadyExists()
    {
        // Arrange
        var existingTitle = "Existing Title";
        var model = new ProductDto { Title = existingTitle };

        // mock the behavior of the repository to simulate that the title already exists
        _mockProductRepository
                    .Setup(repo => repo.GetByTitleAsync(existingTitle, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new Domain.Models.Product { Title = existingTitle });

        // Act
        var ex = await Assert.ThrowsAsync<ArgumentException>(async () => { await _productService.AddAsync(model, default); });

        // Assert
        Assert.Equal("Product title must be unique.", ex.Message);
    }
}