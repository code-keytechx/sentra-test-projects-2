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
    public class InvoiceService
    {
        private readonly ApplicationDbContext _dbContext; // or use repositories

        public InvoiceService(ApplicationDbContext dbContext)
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
                    InvoiceDate = i.InvoiceDate,
                    TotalAmount = i.TotalAmount,
                    Status = i.Status,
                    ExportingBy = i.ExportingBy,
                    ExportingDate = i.ExportingDate
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
    }
}

