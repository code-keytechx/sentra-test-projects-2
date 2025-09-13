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

[Collection("Database Collection")]
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
    public void GetInvoiceSummaries_ReturnsEmptyResults_WhenNoInvoicesExist()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10 };
        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(new List<Invoice>().AsQueryable());

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
        var invoice = new Invoice { Id = 1, CustomerName = "Customer", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "Paid", ExportingBy = "User" };
        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(new List<Invoice> { invoice }.AsQueryable());
        _mapperMock.Setup(mapper => mapper.Map<List<InvoiceSummary>>(It.IsAny<IEnumerable<Invoice>>())).Returns(new List<InvoiceSummary> { new InvoiceSummary { Id = 1, CustomerName = "Customer", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "Paid", ExportingBy = "User" } });

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.Single(result.Items);
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
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Parse("2023-01-01"), TotalAmount = 100, Status = "Paid", ExportingBy = "User" },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Parse("2023-01-02"), TotalAmount = 200, Status = "Pending", ExportingBy = "User" }
        };
        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(invoices.AsQueryable());
        _mapperMock.Setup(mapper => mapper.Map<List<InvoiceSummary>>(It.IsAny<IEnumerable<Invoice>>())).Returns(invoices.Select(i => new InvoiceSummary { Id = i.Id, CustomerName = i.CustomerName, InvoiceDate = i.InvoiceDate, TotalAmount = i.TotalAmount, Status = i.Status, ExportingBy = i.ExportingBy }).ToList());

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(DateTime.Parse("2023-01-02"), result.Items[0].InvoiceDate);
        Assert.Equal(DateTime.Parse("2023-01-01"), result.Items[1].InvoiceDate);
    }
}