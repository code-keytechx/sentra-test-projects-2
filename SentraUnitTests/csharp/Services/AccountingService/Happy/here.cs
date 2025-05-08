using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sample.Api.Models;
using Sample.Api.Services.Accounting;
using Sample.Api.Services.Accounting.Dto;
using System;

[TestClass]
public class AccountingServiceTests_Happy
{
    [TestMethod]
    public void GeneratePdf_WithValidInvoice_ReturnsEmptyByteArray()
    {
        // Arrange
        var invoice = new InvoiceDetailViewModel { /* Initialize with valid data */ };
        var accountingService = new Mock<IAccountingService>();

        // Act
        var result = accountingService.Object.GeneratePdf(invoice);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Length);
    }

    [TestMethod]
    public void GeneratePdf_WithNullInvoice_ReturnsEmptyByteArray()
    {
        // Arrange
        var invoice = null;
        var accountingService = new Mock<IAccountingService>();

        // Act
        var result = accountingService.Object.GeneratePdf(invoice);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Length);
    }

    [TestMethod]
    public void GeneratePdf_WithEmptyInvoice_ReturnsEmptyByteArray()
    {
        // Arrange
        var invoice = new InvoiceDetailViewModel(); // Empty object
        var accountingService = new Mock<IAccountingService>();

        // Act
        var result = accountingService.Object.GeneratePdf(invoice);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Length);
    }
}