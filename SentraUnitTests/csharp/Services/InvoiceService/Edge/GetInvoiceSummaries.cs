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
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly InvoiceService _invoiceService;

    public InvoiceServiceTests()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _mapperMock = new Mock<IMapper>();

        _invoiceService = new InvoiceService(_invoiceRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public void GetInvoiceSummaries_ReturnsPagedResults_WithValidSearchTerm()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { SearchTerm = "John", PageNumber = 1, PageSize = 10 };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "John Doe", InvoiceDate = DateTime.Now },
            new Invoice { Id = 2, CustomerName = "Jane Smith", InvoiceDate = DateTime.Now.AddDays(-1) }
        };
        var invoiceSummaries = new List<InvoiceSummary>
        {
            new InvoiceSummary { Id = 1, CustomerName = "John Doe", InvoiceDate = DateTime.Now },
            new InvoiceSummary { Id = 2, CustomerName = "Jane Smith", InvoiceDate = DateTime.Now.AddDays(-1) }
        };

        _invoiceRepositoryMock.Setup(repo => repo.GetInvoices()).Returns(invoices.AsQueryable());
        _mapperMock.Setup(mapper => mapper.Map<List<InvoiceSummary>>(invoices)).Returns(invoiceSummaries);

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal("John Doe", result.Items[0].CustomerName);
    }

    [Fact]
    public void GetInvoiceSummaries_ReturnsPagedResults_WithoutSearchTerm()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10 };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "John Doe", InvoiceDate = DateTime.Now },
            new Invoice { Id = 2, CustomerName = "Jane Smith", InvoiceDate = DateTime.Now.AddDays(-1) }
        };
        var invoiceSummaries = new List<InvoiceSummary>
        {
            new InvoiceSummary { Id = 1, CustomerName = "John Doe", InvoiceDate = DateTime.Now },
            new InvoiceSummary { Id = 2, CustomerName = "Jane Smith", InvoiceDate = DateTime.Now.AddDays(-1) }
        };

        _invoiceRepositoryMock.Setup(repo => repo.GetInvoices()).Returns(invoices.AsQueryable());
        _mapperMock.Setup(mapper => mapper.Map<List<InvoiceSummary>>(invoices)).Returns(invoiceSummaries);

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal("John Doe", result.Items[0].CustomerName);
    }

    [Fact]
    public void GetInvoiceSummaries_ReturnsEmptyPagedResults_WhenNoInvoicesMatchSearchTerm()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { SearchTerm = "NonExistent", PageNumber = 1, PageSize = 10 };
        var invoices = new List<Invoice>();
        var invoiceSummaries = new List<InvoiceSummary>();

        _invoiceRepositoryMock.Setup(repo => repo.GetInvoices()).Returns(invoices.AsQueryable());
        _mapperMock.Setup(mapper => mapper.Map<List<InvoiceSummary>>(invoices)).Returns(invoiceSummaries);

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Items);
    }
}