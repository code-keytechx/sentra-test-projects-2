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
    public void DownloadInvoicesCsv_ThrowsInvalidOperationException_WhenInvoiceIdsContainNonExistentInvoices()
    {
        // Arrange
        int[] invoiceIds = { 1, 2, 3 }; // Assuming 1, 2, and 3 are non-existent IDs in the database
        _mockDbContext.Setup(db => db.Invoices.Where(i => invoiceIds.Contains(i.Id)).ToList()).Returns(new List<Invoice>());

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(() => _accountingService.DownloadInvoicesCsv(invoiceIds));
    }
}