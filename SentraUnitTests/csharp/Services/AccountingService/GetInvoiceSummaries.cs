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
using static Sample.Api.Controllers.AccountingController;

namespace Sample.Api.Tests
{
    public class AccountingServiceTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IInvoicesRepository> _mockRepository;
        private readonly AccountingService _accountingService;

        public AccountingServiceTests()
        {
            _mockRepository = new Mock<IInvoicesRepository>();
            _mapper = MapperFactory.CreateMapper();
            _accountingService = new AccountingService(_mockRepository.Object, _mapper);
        }

        #region Happy Path Tests

        [Fact]
        public async Task GetInvoiceSummaries_WithValidArguments_ReturnsPagedResults()
        {
            // Arrange
            var invoices = new List<Invoice>
            {
                new Invoice { Id = 1, CustomerName = "John Doe", InvoiceDate = DateTime.UtcNow },
                new Invoice { Id = 2, CustomerName = "Jane Smith", InvoiceDate = DateTime.UtcNow.AddDays(-1) }
            };

            var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10, SearchTerm = "" };

            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(invoices);

            // Act
            var result = await _accountingService.GetInvoiceSummaries(listArgs);

            // Assert
            result.CurrentPage.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.TotalCount.Should().Be(2);
            result.Items.Should().HaveCount(2);
            result.Items.First().Id.Should().Be(2);
            result.Items.First().CustomerName.Should().Be("Jane Smith");
            result.Items.First().InvoiceDate.Should().BeCloseTo(DateTime.UtcNow.AddDays(-1), TimeSpan.FromSeconds(1));
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task GetInvoiceSummaries_WithEmptySearchTerm_ReturnsAllInvoices()
        {
            // Arrange
            var invoices = new List<Invoice>
            {
                new Invoice { Id = 1, CustomerName = "John Doe", InvoiceDate = DateTime.UtcNow },
                new Invoice { Id = 2, CustomerName = "Jane Smith", InvoiceDate = DateTime.UtcNow.AddDays(-1) }
            };

            var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10, SearchTerm = "" };

            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(invoices);

            // Act
            var result = await _accountingService.GetInvoiceSummaries(listArgs);

            // Assert
            result.Items.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetInvoiceSummaries_WithNonExistentSearchTerm_ReturnsNoResults()
        {
            // Arrange
            var invoices = new List<Invoice>();

            var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10, SearchTerm = "NonExistent" };

            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(invoices);

            // Act
            var result = await _accountingService.GetInvoiceSummaries(listArgs);

            // Assert
            result.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task GetInvoiceSummaries_WithLargePageSize_ReturnsAllInvoices()
        {
            // Arrange
            var invoices = new List<Invoice>
            {
                new Invoice { Id = 1, CustomerName = "John Doe", InvoiceDate = DateTime.UtcNow },
                new Invoice { Id = 2, CustomerName = "Jane Smith", InvoiceDate = DateTime.UtcNow.AddDays(-1) }
            };

            var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 100, SearchTerm = "" };

            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(invoices);

            // Act
            var result = await _accountingService.GetInvoiceSummaries(listArgs);

            // Assert
            result.Items.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetInvoiceSummaries_WithFirstPage_ReturnsFirstPageOfResults()
        {
            // Arrange
            var invoices = new List<Invoice>
            {
                new Invoice { Id = 1, CustomerName = "John Doe", InvoiceDate = DateTime.UtcNow },
                new Invoice { Id = 2, CustomerName = "Jane Smith", InvoiceDate = DateTime.UtcNow.AddDays(-1) }
            };

            var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 1, SearchTerm = "" };

            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(invoices);

            // Act
            var result = await _accountingService.GetInvoiceSummaries(listArgs);

            // Assert
            result.Items.Should().HaveCount(1);
            result.Items.First().Id.Should().Be(2);
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task GetInvoiceSummaries_WithNegativePageNumber_ThrowsArgumentException()
        {
            // Arrange
            var listArgs = new InvoiceListArgs { PageNumber = -1, PageSize = 10, SearchTerm = "" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _accountingService.GetInvoiceSummaries(listArgs));
        }

        [Fact]
        public async Task GetInvoiceSummaries_WithZeroPageSize_ThrowsArgumentException()
        {
            // Arrange
            var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 0, SearchTerm = "" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _accountingService.GetInvoiceSummaries(listArgs));
        }

        [Fact]
        public async Task GetInvoiceSummaries_WithNullSearchTerm_DoesNotFilter()
        {
            // Arrange
            var invoices = new List<Invoice>
            {
                new Invoice { Id = 1, CustomerName = "John Doe", InvoiceDate = DateTime.UtcNow },
                new Invoice { Id = 2, CustomerName = "Jane Smith", InvoiceDate = DateTime.UtcNow.AddDays(-1) }
            };

            var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10, SearchTerm = null };

            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(invoices);

            // Act
            var result = await _accountingService.GetInvoiceSummaries(listArgs);

            // Assert
            result.Items.Should().HaveCount(2);
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task GetInvoiceSummaries_WhenRepositoryThrowsException_ThrowsBusinessException()
        {
            // Arrange
            var listArgs = new InvoiceListArgs { PageNumber = 1, PageSize = 10, SearchTerm = "" };

            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _accountingService.GetInvoiceSummaries(listArgs));
        }

        #endregion
    }
}