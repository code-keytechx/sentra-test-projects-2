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
using Moq;
using FluentAssertions;

namespace Sample.Api.Tests
{
    public class AccountingServiceTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IInvoiceDocumentFactory> _invoiceDocumentFactoryMock;
        private readonly AccountingService _accountingService;

        public AccountingServiceTests()
        {
            _mapperMock = new Mock<IMapper>();
            _invoiceDocumentFactoryMock = new Mock<IInvoiceDocumentFactory>();
            _accountingService = new AccountingService(_mapperMock.Object, _invoiceDocumentFactoryMock.Object);
        }

        #region Happy Path Tests

        [Fact]
        public async Task GeneratePdf_WithValidInvoiceDetailViewModel_GeneratesPdf()
        {
            // Arrange
            var invoiceDetailViewModel = new InvoiceDetailViewModel
            {
                InvoiceNumber = "INV-2023-001",
                CustomerName = "John Doe",
                Items = new List<InvoiceItemViewModel>
                {
                    new InvoiceItemViewModel { Description = "Item 1", Quantity = 1, Price = 100.00M }
                },
                TotalAmount = 100.00M,
                DueDate = DateTime.UtcNow.AddDays(30)
            };

            _invoiceDocumentFactoryMock.Setup(factory => factory.CreateInvoiceDocument(It.IsAny<InvoiceDto>())).Returns(Task.FromResult(Array.Empty<byte>()));

            // Act
            var result = await _accountingService.GeneratePdf(invoiceDetailViewModel);

            // Assert
            result.Should().NotBeNull();
            _invoiceDocumentFactoryMock.Verify(factory => factory.CreateInvoiceDocument(It.IsAny<InvoiceDto>()), Times.Once);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task GeneratePdf_WithEmptyItems_GeneratesPdf()
        {
            // Arrange
            var invoiceDetailViewModel = new InvoiceDetailViewModel
            {
                InvoiceNumber = "INV-2023-001",
                CustomerName = "John Doe",
                Items = new List<InvoiceItemViewModel>(),
                TotalAmount = 0.00M,
                DueDate = DateTime.UtcNow.AddDays(30)
            };

            _invoiceDocumentFactoryMock.Setup(factory => factory.CreateInvoiceDocument(It.IsAny<InvoiceDto>())).Returns(Task.FromResult(Array.Empty<byte>()));

            // Act
            var result = await _accountingService.GeneratePdf(invoiceDetailViewModel);

            // Assert
            result.Should().NotBeNull();
            _invoiceDocumentFactoryMock.Verify(factory => factory.CreateInvoiceDocument(It.IsAny<InvoiceDto>()), Times.Once);
        }

        [Fact]
        public async Task GeneratePdf_WithNullItems_GeneratesPdf()
        {
            // Arrange
            var invoiceDetailViewModel = new InvoiceDetailViewModel
            {
                InvoiceNumber = "INV-2023-001",
                CustomerName = "John Doe",
                Items = null,
                TotalAmount = 0.00M,
                DueDate = DateTime.UtcNow.AddDays(30)
            };

            _invoiceDocumentFactoryMock.Setup(factory => factory.CreateInvoiceDocument(It.IsAny<InvoiceDto>())).Returns(Task.FromResult(Array.Empty<byte>()));

            // Act
            var result = await _accountingService.GeneratePdf(invoiceDetailViewModel);

            // Assert
            result.Should().NotBeNull();
            _invoiceDocumentFactoryMock.Verify(factory => factory.CreateInvoiceDocument(It.IsAny<InvoiceDto>()), Times.Once);
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task GeneratePdf_WithNegativeTotalAmount_ThrowsArgumentException()
        {
            // Arrange
            var invoiceDetailViewModel = new InvoiceDetailViewModel
            {
                InvoiceNumber = "INV-2023-001",
                CustomerName = "John Doe",
                Items = new List<InvoiceItemViewModel>
                {
                    new InvoiceItemViewModel { Description = "Item 1", Quantity = 1, Price = 100.00M }
                },
                TotalAmount = -100.00M,
                DueDate = DateTime.UtcNow.AddDays(30)
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _accountingService.GeneratePdf(invoiceDetailViewModel));
        }

        [Fact]
        public async Task GeneratePdf_WithNullInvoiceNumber_ThrowsArgumentNullException()
        {
            // Arrange
            var invoiceDetailViewModel = new InvoiceDetailViewModel
            {
                CustomerName = "John Doe",
                Items = new List<InvoiceItemViewModel>
                {
                    new InvoiceItemViewModel { Description = "Item 1", Quantity = 1, Price = 100.00M }
                },
                TotalAmount = 100.00M,
                DueDate = DateTime.UtcNow.AddDays(30)
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _accountingService.GeneratePdf(invoiceDetailViewModel));
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task GeneratePdf_WhenInvoiceDocumentFactoryThrowsException_ThrowsException()
        {
            // Arrange
            var invoiceDetailViewModel = new InvoiceDetailViewModel
            {
                InvoiceNumber = "INV-2023-001",
                CustomerName = "John Doe",
                Items = new List<InvoiceItemViewModel>
                {
                    new InvoiceItemViewModel { Description = "Item 1", Quantity = 1, Price = 100.00M }
                },
                TotalAmount = 100.00M,
                DueDate = DateTime.UtcNow.AddDays(30)
            };

            _invoiceDocumentFactoryMock.Setup(factory => factory.CreateInvoiceDocument(It.IsAny<InvoiceDto>())).ThrowsAsync(new InvalidOperationException("PDF creation failed"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _accountingService.GeneratePdf(invoiceDetailViewModel));
        }

        #endregion

        #region Helper Methods

        private InvoiceDto MapToInvoiceDto(InvoiceDetailViewModel viewModel)
        {
            return _mapperMock.Object.Map<InvoiceDto>(viewModel);
        }

        #endregion
    }
}