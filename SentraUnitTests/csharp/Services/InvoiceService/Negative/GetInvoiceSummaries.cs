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

public class InvoiceServiceTests
{
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly InvoiceService _invoiceService;

    public InvoiceServiceTests()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _mapperMock = new Mock<IMapper>();
        _invoiceService = new InvoiceService(_invoiceRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public void GetInvoiceSummaries_ThrowsArgumentNullException_WhenListArgsIsNull()
    {
        // Arrange
        InvoiceListArgs listArgs = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _invoiceService.GetInvoiceSummaries(listArgs));
    }

    [Fact]
    public void GetInvoiceSummaries_ThrowsArgumentException_WhenPageSizeIsZero()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 0 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _invoiceService.GetInvoiceSummaries(listArgs));
    }

    [Fact]
    public void GetInvoiceSummaries_ThrowsArgumentException_WhenPageNumberIsNegative()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = -1, PageSize = 10 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _invoiceService.GetInvoiceSummaries(listArgs));
    }
}