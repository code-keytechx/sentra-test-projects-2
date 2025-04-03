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
    public async Task GetInvoiceSummaries_ThrowsArgumentException_WhenListArgsIsNull()
    {
        // Arrange
        InvoiceListArgs listArgs = null;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => _invoiceService.GetInvoiceSummaries(listArgs));
    }

    [Test]
    public async Task GetInvoiceSummaries_ThrowsArgumentNullException_WhenDbContextIsNotInitialized()
    {
        // Arrange
        _invoiceRepositoryMock.Setup(repo => repo.GetAll()).Returns(Task.FromResult<IEnumerable<Invoice>>(null));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(() => _invoiceService.GetInvoiceSummaries(new InvoiceListArgs()));
    }
}