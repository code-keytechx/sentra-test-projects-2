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

    [SetUp]
    public void Setup()
    {
        _accountingServiceMock = new Mock<IAccountingService>();
    }

    [Test]
    public void GetInvoiceSummaries_ThrowsArgumentException_WithNullListArgs()
    {
        var listArgs = (InvoiceListArgs)null;

        Assert.ThrowsAsync<ArgumentException>(() => _accountingServiceMock.Object.GetInvoiceSummaries(listArgs));
    }

    [Test]
    public void AddInvoice_ThrowsArgumentNullException_WithNullDtoInvoice()
    {
        DtoAddInvoiceInput dtoInvoice = null;

        Assert.ThrowsAsync<ArgumentNullException>(() => _accountingServiceMock.Object.AddInvoice(dtoInvoice));
    }

    [Test]
    public void DownloadInvoicesCsv_ThrowsArgumentException_WithEmptyInvoiceIdsArray()
    {
        int[] invoiceIds = Array.Empty<int>();

        Assert.ThrowsAsync<ArgumentException>(() => _accountingServiceMock.Object.DownloadInvoicesCsv(invoiceIds));
    }

    [Test]
    public void GetInvoiceById_ThrowsArgumentOutOfRangeException_WithInvalidId()
    {
        int id = -1;

        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _accountingServiceMock.Object.GetInvoiceById(id));
    }

    [Test]
    public void CalculateInvoiceTotal_ThrowsArgumentOutOfRangeException_WithInvalidId()
    {
        int id = -1;

        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _accountingServiceMock.Object.CalculateInvoiceTotal(id));
    }

    [Test]
    public void UpdateInvoice_ThrowsArgumentNullException_WithNullDtoInvoice()
    {
        DtoUpdateInvoiceInput dtoInvoice = null;
        int id = 1;

        Assert.ThrowsAsync<ArgumentNullException>(() => _accountingServiceMock.Object.UpdateInvoice(dtoInvoice, id));
    }

    [Test]
    public void UpdateInvoice_ThrowsArgumentOutOfRangeException_WithInvalidId()
    {
        DtoUpdateInvoiceInput dtoInvoice = new DtoUpdateInvoiceInput();
        int id = -1;

        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _accountingServiceMock.Object.UpdateInvoice(dtoInvoice, id));
    }

    [Test]
    public void DownloadInvoiceDetailPdf_ThrowsArgumentOutOfRangeException_WithInvalidInvoiceId()
    {
        int invoiceId = -1;

        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _accountingServiceMock.Object.DownloadInvoiceDetailPdf(invoiceId));
    }

    [Test]
    public void PreviewInvoiceDetailHtml_ThrowsArgumentOutOfRangeException_WithInvalidInvoiceId()
    {
        int invoiceId = -1;

        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _accountingServiceMock.Object.PreviewInvoiceDetailHtml(invoiceId));
    }
}