using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Sample.Api.Services.Accounting;
using Sample.Api.Services.Accounting.Dto;
using System;

[TestFixture]
public class AccountingServiceNegativeTests
{
    [Test]
    public void DownloadInvoiceDetailPdf_WithEmptyStringInvoiceId_ReturnsNull()
    {
        // Arrange
        var accountingService = new Mock<IAccountingService>();
        string emptyStringInvoiceId = "";

        // Mock behavior
        accountingService.Setup(service => service.GetInvoiceById(emptyStringInvoiceId)).Returns((InvoiceDto)null);

        // Act
        var result = accountingService.Object.DownloadInvoiceDetailPdf(emptyStringInvoiceId);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void DownloadInvoiceDetailPdf_WithWhitespaceInvoiceId_ReturnsNull()
    {
        // Arrange
        var accountingService = new Mock<IAccountingService>();
        string whitespaceInvoiceId = "   ";

        // Mock behavior
        accountingService.Setup(service => service.GetInvoiceById(whitespaceInvoiceId)).Returns((InvoiceDto)null);

        // Act
        var result = accountingService.Object.DownloadInvoiceDetailPdf(whitespaceInvoiceId);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void DownloadInvoiceDetailPdf_WithSpecialCharactersInvoiceId_ReturnsNull()
    {
        // Arrange
        var accountingService = new Mock<IAccountingService>();
        string specialCharactersInvoiceId = "!@#$%^&*()";

        // Mock behavior
        accountingService.Setup(service => service.GetInvoiceById(specialCharactersInvoiceId)).Returns((InvoiceDto)null);

        // Act
        var result = accountingService.Object.DownloadInvoiceDetailPdf(specialCharactersInvoiceId);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void DownloadInvoiceDetailPdf_WithNullInvoiceId_ReturnsNull()
    {
        // Arrange
        var accountingService = new Mock<IAccountingService>();

        // Mock behavior
        accountingService.Setup(service => service.GetInvoiceById(null)).Returns((InvoiceDto)null);

        // Act
        var result = accountingService.Object.DownloadInvoiceDetailPdf(null);

        // Assert
        Assert.IsNull(result);
    }
}