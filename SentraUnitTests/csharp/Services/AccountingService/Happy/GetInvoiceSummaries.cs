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
        var invoiceSummaries = invoices.Select(i => new InvoiceSummary { Id = i.Id, CustomerName = i.CustomerName, InvoiceDate = i.InvoiceDate }).ToList();
        var pagedResults = new PagedResults<InvoiceSummary>
        {
            CurrentPage = listArgs.PageNumber,
            PageSize = listArgs.PageSize,
            TotalCount = invoices.Count,
            Items = invoiceSummaries
        };

        _invoicesRepositoryMock.Setup(repo => repo.GetInvoices()).Returns(invoices.AsQueryable());

        // Act
        var result = _accountingService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(pagedResults.CurrentPage, result.CurrentPage);
        Assert.AreEqual(pagedResults.PageSize, result.PageSize);
        Assert.AreEqual(pagedResults.TotalCount, result.TotalCount);
        Assert.AreEqual(pagedResults.Items.Count, result.Items.Count);
        Assert.AreEqual(pagedResults.Items[0].Id, result.Items[0].Id);
        Assert.AreEqual(pagedResults.Items[0].CustomerName, result.Items[0].CustomerName);
        Assert.AreEqual(pagedResults.Items[0].InvoiceDate, result.Items[0].InvoiceDate);
    }

    [Test]
    public void GetInvoiceSummaries_ReturnsFilteredPagedResults_WithSearchTerm()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10, SearchTerm = "Customer1" };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now.AddDays(-1) }
        };
        var filteredInvoices = invoices.Where(i => i.CustomerName.Contains(listArgs.SearchTerm)).ToList();
        var invoiceSummaries = filteredInvoices.Select(i => new InvoiceSummary { Id = i.Id, CustomerName = i.CustomerName, InvoiceDate = i.InvoiceDate }).ToList();
        var pagedResults = new PagedResults<InvoiceSummary>
        {
            CurrentPage = listArgs.PageNumber,
            PageSize = listArgs.PageSize,
            TotalCount = filteredInvoices.Count,
            Items = invoiceSummaries
        };

        _invoicesRepositoryMock.Setup(repo => repo.GetInvoices()).Returns(invoices.AsQueryable());

        // Act
        var result = _accountingService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(pagedResults.CurrentPage, result.CurrentPage);
        Assert.AreEqual(pagedResults.PageSize, result.PageSize);
        Assert.AreEqual(pagedResults.TotalCount, result.TotalCount);
        Assert.AreEqual(pagedResults.Items.Count, result.Items.Count);
        Assert.AreEqual(pagedResults.Items[0].Id, result.Items[0].Id);
        Assert.AreEqual(pagedResults.Items[0].CustomerName, result.Items[0].CustomerName);
        Assert.AreEqual(pagedResults.Items[0].InvoiceDate, result.Items[0].InvoiceDate);
    }

    [Test]
    public void GetInvoiceSummaries_SortsByInvoiceDateDesc()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10, SearchTerm = null };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now.AddDays(-1) }
        };
        var sortedInvoices = invoices.OrderByDescending(i