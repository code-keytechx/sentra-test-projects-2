using NUnit.Framework;
using Sample.Api.Services.Accounting;
using Sample.Api.Models;
using System;

[TestFixture]
public class AccountingServiceTests_Negative
{
    [Test]
    public void GeneratePdf_WithInvalidInvoiceType_ThrowsArgumentException()
    {
        var service = new AccountingService();
        var invalidInvoice = new InvoiceDetailViewModel { Type = "Invalid" };

        Assert.Throws<ArgumentException>(() => service.GeneratePdf(invalidInvoice));
    }

    [Test]
    public void GeneratePdf_WithNegativeAmount_ThrowsArgumentOutOfRangeException()
    {
        var service = new AccountingService();
        var negativeInvoice = new InvoiceDetailViewModel { Amount = -100 };

        Assert.Throws<ArgumentOutOfRangeException>(() => service.GeneratePdf(negativeInvoice));
    }

    [Test]
    public void GeneratePdf_WithNullItems_ThrowsArgumentNullException()
    {
        var service = new AccountingService();
        var nullItemsInvoice = new InvoiceDetailViewModel { Items = null };

        Assert.Throws<ArgumentNullException>(() => service.GeneratePdf(nullItemsInvoice));
    }
}