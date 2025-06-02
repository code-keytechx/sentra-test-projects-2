using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Sample.Api.Commands;
using Sample.Api.Factories;
using Sample.Api.Models;
using Sample.Api.Services.Accounting.Dto;
using Sample.Infrastructure.Documents;
using System;
using System.Linq;

[TestFixture]
public class InvoiceServiceEdgeTests
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
    public void GetInvoiceSummaries_ThrowsArgumentNullException_WhenListArgsIsNull()
    {
        // Arrange
        InvoiceListArgs listArgs = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _invoiceService.GetInvoiceSummaries(listArgs));
    }

    [Test]
    public void GetInvoiceSummaries_ThrowsArgumentException_WhenPageSizeIsZero()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 0 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _invoiceService.GetInvoiceSummaries(listArgs));
    }

    [Test]
    public void GetInvoiceSummaries_ThrowsArgumentException_WhenPageNumberIsNegative()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = -1, PageSize = 10 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _invoiceService.GetInvoiceSummaries(listArgs));
    }

    [Test]
    public void GetInvoiceSummaries_SortsByInvoiceDateDescending()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10 };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "Paid", ExportingBy = "User1" },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now.AddDays(-1), TotalAmount = 200, Status = "Pending", ExportingBy = "User2" }
        };
        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(invoices);

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Items.Count);
        Assert.AreEqual(DateTime.Now, result.Items[0].InvoiceDate);
        Assert.AreEqual(DateTime.Now.AddDays(-1), result.Items[1].InvoiceDate);
    }

    [Test]
    public void GetInvoiceSummaries_FiltersBySearchTerm()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10, SearchTerm = "Customer1" };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "Paid", ExportingBy = "User1" },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now.AddDays(-1), TotalAmount = 200, Status = "Pending", ExportingBy = "User2" }
        };
        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(invoices);

        // Act
        var result = _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Items.Count);
        Assert.AreEqual("Customer1", result.Items[0].CustomerName);
    }
}