using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sample.Api.Commands;
using Sample.Api.Factories;
using Sample.Api.Models;
using Sample.Api.Services.Accounting.Dto;
using Sample.Infrastructure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static Sample.Api.Models.InvoiceListArgs;
using static Sample.Api.Models.InvoiceSummary;

public class InvoiceServiceTests
{
    // Test data - varied and realistic
    private readonly List<Invoice> _invoices = new()
    {
        new Invoice { Id = 1, CustomerName = "John Doe", InvoiceDate = DateTime.UtcNow, TotalAmount = 1000.00M, Status = "PAID", ExportingBy = "Admin" },
        new Invoice { Id = 2, CustomerName = "Jane Smith", InvoiceDate = DateTime.UtcNow.AddDays(-1), TotalAmount = 500.00M, Status = "UNPAID", ExportingBy = "Manager" },
        new Invoice { Id = 3, CustomerName = "Alice Johnson", InvoiceDate = DateTime.UtcNow.AddDays(-2), TotalAmount = 750.00M, Status = "PAID", ExportingBy = "Admin" },
        new Invoice { Id = 4, CustomerName = "Bob Brown", InvoiceDate = DateTime.UtcNow.AddDays(-3), TotalAmount = 250.00M, Status = "CANCELLED", ExportingBy = "Manager" }
    };

    // Mock declarations
    private readonly Mock<IInvoiceRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;

    // System under test
    private readonly InvoiceService _sut;

    // Constructor with setup
    public InvoiceServiceTests()
    {
        _mockRepository = new Mock<IInvoiceRepository>();
        _mockMapper = new Mock<IMapper>();

        _mockRepository.Setup(repo => repo.GetAllInvoices()).ReturnsAsync(_invoices);

        _mockMapper.Setup(mapper => mapper.Map<IEnumerable<InvoiceSummary>>(It.IsAny<IEnumerable<Invoice>>())).Returns((IEnumerable<Invoice> invoices) =>
        {
            return invoices.Select(i => new InvoiceSummary
            {
                Id = i.Id,
                CustomerName = i.CustomerName,
                InvoiceDate = i.InvoiceDate,
                TotalAmount = i.TotalAmount,
                Status = i.Status,
                ExportingBy = i.ExportingBy
            }).ToList();
        });

        _sut = new InvoiceService(_mockRepository.Object, _mockMapper.Object);
    }

    #region Happy Path Tests

    [Fact]
    public async Task GetInvoiceSummaries_WithValidSearchTerm_ReturnsFilteredResults()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { SearchTerm = "John", PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _sut.GetInvoiceSummaries(listArgs);

        // Assert
        result.Items.Count.Should().Be(1);
        result.Items.First().CustomerName.Should().Be("John Doe");
    }

    [Fact]
    public async Task GetInvoiceSummaries_WithoutSearchTerm_ReturnsAllResults()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _sut.GetInvoiceSummaries(listArgs);

        // Assert
        result.Items.Count.Should().Be(4);
        result.Items.First().CustomerName.Should().Be("Alice Johnson");
    }

    [Fact]
    public async Task GetInvoiceSummaries_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 2, PageSize = 2 };

        // Act
        var result = await _sut.GetInvoiceSummaries(listArgs);

        // Assert
        result.CurrentPage.Should().Be(2);
        result.PageSize.Should().Be(2);
        result.Items.Count.Should().Be(2);
        result.Items.First().CustomerName.Should().Be("Jane Smith");
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public async Task GetInvoiceSummaries_WithEmptySearchTerm_ReturnsAllResults()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { SearchTerm = "", PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _sut.GetInvoiceSummaries(listArgs);

        // Assert
        result.Items.Count.Should().Be(4);
        result.Items.First().CustomerName.Should().Be("Alice Johnson");
    }

    [Fact]
    public async Task GetInvoiceSummaries_WithLargePageSize_ReturnsAllResults()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 100 };

        // Act
        var result = await _sut.GetInvoiceSummaries(listArgs);

        // Assert
        result.Items.Count.Should().Be(4);
        result.Items.First().CustomerName.Should().Be("Alice Johnson");
    }

    [Fact]
    public async Task GetInvoiceSummaries_WithZeroPageSize_ReturnsNoResults()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 0 };

        // Act
        var result = await _sut.GetInvoiceSummaries(listArgs);

        // Assert
        result.Items.Count.Should().Be(0);
    }

    #endregion

    #region Negative Tests

    [Fact]
    public async Task GetInvoiceSummaries_WithNullSearchTerm_ReturnsAllResults()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { SearchTerm = null, PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _sut.GetInvoiceSummaries(listArgs);

        // Assert
        result.Items.Count.Should().Be(4);
        result.Items.First().CustomerName.Should().Be("Alice Johnson");
    }

    [Fact]
    public async Task GetInvoiceSummaries_WithNonExistentStatus_ReturnsNoResults()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { Status = "INVALID_STATUS", PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _sut.GetInvoiceSummaries(listArgs);

        // Assert
        result.Items.Count.Should().Be(0);
    }

    #endregion

    #region Exception Tests

    [Fact]
    public async Task GetInvoiceSummaries_WhenRepositoryThrowsException_ThrowsBusinessException()
    {
        // Arrange
        var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10 };
        _mockRepository.Setup(repo => repo.GetAllInvoices()).ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act & Assert
        var exception = await FluentActions.Invoking(async () => await _sut.GetInvoiceSummaries(listArgs))
            .Should().ThrowAsync<BusinessException>();

        exception.Which.Message.Should().Be("An error occurred while fetching invoices.");
        exception.Which.InnerException.Should().BeOfType<InvalidOperationException>();
    }

    #endregion

    #region Helper Methods

    private InvoiceListArgs CreateValidListArgs(string searchTerm = null, int pageNumber = 1, int pageSize = 10)
    {
        return new InvoiceListArgs { SearchTerm = searchTerm, PageNumber = pageNumber, PageSize = pageSize };
    }

    #endregion
}