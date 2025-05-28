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
    public void UpdateInvoice_WithValidInputs_ReturnsUpdatedInvoiceDetailViewModel()
    {
        // Arrange
        var dtoInvoice = new DtoUpdateInvoiceInput
        {
            CustomerName = "John Doe",
            InvoiceDate = DateTime.Now,
            LineItems = new List<DtoInvoiceLineItem>
            {
                new DtoInvoiceLineItem { Description = "Item 1", Quantity = 2, UnitPrice = 10 },
                new DtoInvoiceLineItem { Description = "Item 2", Quantity = 1, UnitPrice = 5 }
            }
        };

        var id = 1;
        var invoice = new Invoice
        {
            Id = id,
            CustomerName = "Old Name",
            InvoiceDate = DateTime.Now.AddDays(-1),
            LineItems = new List<InvoiceLineItem>()
        };

        _mockDbContext.Setup(db => db.Invoices.Include("LineItems").FirstOrDefault(i => i.Id == id)).Returns(invoice);

        var expectedViewModel = new InvoiceDetailViewModel
        {
            Id = id,
            CustomerName = dtoInvoice.CustomerName,
            InvoiceDate = dtoInvoice.InvoiceDate,
            TotalAmount = dtoInvoice.LineItems.Sum(li => li.Quantity * li.UnitPrice),
            LineItems = dtoInvoice.LineItems.Select(li => new InvoiceLineItemViewModel
            {
                Id = Guid.NewGuid(),
                Description = li.Description,
                Quantity = li.Quantity,
                UnitPrice = li.UnitPrice,
                LineTotal = li.Quantity * li.UnitPrice
            }).ToList()
        };

        _mockMapper.Setup(mapper => mapper.Map<InvoiceDetailViewModel>(invoice)).Returns(expectedViewModel);

        // Act
        var result = _accountingService.UpdateInvoice(dtoInvoice, id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedViewModel.Id, result.Id);
        Assert.AreEqual(expectedViewModel.CustomerName, result.CustomerName);
        Assert.AreEqual(expectedViewModel.InvoiceDate, result.InvoiceDate);
        Assert.AreEqual(expectedViewModel.TotalAmount, result.TotalAmount);
        Assert.AreEqual(expectedViewModel.LineItems.Count, result.LineItems.Count);
    }

    [Test]
    public void UpdateInvoice_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var dtoInvoice = new DtoUpdateInvoiceInput
        {
            CustomerName = "John Doe",
            InvoiceDate = DateTime.Now,
            LineItems = new List<DtoInvoiceLineItem>
            {
                new DtoInvoiceLineItem { Description = "Item 1", Quantity = 2, UnitPrice = 10 },
                new DtoInvoiceLineItem { Description = "Item 2", Quantity = 1, UnitPrice = 5 }
            }
        };

        var id = 1;
        _mockDbContext.Setup(db => db.Invoices.Include("LineItems").FirstOrDefault(i => i.Id == id)).Returns((Invoice)null);

        // Act
        var result = _accountingService.UpdateInvoice(dtoInvoice, id);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void UpdateInvoice_WithEmptyLineItems_ClearsExistingLineItemsAndAddsNewOnes()
    {
        // Arrange
        var dtoInvoice = new DtoUpdateInvoiceInput
        {
            CustomerName = "John Doe",
            InvoiceDate = DateTime.Now,
            LineItems = new List<DtoInvoiceLineItem>()
        };

        var id = 1;
        var invoice = new Invoice
        {
            Id = id,
            CustomerName = "Old Name",
            InvoiceDate = DateTime.Now.AddDays(-1),
            LineItems = new List<InvoiceLineItem>
            {
                new InvoiceLineItem { Id = Guid.NewGuid(), Description = "Old Item", Quantity = 1, UnitPrice = 10 }
            }
        };

        _mockDbContext.Setup(db => db.Invoices.Include("LineItems").FirstOrDefault(i => i.Id == id)).Returns(invoice);

        // Act
        _accountingService.UpdateInvoice(dtoInvoice, id);

        // Assert
        Assert.IsEmpty(invoice.LineItems);
    }
}