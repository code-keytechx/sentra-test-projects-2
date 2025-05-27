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
    private Mock<IRepository<Invoice>> _invoiceRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private InvoiceService _invoiceService;

    [SetUp]
    public void Setup()
    {
        _invoiceRepositoryMock = new Mock<IRepository<Invoice>>();
        _mapperMock = new Mock<IMapper>();
        _invoiceService = new InvoiceService(_invoiceRepositoryMock.Object, _mapperMock.Object);
    }

    [Test]
    public void GetInvoiceSummaries_ReturnsPagedResults_WithValidInput()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10, SearchTerm = null };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "Paid", ExportingBy = "User1" },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now.AddDays(-1), TotalAmount = 200, Status = "Pending", ExportingBy = "User2" }
        };

        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(invoices.AsQueryable());
        _mapperMock.Setup(mapper => mapper.Map<List<InvoiceSummary>>(invoices)).Returns(new List<InvoiceSummary>());

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.CurrentPage);
        Assert.AreEqual(10, result.PageSize);
        Assert.AreEqual(2, result.TotalCount);
        Assert.IsNotNull(result.Items);
        Assert.AreEqual(2, result.Items.Count);
    }

    [Test]
    public void GetInvoiceSummaries_ReturnsFilteredResults_WithSearchTerm()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10, SearchTerm = "Customer1" };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "Paid", ExportingBy = "User1" },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now.AddDays(-1), TotalAmount = 200, Status = "Pending", ExportingBy = "User2" }
        };

        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(invoices.AsQueryable());
        _mapperMock.Setup(mapper => mapper.Map<List<InvoiceSummary>>(new List<Invoice> { invoices[0] })).Returns(new List<InvoiceSummary>());

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.CurrentPage);
        Assert.AreEqual(10, result.PageSize);
        Assert.AreEqual(1, result.TotalCount);
        Assert.IsNotNull(result.Items);
        Assert.AreEqual(1, result.Items.Count);
    }

    [Test]
    public void GetInvoiceSummaries_SortsByInvoiceDateDesc()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10, SearchTerm = null };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now.AddDays(-1), TotalAmount = 100, Status = "Paid", ExportingBy = "User1" },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now, TotalAmount = 200, Status = "Pending", ExportingBy = "User2" }
        };

        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(invoices.AsQueryable());
        _mapperMock.Setup(mapper => mapper.Map<List<InvoiceSummary>>(invoices)).Returns(new List<InvoiceSummary>());

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.CurrentPage);
        Assert.AreEqual(10, result.PageSize);
        Assert.AreEqual(2, result.TotalCount);
        Assert.IsNotNull(result.Items);
        Assert.AreEqual(2, result.Items.Count);
        Assert.GreaterOrEqual(result.Items[0].InvoiceDate, result.Items[1].InvoiceDate);
    }
}