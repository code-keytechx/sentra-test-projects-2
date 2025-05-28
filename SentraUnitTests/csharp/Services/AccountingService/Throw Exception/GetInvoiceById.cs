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
    public void GetInvoiceById_InvalidId_ThrowsArgumentNullException()
    {
        // Arrange
        int invalidId = 0;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _accountingService.GetInvoiceById(invalidId));
    }

    [Test]
    public async Task GetInvoiceById_EmptyDatabase_ThrowsInvalidOperationException()
    {
        // Arrange
        int nonExistentId = 1;
        _mockDbContext.Setup(db => db.Invoices).Returns(new List<Invoice>());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await _accountingService.GetInvoiceById(nonExistentId));
    }

    [Test]
    public async Task GetInvoiceById_IdWithNoMatchingRecord_ThrowsInvalidOperationException()
    {
        // Arrange
        int nonExistentId = 1;
        _mockDbContext.Setup(db => db.Invoices).Returns(new List<Invoice>
        {
            new Invoice { Id = 2, CustomerName = "Customer1", InvoiceDate = DateTime.Now, TotalAmount = 100 }
        });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await _accountingService.GetInvoiceById(nonExistentId));
    }
}