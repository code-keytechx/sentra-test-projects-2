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

[TestFixture]
public class AccountingServiceTests
{
    [Test]
    public void PreviewInvoiceDetailHtml_WithValidInvoice_ReturnsHtmlContent()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentRepository = new Mock<IDocumentRepository>();

        var accountingService = new AccountingService(mockMapper.Object, mockDocumentRepository.Object);

        var invoiceId = 1;
        var invoice = new Invoice
        {
            Id = invoiceId,
            CustomerName = "John Doe",
            InvoiceDate = DateTime.Now,
            TotalAmount = 100.00m
        };

        mockDocumentRepository.Setup(repo => repo.GetInvoiceById(invoiceId)).Returns(invoice);

        // Act
        var result = accountingService.PreviewInvoiceDetailHtml(invoiceId);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<ContentResult>(result);
        var contentResult = result as ContentResult;
        Assert.AreEqual("text/html", contentResult.ContentType);
        Assert.IsTrue(contentResult.Content.Contains($"Invoice #{invoiceId}"));
        Assert.IsTrue(contentResult.Content.Contains($"Customer: {invoice.CustomerName}"));
        Assert.IsTrue(contentResult.Content.Contains($"Date: {invoice.InvoiceDate.ToString("yyyy-MM-dd")}"));
        Assert.IsTrue(contentResult.Content.Contains($"Total: {invoice.TotalAmount:F2}"));
    }

    [Test]
    public void PreviewInvoiceDetailHtml_WithNonExistentInvoice_ReturnsNotFound()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentRepository = new Mock<IDocumentRepository>();

        var accountingService = new AccountingService(mockMapper.Object, mockDocumentRepository.Object);

        var invoiceId = 1;

        mockDocumentRepository.Setup(repo => repo.GetInvoiceById(invoiceId)).Returns((Invoice)null);

        // Act
        var result = accountingService.PreviewInvoiceDetailHtml(invoiceId);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<NotFoundObjectResult>(result);
    }
}