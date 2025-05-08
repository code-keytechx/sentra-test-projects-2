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
        // Arrange
        var controller = new YourController(_accountingServiceMock.Object);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => controller.GetInvoiceSummaries(null));
    }

    [Test]
    public void AddInvoice_ThrowsArgumentNullException_WithNullDtoInvoice()
    {
        // Arrange
        var controller = new YourController(_accountingServiceMock.Object);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => controller.AddInvoice(null));
    }

    [Test]
    public void DownloadInvoicesCsv_ThrowsArgumentException_WithEmptyInvoiceIdsArray()
    {
        // Arrange
        var controller = new YourController(_accountingServiceMock.Object);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => controller.DownloadInvoicesCsv(new int[0]));
    }

    [Test]
    public void GetInvoiceById_ThrowsArgumentOutOfRangeException_WithInvalidId()
    {
        // Arrange
        var controller = new YourController(_accountingServiceMock.Object);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => controller.GetInvoiceById(-1));
    }

    [Test]
    public void CalculateInvoiceTotal_ThrowsArgumentOutOfRangeException_WithInvalidId()
    {
        // Arrange
        var controller = new YourController(_accountingServiceMock.Object);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => controller.CalculateInvoiceTotal(-1));
    }

    [Test]
    public void UpdateInvoice_ThrowsArgumentNullException_WithNullDtoInvoice()
    {
        // Arrange
        var controller = new YourController(_accountingServiceMock.Object);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => controller.UpdateInvoice(null, 1));
    }

    [Test]
    public void UpdateInvoice_ThrowsArgumentOutOfRangeException_WithInvalidId()
    {
        // Arrange
        var controller = new YourController(_accountingServiceMock.Object);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => controller.UpdateInvoice(new DtoUpdateInvoiceInput(), -1));
    }

    [Test]
    public void DownloadInvoiceDetailPdf_ThrowsArgumentOutOfRangeException_WithInvalidInvoiceId()
    {
        // Arrange
        var controller = new YourController(_accountingServiceMock.Object);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => controller.DownloadInvoiceDetailPdf(-1));
    }

    [Test]
    public void PreviewInvoiceDetailHtml_ThrowsArgumentOutOfRangeException_WithInvalidInvoiceId()
    {
        // Arrange
        var controller = new YourController(_accountingServiceMock.Object);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => controller.PreviewInvoiceDetailHtml(-1));
    }
}