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
    public void CalculateInvoiceTotal_WithNonExistentId_ThrowsInvalidOperationException()
    {
        // Arrange
        int nonExistentId = 999;
        _mockDbContext.Setup(db => db.Invoices.Include(It.IsAny<string>()).FirstOrDefault(It.IsAny<Func<Invoice, bool>>()))
            .Returns((Invoice)null);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _accountingService.CalculateInvoiceTotal(nonExistentId));
    }

    [Test]
    public void CalculateInvoiceTotal_WithNegativeId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        int negativeId = -1;
        _mockDbContext.Setup(db => db.Invoices.Include(It.IsAny<string>()).FirstOrDefault(It.IsAny<Func<Invoice, bool>>()))
            .Returns((Invoice)null);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _accountingService.CalculateInvoiceTotal(negativeId));
    }

    [Test]
    public void CalculateInvoiceTotal_WithMaxIntId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        int maxIntId = int.MaxValue;
        _mockDbContext.Setup(db => db.Invoices.Include(It.IsAny<string>()).FirstOrDefault(It.IsAny<Func<Invoice, bool>>()))
            .Returns((Invoice)null);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _accountingService.CalculateInvoiceTotal(maxIntId));
    }

    [Test]
    public void CalculateInvoiceTotal_WithNullInvoice_ReturnsWithoutSavingChanges()
    {
        // Arrange
        int validId = 1;
        var invoice = new Invoice { Id = validId };
        _mockDbContext.Setup(db => db.Invoices.Include(It.IsAny<string>()).FirstOrDefault(It.IsAny<Func<Invoice, bool>>()))
            .Returns(invoice);
        _mockDbContext.Setup(db => db.Entry(It.IsAny<Invoice>())).Returns(new EntityEntry<Invoice>(invoice));

        // Act
        _accountingService.CalculateInvoiceTotal(validId);

        // Assert
        _mockDbContext.Verify(db => db.SaveChanges(), Times.Never());
    }

    [Test]
    public void CalculateInvoiceTotal_WithEmptyLineItems_SetsTotalAmountToZero()
    {
        // Arrange
        int validId = 1;
        var invoice = new Invoice { Id = validId, LineItems = new List<LineItem>() };
        _mockDbContext.Setup(db => db.Invoices.Include(It.IsAny<string>()).FirstOrDefault(It.IsAny<Func<Invoice, bool>>()))
            .Returns(invoice);
        _mockDbContext.Setup(db => db.Entry(It.IsAny<Invoice>())).Returns(new EntityEntry<Invoice>(invoice));

        // Act
        _accountingService.CalculateInvoiceTotal(validId);

        // Assert
        Assert.AreEqual(0, invoice.TotalAmount);
        _mockDbContext.Verify(db => db.SaveChanges(), Times.Once());
    }

    [Test]
    public void CalculateInvoiceTotal_WithValidId_SetsTotalAmount()
    {
        // Arrange
        int validId = 1;
        var lineItem = new LineItem { LineTotal = 100 };
        var invoice = new Invoice { Id = validId, LineItems = new List<LineItem> { lineItem } };
        _mockDbContext.Setup(db => db.Invoices.Include(It.IsAny<string>()).FirstOrDefault(It.IsAny<Func<Invoice, bool>>()))
            .Returns(invoice);
        _mockDbContext.Setup(db => db.Entry(It.IsAny<Invoice>())).Returns(new EntityEntry<Invoice>(invoice));

        // Act
        _accountingService.CalculateInvoiceTotal(validId);

        // Assert
        Assert.AreEqual(100, invoice.TotalAmount);
        _mockDbContext.Verify(db => db.SaveChanges(), Times.Once());
    }
}