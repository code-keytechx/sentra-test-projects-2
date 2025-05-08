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

public class InvoiceServiceTests
{
    private readonly Mock<IRepository<Invoice>> _invoiceRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly InvoiceService _invoiceService;

    public InvoiceServiceTests()
    {
        _invoiceRepositoryMock = new Mock<IRepository<Invoice>>();
        _mapperMock = new Mock<IMapper>();
        _invoiceService = new InvoiceService(_invoiceRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public void GetInvoiceSummaries_ReturnsPagedResults_WithEmptySearchTerm()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { SearchTerm = "", PageNumber = 1, PageSize = 10 };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "Paid" },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now.AddDays(-1), TotalAmount = 200, Status = "Pending" }
        };
        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(invoices.AsQueryable());

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(2, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal("Customer1", result.Items[0].CustomerName);
    }

    [Fact]
    public void GetInvoiceSummaries_SortsByInvoiceDateDescending()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { SearchTerm = "", PageNumber = 1, PageSize = 10 };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "Paid" },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now.AddDays(-1), TotalAmount = 200, Status = "Pending" }
        };
        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(invoices.AsQueryable());

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal("Customer2", result.Items[0].CustomerName);
    }

    [Fact]
    public void GetInvoiceSummaries_ThrowsArgumentNullException_WhenListArgsIsNull()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => _invoiceService.GetInvoiceSummaries(null));
    }
}