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
public class AccountingServiceTests
{
    private Mock<IAccountingService> _accountingServiceMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IDocumentFactory> _documentFactoryMock;

    [SetUp]
    public void Setup()
    {
        _accountingServiceMock = new Mock<IAccountingService>();
        _mapperMock = new Mock<IMapper>();
        _documentFactoryMock = new Mock<IDocumentFactory>();
    }

    [Test]
    public void DownloadInvoiceDetailPdf_WithNegativeInvoiceId_ThrowsArgumentException()
    {
        // Arrange
        int negativeInvoiceId = -1;
        _accountingServiceMock.Setup(service => service.GetInvoiceById(negativeInvoiceId)).Returns((InvoiceDto)null);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => _accountingServiceMock.Object.DownloadInvoiceDetailPdf(negativeInvoiceId));
    }

    [Test]
    public void DownloadInvoiceDetailPdf_WithZeroInvoiceId_ThrowsArgumentException()
    {
        // Arrange
        int zeroInvoiceId = 0;
        _accountingServiceMock.Setup(service => service.GetInvoiceById(zeroInvoiceId)).Returns((InvoiceDto)null);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => _accountingServiceMock.Object.DownloadInvoiceDetailPdf(zeroInvoiceId));
    }

    [Test]
    public void DownloadInvoiceDetailPdf_WithEmptyStringInvoiceId_ThrowsArgumentException()
    {
        // Arrange
        string emptyStringInvoiceId = "";
        _accountingServiceMock.Setup(service => service.GetInvoiceById(emptyStringInvoiceId)).Returns((InvoiceDto)null);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => _accountingServiceMock.Object.DownloadInvoiceDetailPdf(emptyStringInvoiceId));
    }

    [Test]
    public void DownloadInvoiceDetailPdf_WithWhitespaceInvoiceId_ThrowsArgumentException()
    {
        // Arrange
        string whitespaceInvoiceId = " ";
        _accountingServiceMock.Setup(service => service.GetInvoiceById(whitespaceInvoiceId)).Returns((InvoiceDto)null);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => _accountingServiceMock.Object.DownloadInvoiceDetailPdf(whitespaceInvoiceId));
    }

    [Test]
    public void DownloadInvoiceDetailPdf_WithSpecialCharactersInvoiceId_ThrowsArgumentException()
    {
        // Arrange
        string specialCharactersInvoiceId = "!@#$%^&*()";
        _accountingServiceMock.Setup(service => service.GetInvoiceById(specialCharactersInvoiceId)).Returns((InvoiceDto)null);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => _accountingServiceMock.Object.DownloadInvoiceDetailPdf(specialCharactersInvoiceId));
    }
}