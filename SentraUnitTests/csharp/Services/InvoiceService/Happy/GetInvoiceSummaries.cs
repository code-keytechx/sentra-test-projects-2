using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sample.Api.Commands;
using Sample.Api.Factories;
using Sample.Api.Models;
using Sample.Api.Services.Accounting.Dto;
using Sample.Infrastructure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

[Collection("InvoiceServiceTests")]
public class InvoiceServiceHappyTests
{
    private readonly Mock<IRepository<Invoice>> _invoiceRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly InvoiceService _invoiceService;

    public InvoiceServiceHappyTests()
    {
        _invoiceRepositoryMock = new Mock<IRepository<Invoice>>();
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
            new Invoice { Id = 1, CustomerName = "John Doe", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "Paid" },
            new Invoice { Id = 2, CustomerName = "Jane Smith", InvoiceDate = DateTime.Now, TotalAmount = 200, Status = "Pending" }
        };

        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(invoices.AsQueryable());
        _mapperMock.Setup(mapper => mapper.Map<List<InvoiceSummary>>(invoices)).Returns(invoices.Select(i => new InvoiceSummary { Id = i.Id, CustomerName = i.CustomerName, InvoiceDate = i.InvoiceDate, TotalAmount = i.TotalAmount, Status = i.Status }).ToList());

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(2, result.TotalCount);
        Assert.Single(result.Items, item => item.CustomerName == "John Doe");
    }

    [Fact]
    public void GetInvoiceSummaries_ReturnsPagedResults_WithoutSearchTerm()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10 };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "John Doe", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "Paid" },
            new Invoice { Id = 2, CustomerName = "Jane Smith", InvoiceDate = DateTime.Now, TotalAmount = 200, Status = "Pending" }
        };

        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(invoices.AsQueryable());
        _mapperMock.Setup(mapper => mapper.Map<List<InvoiceSummary>>(invoices)).Returns(invoices.Select(i => new InvoiceSummary { Id = i.Id, CustomerName = i.CustomerName, InvoiceDate = i.InvoiceDate, TotalAmount = i.TotalAmount, Status = i.Status }).ToList());

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public void GetInvoiceSummaries_ReturnsEmptyPagedResults_WithNoInvoices()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10 };
        var invoices = new List<Invoice>();

        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(invoices.AsQueryable());
        _mapperMock.Setup(mapper => mapper.Map<List<InvoiceSummary>>(invoices)).Returns(new List<InvoiceSummary>());

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Items);
    }
}