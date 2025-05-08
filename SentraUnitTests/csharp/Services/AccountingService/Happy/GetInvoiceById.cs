using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sample.Api.Commands;
using Sample.Api.Factories;
using Sample.Api.Models;
using Sample.Api.Services.Accounting.Dto;
using Sample.Infrastructure.Documents;
using System;
using Xunit;

public class AccountingServiceTests
{
    [Fact]
    public void GetInvoiceById_ReturnsCorrectInvoiceDetails()
    {
        // Arrange
        var mockDbContext = new Mock<IInvoicesContext>();
        var mockMapper = new Mock<IMapper>();

        var invoiceId = 1;
        var expectedInvoice = new InvoiceDetailViewModel
        {
            Id = invoiceId,
            CustomerName = "John Doe",
            InvoiceDate = DateTime.Now,
            TotalAmount = 100.00m,
            LineItems = new List<InvoiceLineItemViewModel>
            {
                new InvoiceLineItemViewModel { Id = 1, Description = "Item 1", Quantity = 1, UnitPrice = 50.00m, LineTotal = 50.00m },
                new InvoiceLineItemViewModel { Id = 2, Description = "Item 2", Quantity = 2, UnitPrice = 25.00m, LineTotal = 50.00m }
            }
        };

        mockDbContext.Setup(db => db.Invoices.Where(i => i.Id == invoiceId))
            .Returns(new[] { new InvoiceDto { Id = invoiceId, CustomerName = "John Doe", InvoiceDate = DateTime.Now, TotalAmount = 100.00m } });

        mockMapper.Setup(mapper => mapper.Map<InvoiceDetailViewModel>(It.IsAny<InvoiceDto>()))
            .Returns(expectedInvoice);

        var accountingService = new AccountingService(mockDbContext.Object, mockMapper.Object);

        // Act
        var result = accountingService.GetInvoiceById(invoiceId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedInvoice.Id, result.Id);
        Assert.Equal(expectedInvoice.CustomerName, result.CustomerName);
        Assert.Equal(expectedInvoice.InvoiceDate, result.InvoiceDate);
        Assert.Equal(expectedInvoice.TotalAmount, result.TotalAmount);
        Assert.Collection(result.LineItems,
            item1 => 
            {
                Assert.Equal(1, item1.Id);
                Assert.Equal("Item 1", item1.Description);
                Assert.Equal(1, item1.Quantity);
                Assert.Equal(50.00m, item1.UnitPrice);
                Assert.Equal(50.00m, item1.LineTotal);
            },
            item2 => 
            {
                Assert.Equal(2, item2.Id);
                Assert.Equal("Item 2", item2.Description);
                Assert.Equal(2, item2.Quantity);
                Assert.Equal(25.00m, item2.UnitPrice);
                Assert.Equal(50.00m, item2.LineTotal);
            });
    }

    [Fact]
    public void GetInvoiceById_ReturnsNullForNonExistentInvoice()
    {
        // Arrange
        var mockDbContext = new Mock<IInvoicesContext>();
        var mockMapper = new Mock<IMapper>();

        var invoiceId = 1;

        mockDbContext.Setup(db => db.Invoices.Where(i => i.Id == invoiceId)).Returns(new InvoiceDto[0]);

        var accountingService = new AccountingService(mockDbContext.Object, mockMapper.Object);

        // Act
        var result = accountingService.GetInvoiceById(invoiceId);

        // Assert
        Assert.Null(result);
    }
}