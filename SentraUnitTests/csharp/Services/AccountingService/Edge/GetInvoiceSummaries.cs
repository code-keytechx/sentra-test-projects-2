using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Sample.Api.Commands;
using Sample.Api.Factories;
using Sample.Api.Models;
using Sample.Api.Services.Accounting.Dto;
using Sample.Infrastructure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class AccountingServiceTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IInvoicesRepository> _repositoryMock;
    private readonly AccountingService _accountingService;

    public AccountingServiceTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryMock = new Mock<IInvoicesRepository>();
        _accountingService = new AccountingService(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public void GetInvoiceSummaries_ThrowsArgumentNullException_WhenListArgsIsNull()
    {
        // Arrange
        InvoiceListArgs listArgs = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _accountingService.GetInvoiceSummaries(listArgs));
    }

    [Fact]
    public void GetInvoiceSummaries_ThrowsArgumentException_WhenPageSizeIsZero()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 0 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _accountingService.GetInvoiceSummaries(listArgs));
    }

    [Fact]
    public void GetInvoiceSummaries_ThrowsArgumentException_WhenPageNumberIsLessThanOne()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 0, PageSize = 10 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _accountingService.GetInvoiceSummaries(listArgs));
    }

    [Fact]
    public void GetInvoiceSummaries_ReturnsFilteredPagedResults_WithSearchTerm()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10, SearchTerm = "Customer" };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Parse("2023-01-01") },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Parse("2023-01-02") }
        };
        _repositoryMock.Setup(repo => repo.GetInvoices()).Returns(invoices);

        // Act
        var result = _accountingService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.All(result.Items, item => item.CustomerName.Contains(listArgs.SearchTerm));
    }

    [Fact]
    public void GetInvoiceSummaries_PaginatesCorrectly_WithMultiplePages()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 2, PageSize = 1 };
        var invoices = new List<Invoice>
        {
            new Invoice { Id = 1, CustomerName = "Customer1", InvoiceDate = DateTime.Parse("2023-01-01") },
            new Invoice { Id = 2, CustomerName = "Customer2", InvoiceDate = DateTime.Parse("2023-01-02") }
        };
        _repositoryMock.Setup(repo => repo.GetInvoices()).Returns(invoices);

        // Act
        var result = _accountingService.GetInvoiceSummaries(listArgs);

        // Assert
        Assert.Equal(2, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal(2, result.Items.First().Id);
    }
}