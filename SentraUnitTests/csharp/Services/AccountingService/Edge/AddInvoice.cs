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

[Collection("AccountingServiceTests")]
public class AccountingServiceEdgeTests
{
    private readonly Mock<IAccountingDbContext> _mockDbContext;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AccountingService _accountingService;

    public AccountingServiceEdgeTests()
    {
        _mockDbContext = new Mock<IAccountingDbContext>();
        _mockMapper = new Mock<IMapper>();

        _accountingService = new AccountingService(_mockDbContext.Object, _mockMapper.Object);
    }

    [Fact]
    public void AddInvoice_WithEmptyCustomerName_ThrowsArgumentException()
    {
        // Arrange
        var dtoInvoice = new DtoAddInvoiceInput
        {
            CustomerName = string.Empty,
            InvoiceDate = DateTime.Now,
            LineItems = new List<DtoInvoiceLineItem>()
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _accountingService.AddInvoice(dtoInvoice));
    }

    [Fact]
    public void AddInvoice_WithInvalidInvoiceDate_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var dtoInvoice = new DtoAddInvoiceInput
        {
            CustomerName = "John Doe",
            InvoiceDate = DateTime.MinValue,
            LineItems = new List<DtoInvoiceLineItem>()
        };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _accountingService.AddInvoice(dtoInvoice));
    }

    [Fact]
    public void AddInvoice_WithNullDtoInvoice_ThrowsArgumentNullException()
    {
        // Arrange
        DtoAddInvoiceInput dtoInvoice = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _accountingService.AddInvoice(dtoInvoice));
    }
}