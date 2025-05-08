using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sample.Api.Models;
using Sample.Api.Services.Accounting;
using System;

[TestClass]
public class AccountingServiceEdgeTests
{
    [TestMethod]
    public void GeneratePdf_WithBoundaryMinInvoiceDate_ReturnsEmptyByteArray()
    {
        // Arrange
        var invoice = new InvoiceDetailViewModel { InvoiceDate = DateTime.MinValue };
        var accountingService = new Mock<IAccountingService>();

        // Act
        byte[] result = accountingService.Object.GeneratePdf(invoice);

        // Assert
        Assert.AreEqual(0, result.Length);
    }

    [TestMethod]
    public void GeneratePdf_WithBoundaryMaxInvoiceDate_ReturnsEmptyByteArray()
    {
        // Arrange
        var invoice = new InvoiceDetailViewModel { InvoiceDate = DateTime.MaxValue };
        var accountingService = new Mock<IAccountingService>();

        // Act
        byte[] result = accountingService.Object.GeneratePdf(invoice);

        // Assert
        Assert.AreEqual(0, result.Length);
    }

    [TestMethod]
    public void GeneratePdf_WithNullInvoiceDate_ReturnsEmptyByteArray()
    {
        // Arrange
        var invoice = new InvoiceDetailViewModel { InvoiceDate = null };
        var accountingService = new Mock<IAccountingService>();

        // Act
        byte[] result = accountingService.Object.GeneratePdf(invoice);

        // Assert
        Assert.AreEqual(0, result.Length);
    }
}