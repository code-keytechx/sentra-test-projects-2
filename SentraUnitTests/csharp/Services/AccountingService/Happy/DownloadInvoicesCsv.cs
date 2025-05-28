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
public class AccountingServiceTests
{
    private Mock<IAccountingDbContext> _mockDbContext;
    private Mock<IMapper> _mockMapper;
    private AccountingService _accountingService;

    [SetUp]
    public void Setup()
    {
        _mockDbContext = new Mock<IAccountingDbContext>();
        _mockMapper = new Mock<IMapper>();
        _accountingService = new AccountingService(_mockDbContext.Object, _mockMapper.Object);
    }

    [Test]
    public void DownloadInvoicesCsv_ReturnsFileResult_WithCorrectCSVData()
    {
        // Arrange
        int[] invoiceIds = { 1, 2, 3 };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Now, TotalAmount = 100.00m },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Now, TotalAmount = 200.00m },
            new Invoice { Id = 3, CustomerName = "Customer3", InvoiceDate = DateTime.Now, TotalAmount = 300.00m }
        };

        _mockDbContext.Setup(db => db.Invoices.Where(i => invoiceIds.Contains(i.Id)).ToList()).Returns(invoices);

        // Act
        var result = _accountingService.DownloadInvoicesCsv(invoiceIds);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("text/csv", result.ContentType);
        Assert.IsTrue(result.FileDownloadName.StartsWith("Invoices_"));
        Assert.AreEqual(3, ((string)result.FileContents).Split(new[] { Environment.NewLine }, StringSplitOptions.None).Length - 1); // Subtract header row
    }

    [Test]
    public void DownloadInvoicesCsv_ReturnsEmptyFileResult_WhenNoInvoicesFound()
    {
        // Arrange
        int[] invoiceIds = { 1, 2, 3 };
        _mockDbContext.Setup(db => db.Invoices.Where(i => invoiceIds.Contains(i.Id)).ToList()).Returns(new List<Invoice>());

        // Act
        var result = _accountingService.DownloadInvoicesCsv(invoiceIds);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("text/csv", result.ContentType);
        Assert.IsTrue(result.FileDownloadName.StartsWith("Invoices_"));
        Assert.AreEqual(1, ((string)result.FileContents).Split(new[] { Environment.NewLine }, StringSplitOptions.None).Length); // Only header row
    }
}