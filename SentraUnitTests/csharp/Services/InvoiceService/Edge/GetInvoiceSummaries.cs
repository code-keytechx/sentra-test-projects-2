using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Sample.Api.Commands;
using Sample.Api.Factories;
using Sample.Api.Models;
using Sample.Api.Services.Accounting.Dto;
using Sample.Infrastructure.Documents;
using System.Collections.Generic;

[TestFixture]
public class InvoiceServiceTests
{
    private Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private InvoiceService _invoiceService;

    [SetUp]
    public void Setup()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _mapperMock = new Mock<IMapper>();
        _invoiceService = new InvoiceService(_invoiceRepositoryMock.Object, _mapperMock.Object);
    }

    [Test]
    public void GetInvoiceSummaries_ReturnsPagedResults_WithValidSearchTerm()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { SearchTerm = "John", PageNumber = 1, PageSize = 10 };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "John Doe", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "Paid" },
            new Invoice { Id = 2, CustomerName = "Jane Smith", InvoiceDate = DateTime.Now, TotalAmount = 200, Status = "Pending" }
        };

        _invoiceRepositoryMock.Setup(repo => repo.GetInvoices()).Returns(invoices.AsQueryable());
        _mapperMock.Setup(mapper => mapper.Map<List<InvoiceSummary>>(invoices)).Returns(new List<InvoiceSummary>());

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.CurrentPage);
        Assert.AreEqual(10, result.PageSize);
        Assert.AreEqual(2, result.TotalCount);
        Assert.AreEqual(0, result.Items.Count); // Assuming mapping returns empty list for simplicity
    }

    [Test]
    public void GetInvoiceSummaries_ReturnsPagedResults_WithoutSearchTerm()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10 };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "John Doe", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "Paid" },
            new Invoice { Id = 2, CustomerName = "Jane Smith", InvoiceDate = DateTime.Now, TotalAmount = 200, Status = "Pending" }
        };

        _invoiceRepositoryMock.Setup(repo => repo.GetInvoices()).Returns(invoices.AsQueryable());
        _mapperMock.Setup(mapper => mapper.Map<List<InvoiceSummary>>(invoices)).Returns(new List<InvoiceSummary>());

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.CurrentPage);
        Assert.AreEqual(10, result.PageSize);
        Assert.AreEqual(2, result.TotalCount);
        Assert.AreEqual(0, result.Items.Count); // Assuming mapping returns empty list for simplicity
    }

    [Test]
    public void GetInvoiceSummaries_ReturnsEmptyPagedResults_WithNoInvoices()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10 };
        var invoices = new List<Invoice>();

        _invoiceRepositoryMock.Setup(repo => repo.GetInvoices()).Returns(invoices.AsQueryable());
        _mapperMock.Setup(mapper => mapper.Map<List<InvoiceSummary>>(invoices)).Returns(new List<InvoiceSummary>());

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.CurrentPage);
        Assert.AreEqual(10, result.PageSize);
        Assert.AreEqual(0, result.TotalCount);
        Assert.AreEqual(0, result.Items.Count);
    }
}