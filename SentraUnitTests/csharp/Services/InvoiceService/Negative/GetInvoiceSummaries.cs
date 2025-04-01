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
using System.Threading.Tasks;

[TestFixture]
public class InvoiceServiceNegativeTests
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
    public async Task GetInvoiceSummaries_WithNullListArgs_ShouldThrowArgumentNullException()
    {
        // Arrange
        InvoiceListArgs listArgs = null;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _invoiceService.GetInvoiceSummaries(listArgs));
    }

    [Test]
    public async Task GetInvoiceSummaries_WithInvalidPageSize_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 0 };

        // Act & Assert
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _invoiceService.GetInvoiceSummaries(listArgs));
    }

    [Test]
    public async Task GetInvoiceSummaries_WithNegativePageNumber_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = -1, PageSize = 10 };

        // Act & Assert
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _invoiceService.GetInvoiceSummaries(listArgs));
    }
}