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
        var accountingService = new Mock<IAccountingService>();
        int largeInvoiceId = int.MaxValue;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => accountingService.Object.PreviewInvoiceDetailHtml(largeInvoiceId));
    }

    [Test]
    public void PreviewInvoiceDetailHtml_WithNegativeInvoiceId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var accountingService = new Mock<IAccountingService>();
        int negativeInvoiceId = -1;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => accountingService.Object.PreviewInvoiceDetailHtml(negativeInvoiceId));
    }

    [Test]
    public void PreviewInvoiceDetailHtml_WithZeroInvoiceId_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var accountingService = new Mock<IAccountingService>();
        int zeroInvoiceId = 0;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => accountingService.Object.PreviewInvoiceDetailHtml(zeroInvoiceId));
    }
}