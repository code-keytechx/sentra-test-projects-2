using Microsoft.AspNetCore.Mvc;
using Sample.Api.Models;
using Sample.Api.Services.Accounting.Dto; // or wherever your DTOs/models are
using System.Collections.Generic;

namespace Sample.Api.Services.Accounting
{
    public interface IAccountingService
    {
        PagedResults<InvoiceSummary> GetInvoiceSummaries(InvoiceListArgs listArgs);
        int AddInvoice(DtoAddInvoiceInput dtoInvoice);
        FileResult DownloadInvoicesCsv(int[] invoiceIds);
        InvoiceDetailViewModel GetInvoiceById(int id);
        void CalculateInvoiceTotal(int id);
        InvoiceDetailViewModel UpdateInvoice(DtoUpdateInvoiceInput dtoInvoice, int id);
        FileResult DownloadInvoiceDetailPdf(int invoiceId);
        IActionResult PreviewInvoiceDetailHtml(int invoiceId);
    }
}
