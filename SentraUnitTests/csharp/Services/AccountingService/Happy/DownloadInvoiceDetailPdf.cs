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
using System.IO;

[TestFixture]
public class AccountingServiceTests
{
    private Mock<IAccountingRepository> _repositoryMock;
    private Mock<IMapper> _mapperMock;
    private AccountingService _accountingService;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IAccountingRepository>();
        _mapperMock = new Mock<IMapper>();
        _accountingService = new AccountingService(_repositoryMock.Object, _mapperMock.Object);
    }

    [Test]
    public void DownloadInvoiceDetailPdf_WithValidInvoiceId_ReturnsFileResult()
    {
        // Arrange
        int invoiceId = 123;
        var invoiceDto = new InvoiceDto { Id = invoiceId };
        var pdfBytes = new byte[0]; // Simulated PDF bytes

        _repositoryMock.Setup(repo => repo.GetInvoiceById(invoiceId)).Returns(invoiceDto);
        _mapperMock.Setup(mapper => mapper.Map<InvoiceDto>(invoiceDto)).Returns(new InvoiceModel());
        _repositoryMock.Setup(repo => repo.GeneratePdf(It.IsAny<InvoiceDto>())).Returns(pdfBytes);

        // Act
        var result = _accountingService.DownloadInvoiceDetailPdf(invoiceId);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<FileContentResult>(result);
        Assert.AreEqual("application/pdf", result.ContentType);
        Assert.AreEqual($"Invoice_{invoiceId}.pdf", result.FileDownloadName);
        Assert.AreEqual(pdfBytes.Length, result.FileContents.Length);
    }

    [Test]
    public void DownloadInvoiceDetailPdf_WithNonExistentInvoiceId_ReturnsNull()
    {
        // Arrange
        int invoiceId = 456;
        _repositoryMock.Setup(repo => repo.GetInvoiceById(invoiceId)).Returns((InvoiceDto)null);

        // Act
        var result = _accountingService.DownloadInvoiceDetailPdf(invoiceId);

        // Assert
        Assert.IsNull(result);
    }
}