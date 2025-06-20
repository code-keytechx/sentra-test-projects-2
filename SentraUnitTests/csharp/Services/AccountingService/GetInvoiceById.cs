using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Sample.Api.Commands;
using Sample.Api.Factories;
using Sample.Api.Models;
using Sample.Api.Services.Accounting.Dto;
using Sample.Infrastructure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class AccountingServiceTests
{
    private readonly Mock<IInvoiceDocumentStore> _invoiceDocumentStoreMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AccountingService _accountingService;

    public AccountingServiceTests()
    {
        _invoiceDocumentStoreMock = new Mock<IInvoiceDocumentStore>();
        _mapperMock = new Mock<IMapper>();
        _accountingService = new AccountingService(_invoiceDocumentStoreMock.Object, _mapperMock.Object);
    }

    #region Happy Path Tests

    [Fact]
    public async Task GetInvoiceById_WithExistingInvoice_ReturnsInvoiceDetails()
    {
        // Arrange
        int invoiceId = 1;
        var invoiceDto = new InvoiceDto
        {
            Id = invoiceId,
            CustomerName = "John Doe",
            InvoiceDate = DateTime.UtcNow,
            TotalAmount = 100.00M,
            LineItems = new List<LineItemDto>
            {
                new LineItemDto { Id = 1, Description = "Item 1", Quantity = 1, UnitPrice = 50.00M, LineTotal = 50.00M },
                new LineItemDto { Id = 2, Description = "Item 2", Quantity = 2, UnitPrice = 25.00M, LineTotal = 50.00M }
            }
        };

        _invoiceDocumentStoreMock.Setup(store => store.GetAsync(invoiceId))
            .ReturnsAsync(invoiceDto);

        _mapperMock.Setup(mapper => mapper.Map<InvoiceDetailViewModel>(invoiceDto))
            .Returns(new InvoiceDetailViewModel
            {
                Id = invoiceId,
                CustomerName = "John Doe",
                InvoiceDate = DateTime.UtcNow,
                TotalAmount = 100.00M,
                LineItems = new List<InvoiceLineItemViewModel>
                {
                    new InvoiceLineItemViewModel { Id = 1, Description = "Item 1", Quantity = 1, UnitPrice = 50.00M, LineTotal = 50.00M },
                    new InvoiceLineItemViewModel { Id = 2, Description = "Item 2", Quantity = 2, UnitPrice = 25.00M, LineTotal = 50.00M }
                }
            });

        // Act
        var result = await _accountingService.GetInvoiceById(invoiceId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(invoiceId);
        result.CustomerName.Should().Be("John Doe");
        result.InvoiceDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.TotalAmount.Should().Be(100.00M);
        result.LineItems.Should().HaveCount(2);
        result.LineItems.First().Description.Should().Be("Item 1");
        result.LineItems.First().Quantity.Should().Be(1);
        result.LineItems.First().UnitPrice.Should().Be(50.00M);
        result.LineItems.First().LineTotal.Should().Be(50.00M);
        result.LineItems.Skip(1).First().Description.Should().Be("Item 2");
        result.LineItems.Skip(1).First().Quantity.Should().Be(2);
        result.LineItems.Skip(1).First().UnitPrice.Should().Be(25.00M);
        result.LineItems.Skip(1).First().LineTotal.Should().Be(50.00M);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public async Task GetInvoiceById_WithNonExistentInvoice_ReturnsNull()
    {
        // Arrange
        int nonExistentInvoiceId = 999;
        _invoiceDocumentStoreMock.Setup(store => store.GetAsync(nonExistentInvoiceId))
            .ReturnsAsync((InvoiceDto)null);

        // Act
        var result = await _accountingService.GetInvoiceById(nonExistentInvoiceId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetInvoiceById_WithEmptyLineItems_ReturnsInvoiceWithoutLineItems()
    {
        // Arrange
        int invoiceId = 1;
        var invoiceDto = new InvoiceDto
        {
            Id = invoiceId,
            CustomerName = "John Doe",
            InvoiceDate = DateTime.UtcNow,
            TotalAmount = 0.00M,
            LineItems = new List<LineItemDto>()
        };

        _invoiceDocumentStoreMock.Setup(store => store.GetAsync(invoiceId))
            .ReturnsAsync(invoiceDto);

        _mapperMock.Setup(mapper => mapper.Map<InvoiceDetailViewModel>(invoiceDto))
            .Returns(new InvoiceDetailViewModel
            {
                Id = invoiceId,
                CustomerName = "John Doe",
                InvoiceDate = DateTime.UtcNow,
                TotalAmount = 0.00M,
                LineItems = new List<InvoiceLineItemViewModel>()
            });

        // Act
        var result = await _accountingService.GetInvoiceById(invoiceId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(invoiceId);
        result.CustomerName.Should().Be("John Doe");
        result.InvoiceDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.TotalAmount.Should().Be(0.00M);
        result.LineItems.Should().BeEmpty();
    }

    #endregion

    #region Negative Tests

    [Fact]
    public async Task GetInvoiceById_WithNegativeInvoiceId_ThrowsArgumentException()
    {
        // Arrange
        int negativeInvoiceId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _accountingService.GetInvoiceById(negativeInvoiceId));
    }

    #endregion

    #region Exception Tests

    [Fact]
    public async Task GetInvoiceById_WhenDatabaseAccessFails_ThrowsInvalidOperationException()
    {
        // Arrange
        int invoiceId = 1;
        _invoiceDocumentStoreMock.Setup(store => store.GetAsync(invoiceId))
            .ThrowsAsync(new InvalidOperationException("Database access failed"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _accountingService.GetInvoiceById(invoiceId));
    }

    #endregion
}