using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Sample.Api.Commands;
using Sample.Api.Factories;
using Sample.Api.Models;
using Sample.Api.Services.Accounting.Dto;
using Sample.Infrastructure.Documents;
using System.Collections.Generic;
using System.Linq;

[TestFixture]
public class AccountingServiceTests
{
    private Mock<DbContext> _mockDbContext;
    private Mock<IMapper> _mockMapper;
    private AccountingService _accountingService;

    [SetUp]
    public void Setup()
    {
        _mockDbContext = new Mock<DbContext>();
        _mockMapper = new Mock<IMapper>();

        _accountingService = new AccountingService(_mockDbContext.Object, _mockMapper.Object);
    }

    [Test]
    public void CalculateInvoiceTotal_WithValidId_SetsTotalAmount()
    {
        // Arrange
        int invoiceId = 1;
        var lineItem1 = new LineItem { Id = 1, LineTotal = 100 };
        var lineItem2 = new LineItem { Id = 2, LineTotal = 200 };
        var invoice = new Invoice { Id = invoiceId, LineItems = new List<LineItem> { lineItem1, lineItem2 } };

        _mockDbContext.Setup(dbContext => dbContext.Invoices.Include("LineItems").FirstOrDefault(i => i.Id == invoiceId))
            .Returns(invoice);

        // Act
        _accountingService.CalculateInvoiceTotal(invoiceId);

        // Assert
        Assert.AreEqual(300, invoice.TotalAmount);
        _mockDbContext.Verify(dbContext => dbContext.SaveChanges(), Times.Once());
    }

    [Test]
    public void CalculateInvoiceTotal_WithNonExistentId_DoesNotThrowException()
    {
        // Arrange
        int nonExistentId = 999;

        _mockDbContext.Setup(dbContext => dbContext.Invoices.Include("LineItems").FirstOrDefault(i => i.Id == nonExistentId))
            .Returns((Invoice)null);

        // Act & Assert
        Assert.DoesNotThrow(() => _accountingService.CalculateInvoiceTotal(nonExistentId));
        _mockDbContext.Verify(dbContext => dbContext.SaveChanges(), Times.Never());
    }

    [Test]
    public void CalculateInvoiceTotal_WithEmptyLineItems_SetsTotalAmountToZero()
    {
        // Arrange
        int invoiceId = 1;
        var invoice = new Invoice { Id = invoiceId, LineItems = new List<LineItem>() };

        _mockDbContext.Setup(dbContext => dbContext.Invoices.Include("LineItems").FirstOrDefault(i => i.Id == invoiceId))
            .Returns(invoice);

        // Act
        _accountingService.CalculateInvoiceTotal(invoiceId);

        // Assert
        Assert.AreEqual(0, invoice.TotalAmount);
        _mockDbContext.Verify(dbContext => dbContext.SaveChanges(), Times.Once());
    }
}