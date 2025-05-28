using AutoMapper;using Microsoft.AspNetCore.Mvc;using Sample.Api.Commands;using Sample.Api.Factories;using Sample.Api.Models;using Sample.Api.Services.Accounting.Dto;using Sample.Infrastructure.Documents;using System;

public class AccountingService : IAccountingService
    {
    summary>
        public FileResult DownloadInvoicesCsv(int[] invoiceIds)
        {
            // 1. Fetch the relevant invoices
            var invoices = _dbContext.Invoices
                .Where(i => invoiceIds.Contains(i.Id))
                .ToList();

            // 2. Generate CSV data in-memory
            // (For brevity, an example of writing CSV manually;
            //  in production, consider using a CSV library like CsvHelper.)
            var csvLines = new List<string>
            {
                "InvoiceId,CustomerName,InvoiceDate,TotalAmount"
            };
            foreach (var inv in invoices)
            {
                var line = $"{inv.Id},{inv.CustomerName},{inv.InvoiceDate:yyyy-MM-dd},{inv.TotalAmount:F2}";
                csvLines.Add(line);
            }
            var csvData = string.Join(Environment.NewLine, csvLines);

            // 3. Return as FileResult (with MIME type text/csv)
            var byteArray = System.Text.Encoding.UTF8.GetBytes(csvData);
            var result = new FileContentResult(byteArray, "text/csv")
            {
                FileDownloadName = $"Invoices_{DateTime.UtcNow:yyyyMMddHHmmss}.csv"
            };
            return result;
        }
}