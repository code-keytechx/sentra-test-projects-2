using AutoMapper;using Microsoft.AspNetCore.Mvc;using Sample.Api.Commands;using Sample.Api.Factories;using Sample.Api.Models;using Sample.Api.Services.Accounting.Dto;using Sample.Infrastructure.Documents;using System;

public class AccountingService : IAccountingService
    {
    summary>
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
}