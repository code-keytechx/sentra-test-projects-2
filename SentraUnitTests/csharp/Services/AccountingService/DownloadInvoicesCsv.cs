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

public class AccountingServiceTests
{
    private readonly Mock<IInvoiceDocumentStore> _mockInvoiceDocumentStore;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AccountingService _accountingService;

    public AccountingServiceTests()
    {
        _mockInvoiceDocumentStore = new Mock<IInvoiceDocumentStore>();
        _mockMapper = new Mock<IMapper>();
        _accountingService = new AccountingService(_mockInvoiceDocumentStore.Object, _mockMapper.Object);
    }

    #region Happy Path Tests

    [Fact]
    public async Task DownloadInvoicesCsv_WithValidInvoiceIds_ReturnsFileResult()
    {
        // Arrange
        var invoiceIds = new int[] { 1, 2, 3 };
        var invoices = new List<InvoiceDto>
        {
            new InvoiceDto { Id = 1, CustomerName = "John Doe", InvoiceDate = DateTime.Now, TotalAmount = 100.00M },
            new InvoiceDto { Id = 2, CustomerName = "Jane Smith", InvoiceDate = DateTime.Now, TotalAmount = 200.00M },
            new InvoiceDto { Id = 3, CustomerName = "Alice Johnson", InvoiceDate = DateTime.Now, TotalAmount = 300.00M }
        };

        _mockInvoiceDocumentStore.Setup(store => store.GetInvoicesByIdsAsync(invoiceIds))
            .ReturnsAsync(invoices);

        // Act
        var result = await _accountingService.DownloadInvoicesCsv(invoiceIds);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/csv", result.ContentType);
        Assert.StartsWith("Invoices_", result.FileDownloadName);
        Assert.EndsWith(".csv", result.FileDownloadName);
        Assert.NotEmpty(result.FileContents);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public async Task DownloadInvoicesCsv_WithNoMatchingInvoices_ReturnsEmptyFile()
    {
        // Arrange
        var invoiceIds = new int[] { 1, 2, 3 };
        var invoices = new List<InvoiceDto>();

        _mockInvoiceDocumentStore.Setup(store => store.GetInvoicesByIdsAsync(invoiceIds))
            .ReturnsAsync(invoices);

        // Act
        var result = await _accountingService.DownloadInvoicesCsv(invoiceIds);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/csv", result.ContentType);
        Assert.StartsWith("Invoices_", result.FileDownloadName);
        Assert.EndsWith(".csv", result.FileDownloadName);
        Assert.Empty(result.FileContents);
    }

    [Fact]
    public async Task DownloadInvoicesCsv_WithSingleInvoice_ReturnsSingleLineCsv()
    {
        // Arrange
        var invoiceIds = new int[] { 1 };
        var invoices = new List<InvoiceDto>
        {
            new InvoiceDto { Id = 1, CustomerName = "John Doe", InvoiceDate = DateTime.Now, TotalAmount = 100.00M }
        };

        _mockInvoiceDocumentStore.Setup(store => store.GetInvoicesByIdsAsync(invoiceIds))
            .ReturnsAsync(invoices);

        // Act
        var result = await _accountingService.DownloadInvoicesCsv(invoiceIds);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/csv", result.ContentType);
        Assert.StartsWith("Invoices_", result.FileDownloadName);
        Assert.EndsWith(".csv", result.FileDownloadName);
        Assert.Single(result.FileContents.Split(Environment.NewLine));
    }

    #endregion

    #region Negative Tests

    [Fact]
    public async Task DownloadInvoicesCsv_WithNullInvoiceIds_ThrowsArgumentNullException()
    {
        // Arrange
        int[] invoiceIds = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _accountingService.DownloadInvoicesCsv(invoiceIds));
    }

    [Fact]
    public async Task DownloadInvoicesCsv_WithEmptyInvoiceIds_ReturnsEmptyFile()
    {
        // Arrange
        var invoiceIds = new int[] { };

        // Act
        var result = await _accountingService.DownloadInvoicesCsv(invoiceIds);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/csv", result.ContentType);
        Assert.StartsWith("Invoices_", result.FileDownloadName);
        Assert.EndsWith(".csv", result.FileDownloadName);
        Assert.Empty(result.FileContents);
    }

    #endregion

    #region Exception Tests

    [Fact]
    public async Task DownloadInvoicesCsv_WhenDatabaseFetchFails_ThrowsInvalidOperationException()
    {
        // Arrange
        var invoiceIds = new int[] { 1, 2, 3 };

        _mockInvoiceDocumentStore.Setup(store => store.GetInvoicesByIdsAsync(invoiceIds))
            .ThrowsAsync(new InvalidOperationException("Database fetch failed"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _accountingService.DownloadInvoicesCsv(invoiceIds));
    }

    #endregion
}