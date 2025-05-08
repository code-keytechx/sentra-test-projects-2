using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sample.Api.Models;
using Sample.Api.Services.Accounting;
using Sample.Api.Services.Accounting.Dto;
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
        var invoice = new InvoiceDetailViewModel { Items = null };
        var accountingService = new Mock<IAccountingService>().Object;
        accountingService.GeneratePdf(invoice);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void GeneratePdf_WithInvalidInvoiceType_ThrowsInvalidOperationException()
    {
        var invoice = new InvoiceDetailViewModel { Type = "InvalidType" };
        var accountingService = new Mock<IAccountingService>().Object;
        accountingService.GeneratePdf(invoice);
    }
}