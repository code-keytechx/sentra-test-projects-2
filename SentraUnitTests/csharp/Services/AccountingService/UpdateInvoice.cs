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
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AccountingService _accountingService;

    public AccountingServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _accountingService = new AccountingService(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    #region Happy Path Tests

    [Fact]
    public async Task UpdateInvoice_WithValidInput_ReturnsUpdatedInvoice()
    {
        // Arrange
        var dtoInvoice = new DtoUpdateInvoiceInput
        {
            CustomerName = "John Doe",
            InvoiceDate = DateTime.UtcNow,
            LineItems = new List<DtoInvoiceLineItem>
            {
                new DtoInvoiceLineItem { Description = "Item 1", Quantity = 2, UnitPrice = 10.00m },
                new DtoInvoiceLineItem { Description = "Item 2", Quantity = 1, UnitPrice = 5.00m }
            }
        };

        var invoice = new Invoice
        {
            Id = 1,
            CustomerName = "Old Customer",
            InvoiceDate = DateTime.UtcNow.AddDays(-1),
            LineItems = new List<InvoiceLineItem>
            {
                new InvoiceLineItem { Id = 1, Description = "Old Item", Quantity = 1, UnitPrice = 10.00m, LineTotal = 10.00m }
            }
        };

        _mockUnitOfWork.Setup(db => db.Invoices.Include("LineItems").FirstOrDefaultAsync(i => i.Id == 1))
            .ReturnsAsync(invoice);

        _mockMapper.Setup(mapper => mapper.Map<Invoice>(dtoInvoice))
            .Returns(invoice);

        // Act
        var result = await _accountingService.UpdateInvoice(dtoInvoice, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("John Doe", result.CustomerName);
        Assert.Equal(DateTime.UtcNow.Date, result.InvoiceDate.Date);
        Assert.Equal(25.00m, result.TotalAmount);
        Assert.Equal(2, result.LineItems.Count);
        Assert.Equal("Item 1", result.LineItems[0].Description);
        Assert.Equal(2, result.LineItems[0].Quantity);
        Assert.Equal(10.00m, result.LineItems[0].UnitPrice);
        Assert.Equal(20.00m, result.LineItems[0].LineTotal);
        Assert.Equal("Item 2", result.LineItems[1].Description);
        Assert.Equal(1, result.LineItems[1].Quantity);
        Assert.Equal(5.00m, result.LineItems[1].UnitPrice);
        Assert.Equal(5.00m, result.LineItems[1].LineTotal);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public async Task UpdateInvoice_WithEmptyLineItems_ClearsExistingItems()
    {
        // Arrange
        var dtoInvoice = new DtoUpdateInvoiceInput
        {
            CustomerName = "John Doe",
            InvoiceDate = DateTime.UtcNow,
            LineItems = new List<DtoInvoiceLineItem>()
        };

        var invoice = new Invoice
        {
            Id = 1,
            CustomerName = "Old Customer",
            InvoiceDate = DateTime.UtcNow.AddDays(-1),
            LineItems = new List<InvoiceLineItem>
            {
                new InvoiceLineItem { Id = 1, Description = "Old Item", Quantity = 1, UnitPrice = 10.00m, LineTotal = 10.00m }
            }
        };

        _mockUnitOfWork.Setup(db => db.Invoices.Include("LineItems").FirstOrDefaultAsync(i => i.Id == 1))
            .ReturnsAsync(invoice);

        _mockMapper.Setup(mapper => mapper.Map<Invoice>(dtoInvoice))
            .Returns(invoice);

        // Act
        var result = await _accountingService.UpdateInvoice(dtoInvoice, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.LineItems);
    }

    [Fact]
    public async Task UpdateInvoice_WithNullDtoInvoice_ReturnsNull()
    {
        // Arrange
        var dtoInvoice = (DtoUpdateInvoiceInput)null;

        _mockUnitOfWork.Setup(db => db.Invoices.Include("LineItems").FirstOrDefaultAsync(i => i.Id == 1))
            .ReturnsAsync((Invoice)null);

        // Act
        var result = await _accountingService.UpdateInvoice(dtoInvoice, 1);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region Negative Tests

    [Fact]
    public async Task UpdateInvoice_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var dtoInvoice = new DtoUpdateInvoiceInput
        {
            CustomerName = "John Doe",
            InvoiceDate = DateTime.UtcNow,
            LineItems = new List<DtoInvoiceLineItem>
            {
                new DtoInvoiceLineItem { Description = "Item 1", Quantity = 2, UnitPrice = 10.00m },
                new DtoInvoiceLineItem { Description = "Item 2", Quantity = 1, UnitPrice = 5.00m }
            }
        };

        _mockUnitOfWork.Setup(db => db.Invoices.Include("LineItems").FirstOrDefaultAsync(i => i.Id == 1))
            .ReturnsAsync((Invoice)null);

        // Act
        var result = await _accountingService.UpdateInvoice(dtoInvoice, 1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateInvoice_WithInvalidLineItemQuantities_ReturnsNull()
    {
        // Arrange
        var dtoInvoice = new DtoUpdateInvoiceInput
        {
            CustomerName = "John Doe",
            InvoiceDate = DateTime.UtcNow,
            LineItems = new List<DtoInvoiceLineItem>
            {
                new DtoInvoiceLineItem { Description = "Item 1", Quantity = -1, UnitPrice = 10.00m },
                new DtoInvoiceLineItem { Description = "Item 2", Quantity = 1, UnitPrice = 5.00m }
            }
        };

        var invoice = new Invoice
        {
            Id = 1,
            CustomerName = "Old Customer",
            InvoiceDate = DateTime.UtcNow.AddDays(-1),
            LineItems = new List<InvoiceLineItem>
            {
                new InvoiceLineItem { Id = 1, Description = "Old Item", Quantity = 1, UnitPrice = 10.00m, LineTotal = 10.00m }
            }
        };

        _mockUnitOfWork.Setup(db => db.Invoices.Include("LineItems").FirstOrDefaultAsync(i => i.Id == 1))
            .ReturnsAsync(invoice);

        _mockMapper.Setup(mapper => mapper.Map<Invoice>(dtoInvoice))
            .Returns(invoice);

        // Act
        var result = await _accountingService.UpdateInvoice(dtoInvoice, 1);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region Exception Tests

    [Fact]
    public async Task UpdateInvoice_WhenDbContextSaveChangesThrowsException_RethrowsException()
    {
        // Arrange
        var dtoInvoice = new DtoUpdateInvoiceInput
        {
            CustomerName = "John Doe",
            InvoiceDate = DateTime.UtcNow,
            LineItems = new List<DtoInvoiceLineItem>
            {
                new DtoInvoiceLineItem { Description = "Item 1", Quantity = 2, UnitPrice = 10.00m },
                new DtoInvoiceLineItem { Description = "Item 2", Quantity = 1, UnitPrice = 5.00m }
            }
        };

        var invoice = new Invoice
        {
            Id = 1,
            CustomerName = "Old Customer",
            InvoiceDate = DateTime.UtcNow.AddDays(-1),
            LineItems = new List<InvoiceLineItem>
            {
                new InvoiceLineItem { Id = 1, Description = "Old Item", Quantity = 1, UnitPrice = 10.00m, LineTotal = 10.00m }
            }
        };

        _mockUnitOfWork.Setup(db => db.Invoices.Include("LineItems").FirstOrDefaultAsync(i => i.Id == 1))
            .ReturnsAsync(invoice);

        _mockMapper.Setup(mapper => mapper.Map<Invoice>(dtoInvoice))
            .Returns(invoice);

        _mockUnitOfWork.Setup(db => db.SaveChangesAsync())
            .ThrowsAsync(new InvalidOperationException("Database save operation failed"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _accountingService.UpdateInvoice(dtoInvoice, 1));
    }

    #endregion
}