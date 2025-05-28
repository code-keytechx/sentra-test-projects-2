using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sample.Api.Models;
using Sample.Api.Services.Accounting;
using System;

[TestClass]
public class AccountingServiceTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void GeneratePdf_WithNullInvoice_ThrowsArgumentNullException()
    {
        var accountingService = new Mock<IAccountingService>().Object;
        accountingService.GeneratePdf(null);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void GeneratePdf_WithEmptyInvoice_ThrowsInvalidOperationException()
    {
        var invoice = new InvoiceDetailViewModel { /* Initialize with empty values */ };
        var accountingService = new Mock<IAccountingService>().Object;
        accountingService.GeneratePdf(invoice);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void GeneratePdf_WithNegativeAmount_ThrowsArgumentOutOfRangeException()
    {
        var invoice = new InvoiceDetailViewModel { Amount = -100 };
        var accountingService = new Mock<IAccountingService>().Object;
        accountingService.GeneratePdf(invoice);
    }
}