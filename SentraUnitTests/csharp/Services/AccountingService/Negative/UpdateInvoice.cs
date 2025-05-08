using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sample.Api.Commands;
using Sample.Api.Factories;
using Sample.Api.Models;
using Sample.Api.Services.Accounting.Dto;
using Sample.Infrastructure.Documents;
using System;
using Xunit;

public class AccountingServiceTests
{
    private readonly Mock<IAccountingDbContext> _mockDbContext;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AccountingService _accountingService;

    public AccountingServiceTests()
    {
        _mockDbContext = new Mock<IAccountingDbContext>();
        _mockMapper = new Mock<IMapper>();
        _accountingService = new AccountingService(_mockDbContext.Object, _mockMapper.Object);
    }

    [Fact]
    public void UpdateInvoice_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var dtoInvoice = new DtoUpdateInvoiceInput
        {
            CustomerName = "John Doe",
            InvoiceDate = DateTime.Now,
            LineItems = new List<DtoInvoiceLineItem>
            {
                new DtoInvoiceLineItem { Description = "Item 1", Quantity = 1, UnitPrice = 10 }
            }
        };

        _mockDbContext.Setup(db => db.Invoices.Include("LineItems").FirstOrDefault(It.IsAny<int>())).Returns((Invoice)null);

        // Act
        var result = _accountingService.UpdateInvoice(dtoInvoice, 1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void UpdateInvoice_WithEmptyLineItems_ClearsExistingLineItems()
    {
        // Arrange
        var dtoInvoice = new DtoUpdateInvoiceInput
        {
            CustomerName = "John Doe",
            InvoiceDate = DateTime.Now,
            LineItems = new List<DtoInvoiceLineItem>()
        };

        var invoice = new Invoice
        {
            Id = 1,
            CustomerName = "Old Customer",
            InvoiceDate = DateTime.Now,
            LineItems = new List<InvoiceLineItem>
            {
                new InvoiceLineItem { Id = 1, Description = "Old Item", Quantity = 1, UnitPrice = 10, LineTotal = 10 }
            }
        };

        _mockDbContext.Setup(db => db.Invoices.Include("LineItems").FirstOrDefault(1)).Returns(invoice);

        // Act
        var result = _accountingService.UpdateInvoice(dtoInvoice, 1);

        // Assert
        Assert.Empty(result.LineItems);
    }

    [Fact]
    public void UpdateInvoice_WithNullDtoInvoice_ThrowsArgumentNullException()
    {
        // Arrange
        DtoUpdateInvoiceInput dtoInvoice = null;
        int id = 1;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _accountingService.UpdateInvoice(dtoInvoice, id));
    }
}