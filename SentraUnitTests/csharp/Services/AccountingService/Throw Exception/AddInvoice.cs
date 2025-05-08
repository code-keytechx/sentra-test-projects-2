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
    public void AddInvoice_WithNullDtoInvoice_ThrowsArgumentNullException()
    {
        // Arrange
        DtoAddInvoiceInput nullDtoInvoice = null;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _accountingService.AddInvoice(nullDtoInvoice));
    }

    [Test]
    public void AddInvoice_WithEmptyCustomerName_ThrowsArgumentException()
    {
        // Arrange
        var dtoInvoice = new DtoAddInvoiceInput
        {
            CustomerName = string.Empty,
            InvoiceDate = DateTime.Now,
            LineItems = new List<DtoInvoiceLineItem>()
        };

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => _accountingService.AddInvoice(dtoInvoice));
    }

    [Test]
    public void AddInvoice_WithInvalidInvoiceDate_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var dtoInvoice = new DtoAddInvoiceInput
        {
            CustomerName = "John Doe",
            InvoiceDate = DateTime.MinValue,
            LineItems = new List<DtoInvoiceLineItem>()
        };

        // Act & Assert
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _accountingService.AddInvoice(dtoInvoice));
    }
}