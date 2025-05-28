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
            CustomerName = "Test Customer",
            InvoiceDate = DateTime.Now,
            TotalAmount = 100.00m,
            LineItems = new List<InvoiceLineItemViewModel>
            {
                new InvoiceLineItemViewModel
                {
                    Id = 1,
                    Description = "Item 1",
                    Quantity = 2,
                    UnitPrice = 50.00m,
                    LineTotal = 100.00m
                }
            }
        };

        mockDbContext.Setup(db => db.Invoices.Where(i => i.Id == invoiceId))
            .Returns(new[] { new InvoiceDto { Id = invoiceId, CustomerName = "Test Customer", InvoiceDate = DateTime.Now, TotalAmount = 100.00m } });

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
        Assert.Single(result.LineItems);
        Assert.Equal(expectedInvoice.LineItems[0].Id, result.LineItems[0].Id);
        Assert.Equal(expectedInvoice.LineItems[0].Description, result.LineItems[0].Description);
        Assert.Equal(expectedInvoice.LineItems[0].Quantity, result.LineItems[0].Quantity);
        Assert.Equal(expectedInvoice.LineItems[0].UnitPrice, result.LineItems[0].UnitPrice);
        Assert.Equal(expectedInvoice.LineItems[0].LineTotal, result.LineItems[0].LineTotal);
    }

    [Fact]
    public void GetInvoiceById_ReturnsNullForNonExistentInvoice()
    {
        // Arrange
        var mockDbContext = new Mock<IInvoicesContext>();
        var mockMapper = new Mock<IMapper>();

        var invoiceId = 1;

        mockDbContext.Setup(db => db.Invoices.Where(i => i.Id == invoiceId)).Returns(new InvoiceDto[] { });

        var accountingService = new AccountingService(mockDbContext.Object, mockMapper.Object);

        // Act
        var result = accountingService.GetInvoiceById(invoiceId);

        // Assert
        Assert.Null(result);
    }
}