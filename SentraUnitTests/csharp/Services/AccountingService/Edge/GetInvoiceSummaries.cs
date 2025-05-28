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
using System.Linq;

[TestFixture]
public class AccountingServiceEdgeTests
{
    private Mock<IRepository<Invoice>> _invoiceRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private AccountingService _accountingService;

    [SetUp]
    public void Setup()
    {
        _invoiceRepositoryMock = new Mock<IRepository<Invoice>>();
        _mapperMock = new Mock<IMapper>();
        _accountingService = new AccountingService(_invoiceRepositoryMock.Object, _mapperMock.Object);
    }

    [Test]
    public void GetInvoiceSummaries_ReturnsEmptyResults_WhenNoInvoicesExist()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10 };
        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(new List<Invoice>().AsQueryable());

        // Act
        var result = _accountingService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.TotalCount);
        Assert.IsEmpty(result.Items);
    }

    [Test]
    public void GetInvoiceSummaries_SortsByInvoiceDateDesc_WhenNoFilterApplied()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10 };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "CustomerA", InvoiceDate = DateTime.Parse("2023-01-01") },
            new Invoice { Id = 2, CustomerName = "CustomerB", InvoiceDate = DateTime.Parse("2023-01-02") }
        };
        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(invoices.AsQueryable());

        // Act
        var result = _accountingService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.TotalCount);
        Assert.AreEqual(2, result.Items.Count);
        Assert.AreEqual(DateTime.Parse("2023-01-02"), result.Items[0].InvoiceDate);
        Assert.AreEqual(DateTime.Parse("2023-01-01"), result.Items[1].InvoiceDate);
    }

    [Test]
    public void GetInvoiceSummaries_PaginatesCorrectly_WhenMultiplePages()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 2, PageSize = 1 };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "CustomerA", InvoiceDate = DateTime.Parse("2023-01-01") },
            new Invoice { Id = 2, CustomerName = "CustomerB", InvoiceDate = DateTime.Parse("2023-01-02") }
        };
        _invoiceRepositoryMock.Setup(repo => repo.Query()).Returns(invoices.AsQueryable());

        // Act
        var result = _accountingService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.TotalCount);
        Assert.AreEqual(1, result.Items.Count);
        Assert.AreEqual(2, result.CurrentPage);
        Assert.AreEqual(1, result.PageSize);
        Assert.AreEqual(2, result.Items[0].Id);
    }
}