using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sample.Api.Models;
using Sample.Api.Services.Accounting;
using System;

[TestClass]
public class AccountingServiceEdgeTests
{
    [TestMethod]
    public void GeneratePdf_WithBoundaryMinInvoiceDate_ReturnsNonEmptyByteArray()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentFactory = new Mock<IDocumentFactory>();

        var invoice = new InvoiceDetailViewModel
        {
            InvoiceDate = DateTime.MinValue,
            DueDate = DateTime.Now.AddDays(30),
            TotalAmount = 100m,
            Currency = "USD",
            Items = new List<InvoiceItemViewModel> { new InvoiceItemViewModel { Description = "Item 1", Quantity = 1, Price = 100m } }
        };

        var accountingService = new AccountingService(mockMapper.Object, mockDocumentFactory.Object);

        // Act
        byte[] result = accountingService.GeneratePdf(invoice);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length > 0);
    }

    [TestMethod]
    public void GeneratePdf_WithBoundaryMaxInvoiceDate_ReturnsNonEmptyByteArray()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentFactory = new Mock<IDocumentFactory>();

        var invoice = new InvoiceDetailViewModel
        {
            InvoiceDate = DateTime.MaxValue,
            DueDate = DateTime.Now.AddDays(30),
            TotalAmount = 100m,
            Currency = "USD",
            Items = new List<InvoiceItemViewModel> { new InvoiceItemViewModel { Description = "Item 1", Quantity = 1, Price = 100m } }
        };

        var accountingService = new AccountingService(mockMapper.Object, mockDocumentFactory.Object);

        // Act
        byte[] result = accountingService.GeneratePdf(invoice);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length > 0);
    }

    [TestMethod]
    public void GeneratePdf_WithBoundaryMaxDueDate_ReturnsNonEmptyByteArray()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentFactory = new Mock<IDocumentFactory>();

        var invoice = new InvoiceDetailViewModel
        {
            InvoiceDate = DateTime.Now,
            DueDate = DateTime.MaxValue,
            TotalAmount = 100m,
            Currency = "USD",
            Items = new List<InvoiceItemViewModel> { new InvoiceItemViewModel { Description = "Item 1", Quantity = 1, Price = 100m } }
        };

        var accountingService = new AccountingService(mockMapper.Object, mockDocumentFactory.Object);

        // Act
        byte[] result = accountingService.GeneratePdf(invoice);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length > 0);
    }

    [TestMethod]
    public void GeneratePdf_WithZeroTotalAmount_ReturnsNonEmptyByteArray()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentFactory = new Mock<IDocumentFactory>();

        var invoice = new InvoiceDetailViewModel
        {
            InvoiceDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(30),
            TotalAmount = 0m,
            Currency = "USD",
            Items = new List<InvoiceItemViewModel> { new InvoiceItemViewModel { Description = "Item 1", Quantity = 1, Price = 0m } }
        };

        var accountingService = new AccountingService(mockMapper.Object, mockDocumentFactory.Object);

        // Act
        byte[] result = accountingService.GeneratePdf(invoice);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length > 0);
    }

    [TestMethod]
    public void GeneratePdf_WithNegativeTotalAmount_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentFactory = new Mock<IDocumentFactory>();

        var invoice = new InvoiceDetailViewModel
        {
            InvoiceDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(30),
            TotalAmount = -100m,
            Currency = "USD",
            Items = new List<InvoiceItemViewModel> { new InvoiceItemViewModel { Description = "Item 1", Quantity = 1, Price = 100m } }
        };

        var accountingService = new AccountingService(mockMapper.Object, mockDocumentFactory.Object);

        // Act & Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => accountingService.GeneratePdf(invoice));
    }
}