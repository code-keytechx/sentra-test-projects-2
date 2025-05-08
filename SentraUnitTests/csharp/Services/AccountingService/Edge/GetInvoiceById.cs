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
    public void GetInvoiceById_InvalidId_ReturnsNull()
    {
        // Arrange
        var mockDbContext = new Mock<IInvoicesDocument>();
        var mapperMock = new Mock<IMapper>();

        var accountingService = new AccountingService(mockDbContext.Object, mapperMock.Object);

        // Act
        var result = accountingService.GetInvoiceById(-1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetInvoiceById_IdDoesNotExist_ReturnsNull()
    {
        // Arrange
        var mockDbContext = new Mock<IInvoicesDocument>();
        mockDbContext.Setup(db => db.Invoices).Returns(new List<Invoice>());

        var mapperMock = new Mock<IMapper>();

        var accountingService = new AccountingService(mockDbContext.Object, mapperMock.Object);

        // Act
        var result = accountingService.GetInvoiceById(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetInvoiceById_NullId_ThrowsArgumentNullException()
    {
        // Arrange
        var mockDbContext = new Mock<IInvoicesDocument>();
        var mapperMock = new Mock<IMapper>();

        var accountingService = new AccountingService(mockDbContext.Object, mapperMock.Object);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => accountingService.GetInvoiceById(default));
    }
}