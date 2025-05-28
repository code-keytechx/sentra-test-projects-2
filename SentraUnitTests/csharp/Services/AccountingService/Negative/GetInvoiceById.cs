```csharp
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
    [Fact]
    public void GetInvoiceById_EmptyDatabase_ReturnsNull()
    {
        // Arrange
        var mockDbContext = new Mock<IInvoicesDocument>();
        mockDbContext.Setup(db => db.Invoices).Returns(new List<Invoice>());

        var mapperMock = new Mock<IMapper>();

        var accountingService = new AccountingService(mockDbContext.Object, mapperMock.Object);

        // Act
        var result = accountingService.GetInvoiceById(0);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetInvoiceById_IdWithNoMatchingRecord_ReturnsNull()
    {
        // Arrange
        var mockDbContext = new Mock<IInvoicesDocument>();
        mockDbContext.Setup(db => db.Invoices).Returns(new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer A", InvoiceDate = DateTime.Now, TotalAmount = 100 },
            new Invoice { Id = 2, CustomerName = "Customer B", InvoiceDate = DateTime.Now, TotalAmount = 200 }
        });

        var mapperMock = new Mock<IMapper>();

        var accountingService = new AccountingService(mockDbContext.Object, mapperMock.Object);

        // Act
        var result = accountingService.GetInvoiceById(3);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetInvoiceById_IdWithPartialMatch_ReturnsNull()
    {
        // Arrange
        var mockDbContext = new Mock<IInvoicesDocument>();
        mockDbContext.Setup(db => db.Invoices).Returns(new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer A", InvoiceDate = DateTime.Now, TotalAmount = 100 },
            new Invoice { Id = 2, CustomerName = "Customer B", InvoiceDate = DateTime.Now, TotalAmount = 200 }
        });

        var mapperMock = new Mock<IMapper>();

        var accountingService = new AccountingService(mockDbContext.Object, mapperMock.Object);

        // Act
        var result = accountingService.GetInvoiceById(123);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetInvoiceById_IdWithLeadingZero_ReturnsNull()
    {
        // Arrange
        var mockDbContext = new Mock<IInvoicesDocument>();
        mockDbContext.Setup(db => db.Invoices).Returns(new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer A", InvoiceDate = DateTime.Now, TotalAmount = 100 },
            new Invoice { Id = 2, CustomerName = "Customer B", InvoiceDate = DateTime.Now, TotalAmount = 200 }
        });

        var mapperMock = new Mock<IMapper>();

        var accountingService = new AccountingService(mockDbContext.Object, mapperMock.Object);

        // Act
        var result = accountingService.GetInvoiceById(01);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetInvoiceById_IdWithTrailingZero_ReturnsNull()
    {
        // Arrange
        var mockDbContext = new Mock<IInvoicesDocument>();
        mockDbContext.Setup(db => db.Invoices).Returns(new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer A", InvoiceDate = DateTime.Now, TotalAmount = 100 },
            new Invoice { Id = 2, CustomerName = "Customer B", InvoiceDate = DateTime.Now, TotalAmount = 200 }
        });

        var mapperMock = new Mock<IMapper>();

        var accountingService = new AccountingService(mockDbContext.Object, mapperMock.Object);

        // Act
        var result = accountingService.GetInvoiceById(10);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetInvoiceById_IdWithNegativeValue_ReturnsNull()
    {
        // Arrange
        var mockDbContext = new Mock<IInvoicesDocument>();
        mockDbContext.Setup(db => db.Invoices).Returns(new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer A", InvoiceDate = DateTime.Now, TotalAmount = 100 },
            new Invoice { Id = 2, CustomerName = "Customer B", InvoiceDate = DateTime.Now, TotalAmount = 200 }
        });

        var mapperMock = new Mock<IMapper>();

        var accountingService = new AccountingService(mockDbContext.Object, mapperMock.Object);

        // Act
        var result = accountingService.GetInvoiceById(-1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetInvoiceById_Id