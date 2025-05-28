using AutoMapper;using Microsoft.AspNetCore.Mvc;using Sample.Api.Commands;using Sample.Api.Factories;using Sample.Api.Models;using Sample.Api.Services.Accounting.Dto;using Sample.Infrastructure.Documents;using System;

public class AccountingService : IAccountingService
    {
    summary>
        public IActionResult PreviewInvoiceDetailHtml(int invoiceId)
        {
            var invoice = GetInvoiceById(invoiceId);
            if (invoice == null)
            {
                return new NotFoundResult();
            }

            // Build a simple HTML string or use a templating library.
            var html = $"<html><body>" +
                    $"<h1>Invoice #{invoice.Id}</h1>" +
                    $"<p>Customer: {invoice.CustomerName}</p>" +
                    $"<p>Date: {invoice.InvoiceDate:yyyy-MM-dd}</p>" +
                    $"<p>Total: {invoice.TotalAmount:F2}</p>" +
                    // etc.
                    $"</body></html>";

            return new ContentResult
            {
                Content = html,
                ContentType = "text/html"
            };
        }
}