using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sample.Api.Commands;
using Sample.Api.Factories;
using Sample.Api.Models;
using Sample.Api.Services.Accounting.Dto;
using Sample.Infrastructure.Documents;
using System;

namespace Sample.Api.Services.Accounting
{
    public class AccountingService : IAccountingService
    {
        private readonly ApplicationDbContext _dbContext; // or use repositories

        public AccountingService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Retrieves paged invoice summaries (header information).
        /// </summary>
        public PagedResults<InvoiceSummary> GetInvoiceSummaries(InvoiceListArgs listArgs)
        {
            // Example:
            // 1. Start a query from your Invoices table.
            // 2. Filter or search using listArgs (date range, status, etc.).
            // 3. Count total for pagination.
            // 4. Select only the needed summary fields.
            // 5. Project them into InvoiceSummary objects.
            // 6. Apply skip and take for paging.
            
            IQueryable<Invoice> query = _dbContext.Invoices.AsQueryable();

            // Example filtering
            if (!string.IsNullOrEmpty(listArgs.SearchTerm))
            {
                query = query.Where(i => i.CustomerName.Contains(listArgs.SearchTerm));
            }

            // Sort by date descending for example
            query = query.OrderByDescending(i => i.InvoiceDate);

            // Pagination
            var totalCount = query.Count();
            var items = query
                .Skip((listArgs.PageNumber - 1) * listArgs.PageSize)
                .Take(listArgs.PageSize)
                .Select(i => new InvoiceSummary
                {
                    Id = i.Id,
                    CustomerName = i.CustomerName,
                    InvoiceDate = i.InvoiceDate
                })
                .ToList();

            // Build the paged results
            var pagedResults = new PagedResults<InvoiceSummary>
            {
                CurrentPage = listArgs.PageNumber,
                PageSize = listArgs.PageSize,
                TotalCount = totalCount,
                Items = items
            };

            return pagedResults;
        }

        /// <summary>
        /// Adds a new invoice and returns the new invoice ID.
        /// </summary>
        public int AddInvoice(DtoAddInvoiceInput dtoInvoice)
        {
            // Map DtoAddInvoiceInput to Invoice entity
            var newInvoice = new Invoice
            {
                CustomerName = dtoInvoice.CustomerName,
                InvoiceDate = dtoInvoice.InvoiceDate,
                // Initialize total to 0 or based on line items
                TotalAmount = 0
            };

            // If there are line items in the DTO, add them as well:
            foreach (var lineItemDto in dtoInvoice.LineItems)
            {
                var lineItem = new InvoiceLineItem
                {
                    Description = lineItemDto.Description,
                    Quantity = lineItemDto.Quantity,
                    UnitPrice = lineItemDto.UnitPrice,
                    // If needed, compute line item total
                    LineTotal = lineItemDto.Quantity * lineItemDto.UnitPrice
                };
                newInvoice.LineItems.Add(lineItem);
            }

            _dbContext.Invoices.Add(newInvoice);
            _dbContext.SaveChanges();

            return newInvoice.Id;
        }

        /// <summary>
        /// Returns a CSV file containing details for the specified invoices.
        /// </summary>
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

        /// <summary>
        /// Retrieves the details of a single invoice.
        /// Returns null if no invoice is found.
        /// </summary>
        public InvoiceDetailViewModel GetInvoiceById(int id)
        {
            var invoice = _dbContext.Invoices
                .Where(i => i.Id == id)
                .Select(i => new InvoiceDetailViewModel
                {
                    Id = i.Id,
                    CustomerName = i.CustomerName,
                    InvoiceDate = i.InvoiceDate,
                    TotalAmount = i.TotalAmount,
                    // Map line items
                    LineItems = i.LineItems.Select(li => new InvoiceLineItemViewModel
                    {
                        Id = li.Id,
                        Description = li.Description,
                        Quantity = li.Quantity,
                        UnitPrice = li.UnitPrice,
                        LineTotal = li.LineTotal
                    }).ToList()
                })
                .FirstOrDefault();

            return invoice;
        }

        /// <summary>
        /// Calculates the invoice total (sums of line items, taxes, discounts, etc.).
        /// </summary>
        public void CalculateInvoiceTotal(int id)
        {
            var invoice = _dbContext.Invoices
                .Include("LineItems")  // or .Include(i => i.LineItems) if using EF
                .FirstOrDefault(i => i.Id == id);

            if (invoice == null)
                return; // or throw new Exception("Invoice not found");

            // For example, sum line items:
            var sumOfLineItems = invoice.LineItems.Sum(li => li.LineTotal);
            // Add taxes, discounts, etc. if needed.
            invoice.TotalAmount = sumOfLineItems;

            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Updates an existing invoice and returns the updated invoice detail.
        /// Returns null if not found.
        /// </summary>
        public InvoiceDetailViewModel UpdateInvoice(DtoUpdateInvoiceInput dtoInvoice, int id)
        {
            var invoice = _dbContext.Invoices
                .Include("LineItems")
                .FirstOrDefault(i => i.Id == id);

            if (invoice == null)
                return null;

            // Update simple fields
            invoice.CustomerName = dtoInvoice.CustomerName;
            invoice.InvoiceDate = dtoInvoice.InvoiceDate;

            // Update or replace line items:
            // Option A: remove all existing line items and re-add (simple approach)
            // Option B: carefully map changes, update, add or remove as needed
            invoice.LineItems.Clear();
            foreach (var lineItemDto in dtoInvoice.LineItems)
            {
                invoice.LineItems.Add(new InvoiceLineItem
                {
                    Description = lineItemDto.Description,
                    Quantity = lineItemDto.Quantity,
                    UnitPrice = lineItemDto.UnitPrice,
                    LineTotal = lineItemDto.Quantity * lineItemDto.UnitPrice
                });
            }

            _dbContext.SaveChanges();

            // Return the updated invoice as a detail view model
            return new InvoiceDetailViewModel
            {
                Id = invoice.Id,
                CustomerName = invoice.CustomerName,
                InvoiceDate = invoice.InvoiceDate,
                TotalAmount = invoice.TotalAmount,
                LineItems = invoice.LineItems
                    .Select(li => new InvoiceLineItemViewModel
                    {
                        Id = li.Id,
                        Description = li.Description,
                        Quantity = li.Quantity,
                        UnitPrice = li.UnitPrice,
                        LineTotal = li.LineTotal
                    })
                    .ToList()
            };
        }

        /// <summary>
        /// Generates a PDF file for the specified invoice and returns it as a file download.
        /// Returns null if invoice does not exist.
        /// </summary>
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

        /// <summary>
        /// Returns an HTML preview of the invoice.
        /// Could simply return HTML as ContentResult or similar.
        /// </summary>
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

        // Example method for generating PDF from invoice data
        private byte[] GeneratePdf(InvoiceDetailViewModel invoice)
        {
            // Implement your PDF generation logic here (iTextSharp, PDFSharp, etc.).
            // Return the byte[] with the PDF content.

            // Placeholder: just return empty
            return Array.Empty<byte>();
        }
    }
}

