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
public class InvoiceServiceTests
{
    private Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private InvoiceService _invoiceService;

    [SetUp]
    public void Setup()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _mapperMock = new Mock<IMapper>();
        _invoiceService = new InvoiceService(_invoiceRepositoryMock.Object, _mapperMock.Object);
    }

    [Test]
    public void GetInvoiceSummaries_ThrowsArgumentNullException_WhenListArgsIsNull()
    {
        // Arrange
        InvoiceListArgs listArgs = null;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _invoiceService.GetInvoiceSummaries(listArgs));
    }

    [Test]
    public void GetInvoiceSummaries_ThrowsArgumentException_WhenPageSizeIsZero()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 0 };

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => _invoiceService.GetInvoiceSummaries(listArgs));
    }

    [Test]
    public void GetInvoiceSummaries_ThrowsArgumentException_WhenPageNumberIsNegative()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = -1, PageSize = 10 };

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => _invoiceService.GetInvoiceSummaries(listArgs));
    }
}