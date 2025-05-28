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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[TestFixture]
public class AccountingServiceTests
{
    private Mock<IMapper> _mapperMock;
    private Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private AccountingService _accountingService;

    [SetUp]
    public void Setup()
    {
        _mapperMock = new Mock<IMapper>();
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _accountingService = new AccountingService(_mapperMock.Object, _invoiceRepositoryMock.Object);
    }

    [Test]
    public void GetInvoiceSummaries_ThrowsArgumentNullException_WhenListArgsIsNull()
    {
        // Arrange
        InvoiceListArgs listArgs = null;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _accountingService.GetInvoiceSummaries(listArgs));
    }

    [Test]
    public void GetInvoiceSummaries_ThrowsArgumentException_WhenPageSizeIsZero()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 0 };

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => _accountingService.GetInvoiceSummaries(listArgs));
    }

    [Test]
    public void GetInvoiceSummaries_ThrowsArgumentException_WhenPageNumberIsLessThanOne()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 0, PageSize = 10 };

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => _accountingService.GetInvoiceSummaries(listArgs));
    }
}