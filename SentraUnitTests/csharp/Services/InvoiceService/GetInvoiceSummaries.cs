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
using System.Threading.Tasks;
using Xunit;

public class InvoiceServiceTests
{
    private const int PageNumber = 1;
    private const int PageSize = 10;
    private const string SearchTerm = "Customer";

    private readonly Mock<IInvoiceRepository> _mockInvoiceRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly InvoiceService _invoiceService;

    public InvoiceServiceTests()
    {
        _mockInvoiceRepository = new Mock<IInvoiceRepository>();
        _mockMapper = new Mock<IMapper>();

        _invoiceService = new InvoiceService(
            _mockInvoiceRepository.Object,
            _mockMapper.Object
        );
    }

    #region Happy Path Tests

    [Fact]
    public async Task GetInvoiceSummaries_WithValidSearchTerm_ReturnsPagedResults()
    {
        // Arrange
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "CustomerA", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "PAID", ExportingBy = "Admin" },
            new Invoice { Id = 2, CustomerName = "CustomerB", InvoiceDate = DateTime.Now, TotalAmount = 200, Status = "UNPAID", ExportingBy = "Admin" }
        };

        var invoiceSummaries = invoices.Select(i => new InvoiceSummary
        {
            Id = i.Id,
            CustomerName = i.CustomerName,
            InvoiceDate = i.InvoiceDate,
            TotalAmount = i.TotalAmount,
            Status = i.Status,
            ExportingBy = i.ExportingBy
        }).ToList();

        _mockInvoiceRepository.Setup(repo => repo.GetInvoices()).ReturnsAsync(invoices);
        _mockMapper.Setup(mapper => mapper.Map<List<InvoiceSummary>>(invoices)).Returns(invoiceSummaries);

        var listArgs = new InvoiceListArgs
        {
            SearchTerm = SearchTerm,
            PageNumber = PageNumber,
            PageSize = PageSize
        };

        // Act
        var result = await _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PageNumber, result.CurrentPage);
        Assert.Equal(PageSize, result.PageSize);
        Assert.Equal(invoiceSummaries.Count, result.Items.Count);
        Assert.Equal(invoiceSummaries.First().CustomerName, result.Items.First().CustomerName);
    }

    [Fact]
    public async Task GetInvoiceSummaries_WithoutSearchTerm_ReturnsPagedResults()
    {
        // Arrange
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "CustomerA", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "PAID", ExportingBy = "Admin" },
            new Invoice { Id = 2, CustomerName = "CustomerB", InvoiceDate = DateTime.Now, TotalAmount = 200, Status = "UNPAID", ExportingBy = "Admin" }
        };

        var invoiceSummaries = invoices.Select(i => new InvoiceSummary
        {
            Id = i.Id,
            CustomerName = i.CustomerName,
            InvoiceDate = i.InvoiceDate,
            TotalAmount = i.TotalAmount,
            Status = i.Status,
            ExportingBy = i.ExportingBy
        }).ToList();

        _mockInvoiceRepository.Setup(repo => repo.GetInvoices()).ReturnsAsync(invoices);
        _mockMapper.Setup(mapper => mapper.Map<List<InvoiceSummary>>(invoices)).Returns(invoiceSummaries);

        var listArgs = new InvoiceListArgs
        {
            PageNumber = PageNumber,
            PageSize = PageSize
        };

        // Act
        var result = await _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PageNumber, result.CurrentPage);
        Assert.Equal(PageSize, result.PageSize);
        Assert.Equal(invoiceSummaries.Count, result.Items.Count);
        Assert.Equal(invoiceSummaries.First().CustomerName, result.Items.First().CustomerName);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public async Task GetInvoiceSummaries_WithEmptySearchTerm_ReturnsAllItems()
    {
        // Arrange
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "CustomerA", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "PAID", ExportingBy = "Admin" },
            new Invoice { Id = 2, CustomerName = "CustomerB", InvoiceDate = DateTime.Now, TotalAmount = 200, Status = "UNPAID", ExportingBy = "Admin" }
        };

        var invoiceSummaries = invoices.Select(i => new InvoiceSummary
        {
            Id = i.Id,
            CustomerName = i.CustomerName,
            InvoiceDate = i.InvoiceDate,
            TotalAmount = i.TotalAmount,
            Status = i.Status,
            ExportingBy = i.ExportingBy
        }).ToList();

        _mockInvoiceRepository.Setup(repo => repo.GetInvoices()).ReturnsAsync(invoices);
        _mockMapper.Setup(mapper => mapper.Map<List<InvoiceSummary>>(invoices)).Returns(invoiceSummaries);

        var listArgs = new InvoiceListArgs
        {
            SearchTerm = "",
            PageNumber = PageNumber,
            PageSize = PageSize
        };

        // Act
        var result = await _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PageNumber, result.CurrentPage);
        Assert.Equal(PageSize, result.PageSize);
        Assert.Equal(invoiceSummaries.Count, result.Items.Count);
        Assert.Equal(invoiceSummaries.First().CustomerName, result.Items.First().CustomerName);
    }

    [Fact]
    public async Task GetInvoiceSummaries_WithLargePageNumber_ReturnsEmptyResults()
    {
        // Arrange
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "CustomerA", InvoiceDate = DateTime.Now, TotalAmount = 100, Status = "PAID", ExportingBy = "Admin" },
            new Invoice { Id = 2, CustomerName = "CustomerB", InvoiceDate = DateTime.Now, TotalAmount = 200, Status = "UNPAID", ExportingBy = "Admin" }
        };

        var invoiceSummaries = invoices.Select(i => new InvoiceSummary
        {
            Id = i.Id,
            CustomerName = i.CustomerName,
            InvoiceDate = i.InvoiceDate,
            TotalAmount = i.TotalAmount,
            Status = i.Status,
            ExportingBy = i.ExportingBy
        }).ToList();

        _mockInvoiceRepository.Setup(repo => repo.GetInvoices()).ReturnsAsync(invoices);
        _mockMapper.Setup(mapper => mapper.Map<List<InvoiceSummary>>(invoices)).Returns(invoiceSummaries);

        var listArgs = new InvoiceListArgs
        {
            SearchTerm = SearchTerm,
            PageNumber = 100,
            PageSize = PageSize
        };

        // Act
        var result = await _invoiceService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100, result.CurrentPage);
        Assert.Equal(PageSize, result.PageSize);
        Assert.Empty(result.Items);
    }

    #endregion

    #region Negative Tests

    [Fact]
    public async Task GetInvoiceSummaries_WithNullSearchTerm_ReturnsArgumentNullException()
    {
        // Arrange
        var listArgs = new InvoiceListArgs
        {
            SearchTerm = null,
            PageNumber = PageNumber,
            PageSize = PageSize
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _invoiceService.GetInvoiceSummaries(listArgs));
    }

    [Fact]
    public async Task GetInvoiceSummaries_WithZeroPageSize_ReturnsArgumentException()
    {
        // Arrange
        var listArgs = new InvoiceListArgs
        {
            SearchTerm = SearchTerm,
            PageNumber = PageNumber,
            PageSize = 0
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _invoiceService.GetInvoiceSummaries(listArgs));
    }

    #endregion

    #region Exception Tests

    [Fact]
    public async Task GetInvoiceSummaries_WhenDatabaseThrowsException_ThrowsBusinessException()
    {
        // Arrange
        _mockInvoiceRepository.Setup(repo => repo.GetInvoices()).ThrowsAsync(new InvalidOperationException("Database error"));

        var listArgs = new InvoiceListArgs
        {
            SearchTerm = SearchTerm,
            PageNumber = PageNumber,
            PageSize = PageSize
        };

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _invoiceService.GetInvoiceSummaries(listArgs));
    }

    #endregion

    #region Helper Methods

    private static InvoiceListArgs CreateValidListArgs(string searchTerm, int pageNumber, int pageSize)
    {
        return new InvoiceListArgs
        {
            SearchTerm = searchTerm,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    #endregion
}