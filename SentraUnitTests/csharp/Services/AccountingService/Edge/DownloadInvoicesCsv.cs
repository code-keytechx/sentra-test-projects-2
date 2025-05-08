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
using System.Collections.Generic;
using System.Linq;

[TestFixture]
public class AccountingServiceEdgeTests
{
    private Mock<SampleDbContext> _mockDbContext;
    private Mock<IMapper> _mockMapper;
    private AccountingService _accountingService;

    [SetUp]
    public void Setup()
    {
        _mockDbContext = new Mock<SampleDbContext>();
        _mockMapper = new Mock<IMapper>();
        _accountingService = new AccountingService(_mockDbContext.Object, _mockMapper.Object);
    }

    [Test]
    public void DownloadInvoicesCsv_ReturnsEmptyFileResult_WhenNoInvoicesFound()
    {
        // Arrange
        int[] invoiceIds = { 1, 2, 3 };
        _mockDbContext.Setup(db => db.Invoices.Where(It.IsAny<Expression<Func<Invoice, bool>>>())).Returns(new List<Invoice>());

        // Act
        var result = _accountingService.DownloadInvoicesCsv(invoiceIds);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("text/csv", result.ContentType);
        Assert.AreEqual(0, result.FileContents.Length);
        Assert.AreEqual($"Invoices_{DateTime.UtcNow:yyyyMMddHHmmss}.csv", result.FileDownloadName);
    }

    [Test]
    public void DownloadInvoicesCsv_ThrowsArgumentNullException_WhenInvoiceIdsIsNull()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => _accountingService.DownloadInvoicesCsv(null));
    }

    [Test]
    public void DownloadInvoicesCsv_ThrowsArgumentOutOfRangeException_WhenInvoiceIdsIsEmpty()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _accountingService.DownloadInvoicesCsv(new int[0]));
    }
}