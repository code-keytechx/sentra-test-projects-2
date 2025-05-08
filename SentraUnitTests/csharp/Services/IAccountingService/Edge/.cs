using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Sample.Api.Models;
using Sample.Api.Services.Accounting;
using Sample.Api.Services.Accounting.Dto;
using System.Collections.Generic;

[TestFixture]
public class AccountingServiceEdgeTests
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
    public void GetInvoiceSummaries_ThrowsArgumentException_WithNullListArgs()
    {
        var listArgs = (InvoiceListArgs)null;

        Assert.ThrowsAsync<ArgumentException>(() => _accountingService.GetInvoiceSummaries(listArgs));
    }

    [Test]
    public void DownloadInvoicesCsv_ThrowsArgumentException_WithEmptyInvoiceIdsArray()
    {
        var invoiceIds = new int[0];

        Assert.ThrowsAsync<ArgumentException>(() => _accountingService.DownloadInvoicesCsv(invoiceIds));
    }

    [Test]
    public void GetInvoiceById_ThrowsArgumentOutOfRangeException_WithInvalidId()
    {
        var id = -1;

        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _accountingService.GetInvoiceById(id));
    }

    [Test]
    public void CalculateInvoiceTotal_ThrowsArgumentOutOfRangeException_WithInvalidId()
    {
        var id = -1;

        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _accountingService.CalculateInvoiceTotal(id));
    }

    [Test]
    public void UpdateInvoice_ThrowsArgumentNullException_WithNullDtoInvoice()
    {
        var dtoInvoice = (DtoUpdateInvoiceInput)null;
        var id = 1;

        Assert.ThrowsAsync<ArgumentNullException>(() => _accountingService.UpdateInvoice(dtoInvoice, id));
    }

    [Test]
    public void UpdateInvoice_ThrowsArgumentOutOfRangeException_WithInvalidId()
    {
        var dtoInvoice = new DtoUpdateInvoiceInput();
        var id = -1;

        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _accountingService.UpdateInvoice(dtoInvoice, id));
    }

    [Test]
    public void DownloadInvoiceDetailPdf_ThrowsArgumentOutOfRangeException_WithInvalidInvoiceId()
    {
        var invoiceId = -1;

        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _accountingService.DownloadInvoiceDetailPdf(invoiceId));
    }

    [Test]
    public void PreviewInvoiceDetailHtml_ThrowsArgumentOutOfRangeException_WithInvalidInvoiceId()
    {
        var invoiceId = -1;

        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _accountingService.PreviewInvoiceDetailHtml(invoiceId));
    }
}