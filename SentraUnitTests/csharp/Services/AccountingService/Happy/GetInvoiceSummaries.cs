```csharp
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
public class AccountingServiceTests
{
    private Mock<IMapper> _mapperMock;
    private Mock<IInvoicesRepository> _invoicesRepositoryMock;
    private AccountingService _accountingService;

    [SetUp]
    public void Setup()
    {
        _mapperMock = new Mock<IMapper>();
        _invoicesRepositoryMock = new Mock<IInvoicesRepository>();
        _accountingService = new AccountingService(_mapperMock.Object, _invoicesRepositoryMock.Object);
    }

    [Test]
    public void GetInvoiceSummaries_ReturnsPagedResults_WithValidInputs()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10, SearchTerm = null };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now.AddDays(-1) }
        };
        var invoiceSummaries = invoices.Select(i => new InvoiceSummary
        {
            Id = i.Id,
            CustomerName = i.CustomerName,
            InvoiceDate = i.InvoiceDate
        }).ToList();

        _invoicesRepositoryMock.Setup(repo => repo.GetInvoices()).Returns(invoices.AsQueryable());

        // Act
        var result = _accountingService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.CurrentPage);
        Assert.AreEqual(10, result.PageSize);
        Assert.AreEqual(2, result.TotalCount);
        Assert.AreEqual(2, result.Items.Count);
        Assert.AreEqual(invoiceSummaries[0].Id, result.Items[0].Id);
        Assert.AreEqual(invoiceSummaries[0].CustomerName, result.Items[0].CustomerName);
        Assert.AreEqual(invoiceSummaries[0].InvoiceDate, result.Items[0].InvoiceDate);
        Assert.AreEqual(invoiceSummaries[1].Id, result.Items[1].Id);
        Assert.AreEqual(invoiceSummaries[1].CustomerName, result.Items[1].CustomerName);
        Assert.AreEqual(invoiceSummaries[1].InvoiceDate, result.Items[1].InvoiceDate);
    }

    [Test]
    public void GetInvoiceSummaries_ReturnsFilteredResults_WithSearchTerm()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10, SearchTerm = "Customer1" };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now.AddDays(-1) }
        };
        var invoiceSummaries = new List<InvoiceSummary>
        {
            new InvoiceSummary { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now }
        };

        _invoicesRepositoryMock.Setup(repo => repo.GetInvoices()).Returns(invoices.AsQueryable());

        // Act
        var result = _accountingService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.CurrentPage);
        Assert.AreEqual(10, result.PageSize);
        Assert.AreEqual(1, result.TotalCount);
        Assert.AreEqual(1, result.Items.Count);
        Assert.AreEqual(invoiceSummaries[0].Id, result.Items[0].Id);
        Assert.AreEqual(invoiceSummaries[0].CustomerName, result.Items[0].CustomerName);
        Assert.AreEqual(invoiceSummaries[0].InvoiceDate, result.Items[0].InvoiceDate);
    }

    [Test]
    public void GetInvoiceSummaries_SortsByInvoiceDateDesc()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10, SearchTerm = null };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now.AddDays(-1) },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now }
        };
        var invoiceSummaries = invoices.Select(i => new InvoiceSummary
        {
            Id = i.Id,
            CustomerName = i.CustomerName,
            InvoiceDate = i.InvoiceDate
        }).ToList();

        _invoicesRepositoryMock.Setup(repo => repo.GetInvoices()).Returns(invoices.AsQueryable());

        // Act
        var result = _accountingService.GetInvoiceSummaries(listArgs);

