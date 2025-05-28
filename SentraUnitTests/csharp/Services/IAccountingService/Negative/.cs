using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Sample.Api.Models;
using Sample.Api.Services.Accounting;
using Sample.Api.Services.Accounting.Dto;
using System.Collections.Generic;

[TestFixture]
public class AccountingServiceNegativeTests
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
        var ex = Assert.ThrowsAsync<ArgumentException>(() => _accountingServiceMock.Object.GetInvoiceSummaries(null));
        Assert.AreEqual("listArgs", ex.ParamName);
    }

    [Test]
    public void AddInvoice_ThrowsArgumentNullException_WithNullDtoInvoice()
    {
        var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _accountingServiceMock.Object.AddInvoice(null));
        Assert.AreEqual("dtoInvoice", ex.ParamName);
    }

    [Test]
    public void DownloadInvoicesCsv_ThrowsArgumentOutOfRangeException_WithEmptyInvoiceIdsArray()
    {
        var ex = Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _accountingServiceMock.Object.DownloadInvoicesCsv(new int[0]));
        Assert.AreEqual("invoiceIds", ex.ParamName);
    }

    [Test]
    public void GetInvoiceById_ThrowsArgumentException_WithInvalidId()
    {
        var ex = Assert.ThrowsAsync<ArgumentException>(() => _accountingServiceMock.Object.GetInvoiceById(-1));
        Assert.AreEqual("id", ex.ParamName);
    }

    [Test]
    public void CalculateInvoiceTotal_ThrowsArgumentException_WithInvalidId()
    {
        var ex = Assert.ThrowsAsync<ArgumentException>(() => _accountingServiceMock.Object.CalculateInvoiceTotal(-1));
        Assert.AreEqual("id", ex.ParamName);
    }

    [Test]
    public void UpdateInvoice_ThrowsArgumentNullException_WithNullDtoInvoice()
    {
        var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _accountingServiceMock.Object.UpdateInvoice(null, 1));
        Assert.AreEqual("dtoInvoice", ex.ParamName);
    }

    [Test]
    public void UpdateInvoice_ThrowsArgumentException_WithInvalidId()
    {
        var ex = Assert.ThrowsAsync<ArgumentException>(() => _accountingServiceMock.Object.UpdateInvoice(new DtoUpdateInvoiceInput(), -1));
        Assert.AreEqual("id", ex.ParamName);
    }

    [Test]
    public void DownloadInvoiceDetailPdf_ThrowsArgumentException_WithInvalidId()
    {
        var ex = Assert.ThrowsAsync<ArgumentException>(() => _accountingServiceMock.Object.DownloadInvoiceDetailPdf(-1));
        Assert.AreEqual("invoiceId", ex.ParamName);
    }

    [Test]
    public void PreviewInvoiceDetailHtml_ThrowsArgumentException_WithInvalidId()
    {
        var ex = Assert.ThrowsAsync<ArgumentException>(() => _accountingServiceMock.Object.PreviewInvoiceDetailHtml(-1));
        Assert.AreEqual("invoiceId", ex.ParamName);
    }
}