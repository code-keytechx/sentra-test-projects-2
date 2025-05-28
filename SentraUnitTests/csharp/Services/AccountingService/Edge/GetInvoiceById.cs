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
    public void GetInvoiceById_InvalidId_ReturnsNull()
    {
        // Arrange
        int invalidId = -1;

        // Act
        var result = _accountingService.GetInvoiceById(invalidId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetInvoiceById_EmptyDatabase_ReturnsNull()
    {
        // Arrange
        _mockDbContext.Setup(db => db.Invoices).Returns(new List<Invoice>());

        // Act
        var result = _accountingService.GetInvoiceById(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetInvoiceById_IdWithNoMatchingRecord_ReturnsNull()
    {
        // Arrange
        _mockDbContext.Setup(db => db.Invoices).Returns(new List<Invoice>
        {
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now, TotalAmount = 100 }
        });

        // Act
        var result = _accountingService.GetInvoiceById(1);

        // Assert
        Assert.Null(result);
    }
}