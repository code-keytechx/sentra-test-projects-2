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

[Collection("Database Collection")]
public class AccountingServiceEdgeTests
{
    private readonly Mock<DbContext> _mockDbContext;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AccountingService _accountingService;

    public AccountingServiceEdgeTests()
    {
        _mockDbContext = new Mock<DbContext>();
        _mockMapper = new Mock<IMapper>();

        _accountingService = new AccountingService(_mockDbContext.Object, _mockMapper.Object);
    }

    [Fact]
    public void UpdateInvoice_WithNegativeId_ReturnsNull()
    {
        // Arrange
        var dtoInvoice = new DtoUpdateInvoiceInput { CustomerName = "John Doe", InvoiceDate = DateTime.Now, LineItems = new List<DtoInvoiceLineItem>() };
        int id = -1;

        // Act
        var result = _accountingService.UpdateInvoice(dtoInvoice, id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void UpdateInvoice_WithZeroId_ReturnsNull()
    {
        // Arrange
        var dtoInvoice = new DtoUpdateInvoiceInput { CustomerName = "John Doe", InvoiceDate = DateTime.Now, LineItems = new List<DtoInvoiceLineItem>() };
        int id = 0;

        // Act
        var result = _accountingService.UpdateInvoice(dtoInvoice, id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void UpdateInvoice_WithLargePositiveId_ReturnsNull()
    {
        // Arrange
        var dtoInvoice = new DtoUpdateInvoiceInput { CustomerName = "John Doe", InvoiceDate = DateTime.Now, LineItems = new List<DtoInvoiceLineItem>() };
        int id = int.MaxValue;

        // Act
        var result = _accountingService.UpdateInvoice(dtoInvoice, id);

        // Assert
        Assert.Null(result);
    }
}