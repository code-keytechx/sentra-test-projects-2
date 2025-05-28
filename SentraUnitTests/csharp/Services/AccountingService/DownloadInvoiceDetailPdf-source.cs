using AutoMapper;using Microsoft.AspNetCore.Mvc;using Sample.Api.Commands;using Sample.Api.Factories;using Sample.Api.Models;using Sample.Api.Services.Accounting.Dto;using Sample.Infrastructure.Documents;using System;

public class AccountingService : IAccountingService
    {
    summary>
        public FileResult DownloadInvoiceDetailPdf(int invoiceId)
        {
            var invoice = GetInvoiceById(invoiceId);
            if (invoice == null)
                return null; 

            // Build your PDF in-memory here, using iTextSharp or another PDF library
            // For simplicity, let's say we have a byte array "pdfBytes".
            byte[] pdfBytes = GeneratePdf(invoice); // your custom method

            var result = new FileContentResult(pdfBytes, "application/pdf")
            {
                FileDownloadName = $"Invoice_{invoiceId}.pdf"
            };

            return result;
        }
}