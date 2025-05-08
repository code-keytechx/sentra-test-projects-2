using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
public class AccountingServiceNegativeTests
{
    private Mock<IRepository<Invoice>> _invoiceRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private AccountingService _accountingService;

    [SetUp]
    public void Setup()
    {
        _invoiceRepositoryMock = new Mock<IRepository<Invoice>>();
        _mapperMock = new Mock<IMapper>();
        _accountingService = new AccountingService(_invoiceRepositoryMock.Object, _mapperMock.Object);
    }

    [Test]
    public void GetInvoiceSummaries_ThrowsArgumentNullException_WhenListArgsIsNull()
    {
        // Arrange
        InvoiceListArgs listArgs = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _accountingService.GetInvoiceSummaries(listArgs));
    }

    [Test]
    public void GetInvoiceSummaries_ThrowsArgumentException_WhenPageSizeIsZero()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 0 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _accountingService.GetInvoiceSummaries(listArgs));
    }

    [Test]
    public void GetInvoiceSummaries_ThrowsArgumentException_WhenPageNumberIsLessThanOne()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 0, PageSize = 10 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _accountingService.GetInvoiceSummaries(listArgs));
    }
}