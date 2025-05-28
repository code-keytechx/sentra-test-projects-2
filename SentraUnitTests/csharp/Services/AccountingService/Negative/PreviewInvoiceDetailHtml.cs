using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

[TestFixture]
public class AccountingServiceTests
{
    [Test]
    public void PreviewInvoiceDetailHtml_WithNonExistentInvoice_ReturnsNotFound()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentFactory = new Mock<IDocumentFactory>();

        mockDocumentFactory.Setup(factory => factory.GetInvoiceById(It.IsAny<int>())).Returns((InvoiceDto)null);

        var accountingService = new AccountingService(mockMapper.Object, mockDocumentFactory.Object);

        // Act
        var result = accountingService.PreviewInvoiceDetailHtml(99999);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public void PreviewInvoiceDetailHtml_WithLargeInvoiceId_ReturnsNotFound()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentFactory = new Mock<IDocumentFactory>();

        mockDocumentFactory.Setup(factory => factory.GetInvoiceById(It.IsAny<int>())).Returns((InvoiceDto)null);

        var accountingService = new AccountingService(mockMapper.Object, mockDocumentFactory.Object);

        // Act
        var result = accountingService.PreviewInvoiceDetailHtml(int.MaxValue);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public void PreviewInvoiceDetailHtml_WithNegativeInvoiceId_ReturnsNotFound()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentFactory = new Mock<IDocumentFactory>();

        mockDocumentFactory.Setup(factory => factory.GetInvoiceById(It.IsAny<int>())).Returns((InvoiceDto)null);

        var accountingService = new AccountingService(mockMapper.Object, mockDocumentFactory.Object);

        // Act
        var result = accountingService.PreviewInvoiceDetailHtml(-1000);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public void PreviewInvoiceDetailHtml_WithZeroInvoiceId_ReturnsNotFound()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentFactory = new Mock<IDocumentFactory>();

        mockDocumentFactory.Setup(factory => factory.GetInvoiceById(It.IsAny<int>())).Returns((InvoiceDto)null);

        var accountingService = new AccountingService(mockMapper.Object, mockDocumentFactory.Object);

        // Act
        var result = accountingService.PreviewInvoiceDetailHtml(0);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public void PreviewInvoiceDetailHtml_WithInvoiceHavingNoTotalAmount_ReturnsHtmlContent()
    {
        // Arrange
        var mockMapper = new Mock<IMapper>();
        var mockDocumentFactory = new Mock<IDocumentFactory>();

        var invoiceDto = new InvoiceDto
        {
            Id = 1,
            CustomerName = "John Doe",
            InvoiceDate = DateTime.Now,
            TotalAmount = null
        };

        mockDocumentFactory.Setup(factory => factory.GetInvoiceById(It.IsAny<int>())).Returns(invoiceDto);

        var accountingService = new AccountingService(mockMapper.Object, mockDocumentFactory.Object);

        // Act
        var result = accountingService.PreviewInvoiceDetailHtml(1);

        // Assert
        Assert.IsInstanceOf<ContentResult>(result);
        var contentResult = result as ContentResult;
        Assert.AreEqual("text/html", contentResult.ContentType);
        Assert.Contains("<h1>Invoice #1</h1>", contentResult.Content);
        Assert.Contains("<p>Customer: John Doe</p>", contentResult.Content);
        Assert.Contains("<p>Date: ", contentResult.Content);
        Assert.Contains("<p>Total: </p>", contentResult.Content);
    }
}