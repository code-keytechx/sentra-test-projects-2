using AutoMapper;using Microsoft.AspNetCore.Mvc;using Sample.Api.Commands;using Sample.Api.Factories;using Sample.Api.Models;using Sample.Api.Services.Accounting.Dto;using Sample.Infrastructure.Documents;using System;

public class AccountingService : IAccountingService
    {
    summary>
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
}