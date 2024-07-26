using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using SFS.Application.Abstractions.Repositories;
using SFS.Application.Services;
using SFS.Domain.Dtos;
using SFS.Domain.Models;

namespace SFS.Application.Tests.UnitTests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<IMemoryCache> _mockCache;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILogger<ProductService>> _mockLogger;

    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockCache = new Mock<IMemoryCache>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILogger<ProductService>>();

        _mockUnitOfWork
             .Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.FromResult(1));

        _productService = new ProductService(_mockProductRepository.Object
                                                , _mockUserRepository.Object
                                                , _mockOrderRepository.Object
                                                , _mockUnitOfWork.Object
                                                , _mockMapper.Object
                                                , _mockCache.Object
                                                , _mockLogger.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldAddProductSuccessfully_WhenProductModelIsValid()
    {
        // Arrange
        var model = new ProductDto { Title = "Product Test", Discount = 10, Price = 100, InventoryCount = 0 };
        var mockProduct = new Product
        {
            Id = 1,
            Title = model.Title,
            Discount = model.Discount,
            InventoryCount = model.InventoryCount,
            Price = model.Price
        };

        _mockMapper
                .Setup(mapper => mapper.Map<Product>(It.IsAny<ProductDto>()))
                .Returns(mockProduct);

        _mockProductRepository
                .Setup(repo => repo.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

        // Act
        var theCreatedProductId = await _productService.AddAsync(model, default);

        // Assert
        Assert.Equal(theCreatedProductId , mockProduct.Id);
        Assert.True(theCreatedProductId > 0);
        _mockMapper.Verify(m => m.Map<Product>(It.IsAny<ProductDto>()), Times.Once);
        _mockProductRepository.Verify(pr => pr.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
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