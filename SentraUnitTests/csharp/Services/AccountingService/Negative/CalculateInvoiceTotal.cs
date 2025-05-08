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
    public void CalculateInvoiceTotal_WithInvalidId_ThrowsArgumentException()
    {
        // Arrange
        var mockDbContext = new Mock<IInvoicesContext>();
        var mockMapper = new Mock<IMapper>();
        var accountingService = new AccountingService(mockDbContext.Object, mockMapper.Object);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => accountingService.CalculateInvoiceTotal(-1));
    }

    [Fact]
    public void CalculateInvoiceTotal_WithNullInvoice_ReturnsWithoutSavingChanges()
    {
        // Arrange
        var mockDbContext = new Mock<IInvoicesContext>();
        mockDbContext.Setup(db => db.Invoices.Include(It.IsAny<string>())).Returns(new List<Invoice>());
        var mockMapper = new Mock<IMapper>();
        var accountingService = new AccountingService(mockDbContext.Object, mockMapper.Object);

        // Act
        accountingService.CalculateInvoiceTotal(1);

        // Assert
        mockDbContext.Verify(db => db.SaveChanges(), Times.Never());
    }

    [Fact]
    public void CalculateInvoiceTotal_WithEmptyLineItems_SetsTotalAmountToZero()
    {
        // Arrange
        var mockDbContext = new Mock<IInvoicesContext>();
        var mockMapper = new Mock<IMapper>();
        var invoice = new Invoice { Id = 1, LineItems = new List<LineItem>() };
        mockDbContext.Setup(db => db.Invoices.Include(It.IsAny<string>())).Returns(new List<Invoice> { invoice });
        var accountingService = new AccountingService(mockDbContext.Object, mockMapper.Object);

        // Act
        accountingService.CalculateInvoiceTotal(1);

        // Assert
        Assert.Equal(0, invoice.TotalAmount);
    }
}