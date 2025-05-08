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
    [Test]
    public void PreviewInvoiceDetailHtml_WithLargeInvoiceId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentStore = new Mock<IDocumentStore>();
        var accountingService = new AccountingService(mockMapper.Object, mockDocumentStore.Object);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => accountingService.PreviewInvoiceDetailHtml(int.MaxValue));
    }

    [Test]
    public void PreviewInvoiceDetailHtml_WithNegativeInvoiceId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentStore = new Mock<IDocumentStore>();
        var accountingService = new AccountingService(mockMapper.Object, mockDocumentStore.Object);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => accountingService.PreviewInvoiceDetailHtml(-1));
    }

    [Test]
    public void PreviewInvoiceDetailHtml_WithNonExistentInvoiceId_ThrowsNotFoundException()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentStore = new Mock<IDocumentStore>();
        mockDocumentStore.Setup(ds => ds.GetInvoiceById(It.IsAny<int>())).Returns((InvoiceDto)null);
        var accountingService = new AccountingService(mockMapper.Object, mockDocumentStore.Object);

        // Act & Assert
        Assert.Throws<NotFoundException>(() => accountingService.PreviewInvoiceDetailHtml(1));
    }
}