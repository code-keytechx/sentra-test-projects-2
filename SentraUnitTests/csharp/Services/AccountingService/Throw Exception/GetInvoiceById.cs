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
    public void GetInvoiceById_InvalidId_ThrowsArgumentException()
    {
        // Arrange
        int invalidId = -1;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => _accountingService.GetInvoiceById(invalidId));
    }

    [Test]
    public async Task GetInvoiceById_IdDoesNotExist_ReturnsNull()
    {
        // Arrange
        int nonExistentId = 999;
        _mockDbContext.Setup(db => db.Invoices.Where(It.IsAny<Func<Invoice, bool>>()))
                       .Returns(new List<Invoice>());

        // Act
        var result = await _accountingService.GetInvoiceById(nonExistentId);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public async Task GetInvoiceById_NullId_ThrowsArgumentNullException()
    {
        // Arrange
        int? nullId = null;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _accountingService.GetInvoiceById(nullId.Value));
    }
}