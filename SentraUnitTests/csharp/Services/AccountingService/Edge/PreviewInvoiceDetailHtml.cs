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
public class AccountingServiceEdgeTests
{
    [Test]
    public void PreviewInvoiceDetailHtml_WithLargeInvoiceId_ReturnsNotFound()
    {
        // Arrange
        int largeInvoiceId = int.MaxValue;
        var mockMapper = new Mock<IMapper>();
        var mockFactory = new Mock<ICommandFactory>();
        var accountingService = new AccountingService(mockMapper.Object, mockFactory.Object);

        // Act
        IActionResult result = accountingService.PreviewInvoiceDetailHtml(largeInvoiceId);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public void PreviewInvoiceDetailHtml_WithNegativeInvoiceId_ReturnsNotFound()
    {
        // Arrange
        int negativeInvoiceId = -1;
        var mockMapper = new Mock<IMapper>();
        var mockFactory = new Mock<ICommandFactory>();
        var accountingService = new AccountingService(mockMapper.Object, mockFactory.Object);

        // Act
        IActionResult result = accountingService.PreviewInvoiceDetailHtml(negativeInvoiceId);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public void PreviewInvoiceDetailHtml_WithZeroInvoiceId_ReturnsNotFound()
    {
        // Arrange
        int zeroInvoiceId = 0;
        var mockMapper = new Mock<IMapper>();
        var mockFactory = new Mock<ICommandFactory>();
        var accountingService = new AccountingService(mockMapper.Object, mockFactory.Object);

        // Act
        IActionResult result = accountingService.PreviewInvoiceDetailHtml(zeroInvoiceId);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }
}