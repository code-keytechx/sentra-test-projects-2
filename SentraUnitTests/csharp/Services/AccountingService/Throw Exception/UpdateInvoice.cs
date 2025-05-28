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
    public void UpdateInvoice_WithInvalidId_ThrowsArgumentException()
    {
        // Arrange
        var dtoInvoice = new DtoUpdateInvoiceInput { CustomerName = "John Doe", InvoiceDate = DateTime.Now, LineItems = new List<DtoInvoiceLineItem>() };
        int invalidId = -1;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => _accountingService.UpdateInvoice(dtoInvoice, invalidId));
    }

    [Test]
    public void UpdateInvoice_WithNonExistentId_ThrowsInvalidOperationException()
    {
        // Arrange
        var dtoInvoice = new DtoUpdateInvoiceInput { CustomerName = "John Doe", InvoiceDate = DateTime.Now, LineItems = new List<DtoInvoiceLineItem>() };
        int nonExistentId = 99999;

        _mockDbContext.Setup(dbContext => dbContext.Invoices.Include("LineItems").FirstOrDefault(It.IsAny<int>())).Returns((Invoice)null);

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(() => _accountingService.UpdateInvoice(dtoInvoice, nonExistentId));
    }

    [Test]
    public void UpdateInvoice_WithNullDtoInvoice_ThrowsArgumentNullException()
    {
        // Arrange
        int validId = 1;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _accountingService.UpdateInvoice(null, validId));
    }
}