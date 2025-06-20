using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
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

namespace Sample.Api.Tests
{
    public class AccountingServiceTests
    {
        private readonly Mock<IInvoiceDbContext> _mockDbContext;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AccountingService _accountingService;

        public AccountingServiceTests()
        {
            _mockDbContext = new Mock<IInvoiceDbContext>();
            _mockMapper = new Mock<IMapper>();
            _accountingService = new AccountingService(_mockDbContext.Object, _mockMapper.Object);
        }

        #region Happy Path Tests

        [Fact]
        public async Task AddInvoice_WithValidInput_ReturnsNewInvoiceId()
        {
            // Arrange
            var dtoInvoice = new DtoAddInvoiceInput
            {
                CustomerName = "John Doe",
                InvoiceDate = DateTime.UtcNow,
                LineItems = new List<DtoInvoiceLineItem>
                {
                    new DtoInvoiceLineItem { Description = "Product A", Quantity = 2, UnitPrice = 100.00M }
                }
            };

            var newInvoice = new Invoice
            {
                CustomerName = dtoInvoice.CustomerName,
                InvoiceDate = dtoInvoice.InvoiceDate,
                TotalAmount = 200.00M,
                LineItems = new List<InvoiceLineItem>
                {
                    new InvoiceLineItem { Description = "Product A", Quantity = 2, UnitPrice = 100.00M, LineTotal = 200.00M }
                }
            };

            _mockMapper.Setup(m => m.Map<Invoice>(dtoInvoice)).Returns(newInvoice);

            // Act
            var result = await _accountingService.AddInvoice(dtoInvoice);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<int>(result);
            Assert.Equal(1, result); // Assuming auto-generated ID starts from 1
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task AddInvoice_WithEmptyLineItems_ReturnsNewInvoiceId()
        {
            // Arrange
            var dtoInvoice = new DtoAddInvoiceInput
            {
                CustomerName = "Jane Smith",
                InvoiceDate = DateTime.UtcNow,
                LineItems = new List<DtoInvoiceLineItem>()
            };

            var newInvoice = new Invoice
            {
                CustomerName = dtoInvoice.CustomerName,
                InvoiceDate = dtoInvoice.InvoiceDate,
                TotalAmount = 0.00M,
                LineItems = new List<InvoiceLineItem>()
            };

            _mockMapper.Setup(m => m.Map<Invoice>(dtoInvoice)).Returns(newInvoice);

            // Act
            var result = await _accountingService.AddInvoice(dtoInvoice);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<int>(result);
            Assert.Equal(1, result); // Assuming auto-generated ID starts from 1
        }

        [Fact]
        public async Task AddInvoice_WithNullDtoInvoice_ThrowsArgumentNullException()
        {
            // Arrange
            DtoAddInvoiceInput dtoInvoice = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _accountingService.AddInvoice(dtoInvoice));
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task AddInvoice_WithNegativeQuantity_ThrowsArgumentException()
        {
            // Arrange
            var dtoInvoice = new DtoAddInvoiceInput
            {
                CustomerName = "Alice Johnson",
                InvoiceDate = DateTime.UtcNow,
                LineItems = new List<DtoInvoiceLineItem>
                {
                    new DtoInvoiceLineItem { Description = "Product B", Quantity = -1, UnitPrice = 50.00M }
                }
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _accountingService.AddInvoice(dtoInvoice));
        }

        [Fact]
        public async Task AddInvoice_WithZeroUnitPrice_ThrowsArgumentException()
        {
            // Arrange
            var dtoInvoice = new DtoAddInvoiceInput
            {
                CustomerName = "Bob Brown",
                InvoiceDate = DateTime.UtcNow,
                LineItems = new List<DtoInvoiceLineItem>
                {
                    new DtoInvoiceLineItem { Description = "Product C", Quantity = 1, UnitPrice = 0.00M }
                }
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _accountingService.AddInvoice(dtoInvoice));
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task AddInvoice_WhenDbContextSaveChangesFails_ThrowsDbUpdateException()
        {
            // Arrange
            var dtoInvoice = new DtoAddInvoiceInput
            {
                CustomerName = "Charlie Davis",
                InvoiceDate = DateTime.UtcNow,
                LineItems = new List<DtoInvoiceLineItem>
                {
                    new DtoInvoiceLineItem { Description = "Product D", Quantity = 3, UnitPrice = 200.00M }
                }
            };

            var newInvoice = new Invoice
            {
                CustomerName = dtoInvoice.CustomerName,
                InvoiceDate = dtoInvoice.InvoiceDate,
                TotalAmount = 600.00M,
                LineItems = new List<InvoiceLineItem>
                {
                    new InvoiceLineItem { Description = "Product D", Quantity = 3, UnitPrice = 200.00M, LineTotal = 600.00M }
                }
            };

            _mockMapper.Setup(m => m.Map<Invoice>(dtoInvoice)).Returns(newInvoice);
            _mockDbContext.Setup(db => db.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => _accountingService.AddInvoice(dtoInvoice));
        }

        #endregion

        #region Helper Methods

        private DtoAddInvoiceInput CreateValidDtoInvoice()
        {
            return new DtoAddInvoiceInput
            {
                CustomerName = "John Doe",
                InvoiceDate = DateTime.UtcNow,
                LineItems = new List<DtoInvoiceLineItem>
                {
                    new DtoInvoiceLineItem { Description = "Product A", Quantity = 2, UnitPrice = 100.00M }
                }
            };
        }

        #endregion
    }
}