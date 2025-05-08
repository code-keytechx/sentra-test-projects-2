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
    public void GetInvoiceSummaries_ReturnsEmptyResults_WhenNoInvoicesExist()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10 };
        _invoiceRepositoryMock.Setup(repo => repo.GetInvoices()).Returns(new List<Invoice>());

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public void GetInvoiceSummaries_ReturnsSingleResult_WhenOneInvoiceExists()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10 };
        var invoice = new Invoice { Id = 1, CustomerName = "Customer", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "Paid", ExportDate = DateTime.Now };
        _invoiceRepositoryMock.Setup(repo => repo.GetInvoices()).Returns(new List<Invoice> { invoice });

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.Single(result.Items);
        Assert.Equal(invoice.Id, result.Items.First().Id);
        Assert.Equal(invoice.CustomerName, result.Items.First().CustomerName);
        Assert.Equal(invoice.InvoiceDate, result.Items.First().InvoiceDate);
        Assert.Equal(invoice.TotalAmount, result.Items.First().TotalAmount);
        Assert.Equal(invoice.Status, result.Items.First().Status);
        Assert.Equal(invoice.ExportDate, result.Items.First().ExportDate);
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public void GetInvoiceSummaries_SortsByInvoiceDateDesc()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10 };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Parse("2023-01-01"), TotalAmount = 100, Status = "Paid", ExportDate = DateTime.Parse("2023-01-01") },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Parse("2023-01-02"), TotalAmount = 200, Status = "Pending", ExportDate = DateTime.Parse("2023-01-02") }
        };
        _invoiceRepositoryMock.Setup(repo => repo.GetInvoices()).Returns(invoices);

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.Items[0].Id);
        Assert.Equal(1, result.Items[1].Id);
    }
}