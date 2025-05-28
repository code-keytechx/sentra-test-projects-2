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
    public void AddInvoice_WithValidInput_ReturnsNewInvoiceId()
    {
        // Arrange
        var dtoInvoice = new DtoAddInvoiceInput
        {
            CustomerName = "John Doe",
            InvoiceDate = DateTime.Now,
            LineItems = new List<DtoAddInvoiceLineItem>
            {
                new DtoAddInvoiceLineItem { Description = "Item 1", Quantity = 2, UnitPrice = 10.0m }
            }
        };

        var invoice = new Invoice
        {
            Id = 1,
            CustomerName = dtoInvoice.CustomerName,
            InvoiceDate = dtoInvoice.InvoiceDate,
            TotalAmount = 20.0m,
            LineItems = new List<InvoiceLineItem>
            {
                new InvoiceLineItem { Description = "Item 1", Quantity = 2, UnitPrice = 10.0m, LineTotal = 20.0m }
            }
        };

        _mockMapper.Setup(m => m.Map<Invoice>(dtoInvoice)).Returns(invoice);

        // Act
        var result = _accountingService.AddInvoice(dtoInvoice);

        // Assert
        Assert.AreEqual(1, result);
        _mockDbContext.Verify(db => db.Invoices.Add(invoice), Times.Once);
        _mockDbContext.Verify(db => db.SaveChanges(), Times.Once);
    }

    [Test]
    public void AddInvoice_WithNoLineItems_ReturnsNewInvoiceId()
    {
        // Arrange
        var dtoInvoice = new DtoAddInvoiceInput
        {
            CustomerName = "Jane Smith",
            InvoiceDate = DateTime.Now,
            LineItems = new List<DtoAddInvoiceLineItem>()
        };

        var invoice = new Invoice
        {
            Id = 2,
            CustomerName = dtoInvoice.CustomerName,
            InvoiceDate = dtoInvoice.InvoiceDate,
            TotalAmount = 0.0m,
            LineItems = new List<InvoiceLineItem>()
        };

        _mockMapper.Setup(m => m.Map<Invoice>(dtoInvoice)).Returns(invoice);

        // Act
        var result = _accountingService.AddInvoice(dtoInvoice);

        // Assert
        Assert.AreEqual(2, result);
        _mockDbContext.Verify(db => db.Invoices.Add(invoice), Times.Once);
        _mockDbContext.Verify(db => db.SaveChanges(), Times.Once);
    }

    [Test]
    public void AddInvoice_WithMultipleLineItems_ReturnsNewInvoiceId()
    {
        // Arrange
        var dtoInvoice = new DtoAddInvoiceInput
        {
            CustomerName = "Alice Johnson",
            InvoiceDate = DateTime.Now,
            LineItems = new List<DtoAddInvoiceLineItem>
            {
                new DtoAddInvoiceLineItem { Description = "Item 1", Quantity = 3, UnitPrice = 15.0m },
                new DtoAddInvoiceLineItem { Description = "Item 2", Quantity = 1, UnitPrice = 25.0m }
            }
        };

        var invoice = new Invoice
        {
            Id = 3,
            CustomerName = dtoInvoice.CustomerName,
            InvoiceDate = dtoInvoice.InvoiceDate,
            TotalAmount = 70.0m,
            LineItems = new List<InvoiceLineItem>
            {
                new InvoiceLineItem { Description = "Item 1", Quantity = 3, UnitPrice = 15.0m, LineTotal = 45.0m },
                new InvoiceLineItem { Description = "Item 2", Quantity = 1, UnitPrice = 25.0m, LineTotal = 25.0m }
            }
        };

        _mockMapper.Setup(m => m.Map<Invoice>(dtoInvoice)).Returns(invoice);

        // Act
        var result = _accountingService.AddInvoice(dtoInvoice);

        // Assert
        Assert.AreEqual(3, result);
        _mockDbContext.Verify(db => db.Invoices.Add(invoice), Times.Once);
        _mockDbContext.Verify(db => db.SaveChanges(), Times.Once);
    }
}