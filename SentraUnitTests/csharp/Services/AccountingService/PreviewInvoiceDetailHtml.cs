using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sample.Api.Commands;
using Sample.Api.Factories;
using Sample.Api.Models;
using Sample.Api.Services.Accounting.Dto;
using Sample.Infrastructure.Documents;
using System;
using Xunit;

public class AccountingServiceTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IInvoiceDocumentStore> _documentStoreMock;
    private readonly AccountingService _accountingService;

    public AccountingServiceTests()
    {
        _mapperMock = new Mock<IMapper>();
        _documentStoreMock = new Mock<IInvoiceDocumentStore>();
        _accountingService = new AccountingService(_mapperMock.Object, _documentStoreMock.Object);
    }

    #region Happy Path Tests

    [Fact]
    public async Task PreviewInvoiceDetailHtml_WithExistingInvoice_ReturnsHtmlContent()
    {
        // Arrange
        int invoiceId = 123;
        var invoiceDto = new InvoiceDto
        {
            Id = invoiceId,
            CustomerName = "John Doe",
            InvoiceDate = DateTime.UtcNow.Date,
            TotalAmount = 100.00m
        };

        _documentStoreMock.Setup(ds => ds.GetAsync(invoiceId)).ReturnsAsync(invoiceDto);

        // Act
        var result = await _accountingService.PreviewInvoiceDetailHtml(invoiceId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ContentResult>(result);
        Assert.Equal("text/html", result.ContentType);
        Assert.Contains($"<h1>Invoice #{invoiceId}</h1>", result.Content);
        Assert.Contains($"<p>Customer: John Doe</p>", result.Content);
        Assert.Contains($"<p>Date: {DateTime.UtcNow.Date.ToString("yyyy-MM-dd")}", result.Content);
        Assert.Contains($"<p>Total: 100.00</p>", result.Content);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public async Task PreviewInvoiceDetailHtml_WithEmptyInvoiceId_ReturnsNotFound()
    {
        // Arrange
        int invoiceId = 0;

        // Act
        var result = await _accountingService.PreviewInvoiceDetailHtml(invoiceId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task PreviewInvoiceDetailHtml_WithNullInvoice_ReturnsNotFound()
    {
        // Arrange
        int invoiceId = 123;
        _documentStoreMock.Setup(ds => ds.GetAsync(invoiceId)).ReturnsAsync((InvoiceDto)null);

        // Act
        var result = await _accountingService.PreviewInvoiceDetailHtml(invoiceId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundObjectResult>(result);
    }

    #endregion

    #region Negative Tests

    [Fact]
    public async Task PreviewInvoiceDetailHtml_WithNegativeInvoiceId_ReturnsBadRequest()
    {
        // Arrange
        int invoiceId = -1;

        // Act
        var result = await _accountingService.PreviewInvoiceDetailHtml(invoiceId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
    }

    #endregion

    #region Exception Tests

    [Fact]
    public async Task PreviewInvoiceDetailHtml_WhenDocumentStoreThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        int invoiceId = 123;
        _documentStoreMock.Setup(ds => ds.GetAsync(invoiceId)).ThrowsAsync(new InvalidOperationException("Database error"));

        // Act
        var result = await _accountingService.PreviewInvoiceDetailHtml(invoiceId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(500, ((StatusCodeResult)result).StatusCode);
    }

    #endregion
}