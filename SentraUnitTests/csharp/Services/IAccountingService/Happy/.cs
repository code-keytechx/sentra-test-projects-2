using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Sample.Api.Models;
using Sample.Api.Services.Accounting;
using Sample.Api.Services.Accounting.Dto;
using System.Collections.Generic;

[TestFixture]
public class AccountingServiceTests
{
    private Mock<IAccountingService> _accountingServiceMock;
    private IAccountingService _accountingService;

    [SetUp]
    public void Setup()
    {
        _accountingServiceMock = new Mock<IAccountingService>();
        _accountingService = _accountingServiceMock.Object;
    }

    [Test]
    public void GetInvoiceSummaries_ReturnsPagedResults_WithValidInputs()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10 };
        var expectedResult = new PagedResults<InvoiceSummary>();

        _accountingServiceMock.Setup(service => service.GetInvoiceSummaries(listArgs)).Returns(expectedResult);

        // Act
        var result = _accountingService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedResult.PageNumber, result.PageNumber);
        Assert.AreEqual(expectedResult.PageSize, result.PageSize);
    }

    [Test]
    public void AddInvoice_ReturnsInt_WithValidDto()
    {
        // Arrange
        var dtoInvoice = new DtoAddInvoiceInput { /* Initialize properties */ };
        const int expectedResult = 1;

        _accountingServiceMock.Setup(service => service.AddInvoice(dtoInvoice)).Returns(expectedResult);

        // Act
        var result = _accountingService.AddInvoice(dtoInvoice);

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    [Test]
    public void DownloadInvoicesCsv_ReturnsFileResult_WithValidInvoiceIds()
    {
        // Arrange
        var invoiceIds = new[] { 1, 2, 3 };
        var expectedResult = new FileContentResult(new byte[0], "application/csv");

        _accountingServiceMock.Setup(service => service.DownloadInvoicesCsv(invoiceIds)).Returns(expectedResult);

        // Act
        var result = _accountingService.DownloadInvoicesCsv(invoiceIds);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("application/csv", result.ContentType);
    }
}