using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Sample.Api.Services.Accounting;
using Sample.Api.Services.Accounting.Dto;
using System.Collections.Generic;
using System.Linq;

[TestFixture]
public class AccountingServiceNegativeTests
{
    private Mock<SampleDbContext> _mockDbContext;
    private AccountingService _accountingService;

    [SetUp]
    public void Setup()
    {
        _mockDbContext = new Mock<SampleDbContext>();
        _accountingService = new AccountingService(_mockDbContext.Object);
    }

    [Test]
    public void DownloadInvoicesCsv_ThrowsArgumentNullException_WhenInvoiceIdsIsNull()
    {
        // Arrange
        int[] invoiceIds = null;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _accountingService.DownloadInvoicesCsv(invoiceIds));
    }

    [Test]
    public void DownloadInvoicesCsv_ThrowsArgumentOutOfRangeException_WhenInvoiceIdsIsEmpty()
    {
        // Arrange
        int[] invoiceIds = Array.Empty<int>();

        // Act & Assert
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _accountingService.DownloadInvoicesCsv(invoiceIds));
    }

    [Test]
    public void DownloadInvoicesCsv_ReturnsNull_WhenDatabaseContextIsNull()
    {
        // Arrange
        _mockDbContext.SetupGet(db => db.Invoices).Returns(null as IQueryable<Invoice>);
        int[] invoiceIds = { 1, 2, 3 };

        // Act
        var result = _accountingService.DownloadInvoicesCsv(invoiceIds).Result;

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void DownloadInvoicesCsv_ThrowsInvalidOperationException_WhenInvoiceIdsContainNonExistentInvoices()
    {
        // Arrange
        int[] invoiceIds = { 1, 2, 3 };
        _mockDbContext.Setup(db => db.Invoices.Where(It.IsAny<Func<Invoice, bool>>())).Returns(new List<Invoice>());

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(() => _accountingService.DownloadInvoicesCsv(invoiceIds));
    }
}