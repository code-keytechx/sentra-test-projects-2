using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Sample.Api.Commands;
using Sample.Api.Factories;
using Sample.Api.Models;
using Sample.Api.Services.Accounting;
using Sample.Api.Services.Accounting.Dto;
using Sample.Infrastructure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sample.Api.Tests
{
    public class AccountingServiceTests
    {
        private readonly Mock<IApplicationDbContext> _mockDbContext;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<AccountingService>> _mockLogger;
        private readonly AccountingService _accountingService;

        public AccountingServiceTests()
        {
            _mockDbContext = new Mock<IApplicationDbContext>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<AccountingService>>();
            _accountingService = new AccountingService(_mockDbContext.Object, _mockMapper.Object, _mockLogger.Object);
        }

        #region Happy Path Tests

        [Fact]
        public async Task CalculateInvoiceTotal_WithExistingInvoice_SumsLineItems()
        {
            // Arrange
            var invoiceId = 1;
            var lineItem1 = new LineItem { LineTotal = 100 };
            var lineItem2 = new LineItem { LineTotal = 200 };
            var invoice = new Invoice { Id = invoiceId, LineItems = new List<LineItem> { lineItem1, lineItem2 } };

            _mockDbContext.Setup(db => db.Invoices.Include("LineItems").FirstOrDefaultAsync(i => i.Id == invoiceId))
                .ReturnsAsync(invoice);

            // Act
            await _accountingService.CalculateInvoiceTotal(invoiceId);

            // Assert
            Assert.Equal(300, invoice.TotalAmount);
            _mockDbContext.Verify(db => db.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task CalculateInvoiceTotal_WithEmptyLineItems_SetsTotalToZero()
        {
            // Arrange
            var invoiceId = 1;
            var invoice = new Invoice { Id = invoiceId, LineItems = new List<LineItem>() };

            _mockDbContext.Setup(db => db.Invoices.Include("LineItems").FirstOrDefaultAsync(i => i.Id == invoiceId))
                .ReturnsAsync(invoice);

            // Act
            await _accountingService.CalculateInvoiceTotal(invoiceId);

            // Assert
            Assert.Equal(0, invoice.TotalAmount);
            _mockDbContext.Verify(db => db.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CalculateInvoiceTotal_WithNullInvoice_DoesNothing()
        {
            // Arrange
            var invoiceId = 1;

            _mockDbContext.Setup(db => db.Invoices.Include("LineItems").FirstOrDefaultAsync(i => i.Id == invoiceId))
                .ReturnsAsync((Invoice)null);

            // Act
            await _accountingService.CalculateInvoiceTotal(invoiceId);

            // Assert
            _mockDbContext.Verify(db => db.SaveChangesAsync(), Times.Never);
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task CalculateInvoiceTotal_WithNegativeLineItemTotals_IgnoresNegatives()
        {
            // Arrange
            var invoiceId = 1;
            var lineItem1 = new LineItem { LineTotal = 100 };
            var lineItem2 = new LineItem { LineTotal = -50 };
            var invoice = new Invoice { Id = invoiceId, LineItems = new List<LineItem> { lineItem1, lineItem2 } };

            _mockDbContext.Setup(db => db.Invoices.Include("LineItems").FirstOrDefaultAsync(i => i.Id == invoiceId))
                .ReturnsAsync(invoice);

            // Act
            await _accountingService.CalculateInvoiceTotal(invoiceId);

            // Assert
            Assert.Equal(100, invoice.TotalAmount);
            _mockDbContext.Verify(db => db.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task CalculateInvoiceTotal_WhenDatabaseSaveFails_ThrowsException()
        {
            // Arrange
            var invoiceId = 1;
            var lineItem1 = new LineItem { LineTotal = 100 };
            var lineItem2 = new LineItem { LineTotal = 200 };
            var invoice = new Invoice { Id = invoiceId, LineItems = new List<LineItem> { lineItem1, lineItem2 } };

            _mockDbContext.Setup(db => db.Invoices.Include("LineItems").FirstOrDefaultAsync(i => i.Id == invoiceId))
                .ReturnsAsync(invoice);
            _mockDbContext.Setup(db => db.SaveChangesAsync())
                .ThrowsAsync(new DbUpdateConcurrencyException());

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _accountingService.CalculateInvoiceTotal(invoiceId));
        }

        #endregion
    }
}