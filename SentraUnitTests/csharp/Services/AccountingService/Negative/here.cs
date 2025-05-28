using NUnit.Framework;
using Sample.Api.Services.Accounting;
using Sample.Api.Models;
using System;

[TestFixture]
public class AccountingServiceTests_Negative
{
    [Test]
    public void GeneratePdf_WithNullInvoice_ThrowsArgumentNullException()
    {
        var accountingService = new AccountingService();

        Assert.Throws<ArgumentNullException>(() => accountingService.GeneratePdf(null));
    }

    [Test]
    public void GeneratePdf_WithInvalidInvoiceType_ThrowsArgumentException()
    {
        var accountingService = new AccountingService();
        var invalidInvoice = new InvoiceDetailViewModel { Type = "Invalid" };

        Assert.Throws<ArgumentException>(() => accountingService.GeneratePdf(invalidInvoice));
    }

    [Test]
    public void GeneratePdf_WithNegativeAmount_ThrowsArgumentOutOfRangeException()
    {
        var accountingService = new AccountingService();
        var negativeInvoice = new InvoiceDetailViewModel { Amount = -100 };

        Assert.Throws<ArgumentOutOfRangeException>(() => accountingService.GeneratePdf(negativeInvoice));
    }
}