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
    public void GetInvoiceSummaries_WithMinimumPageNumber_ReturnsCorrectPagedResults()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10, SearchTerm = null };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "Paid", ExportedDate = DateTime.Now, ExportedBy = "User1" },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now, TotalAmount = 200, Status = "Pending", ExportedDate = DateTime.Now, ExportedBy = "User2" }
        };
        _invoiceRepositoryMock.Setup(repo => repo.GetInvoices()).Returns(invoices.AsQueryable());

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(2, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal(1, result.Items[0].Id);
    }

    [Fact]
    public void GetInvoiceSummaries_WithMaximumPageSize_ReturnsCorrectPagedResults()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = int.MaxValue, SearchTerm = null };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "Paid", ExportedDate = DateTime.Now, ExportedBy = "User1" },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now, TotalAmount = 200, Status = "Pending", ExportedDate = DateTime.Now, ExportedBy = "User2" }
        };
        _invoiceRepositoryMock.Setup(repo => repo.GetInvoices()).Returns(invoices.AsQueryable());

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(int.MaxValue, result.PageSize);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(1, result.Items[0].Id);
        Assert.Equal(2, result.Items[1].Id);
    }

    [Fact]
    public void GetInvoiceSummaries_WithLargePageNumber_ReturnsEmptyPagedResults()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 100, PageSize = 10, SearchTerm = null };
        var invoices = new List<Invoice>();
        _invoiceRepositoryMock.Setup(repo => repo.GetInvoices()).Returns(invoices.AsQueryable());

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.Equal(100, result.CurrentPage);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Items);
    }
}