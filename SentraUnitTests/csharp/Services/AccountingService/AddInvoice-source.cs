using AutoMapper;using Microsoft.AspNetCore.Mvc;using Sample.Api.Commands;using Sample.Api.Factories;using Sample.Api.Models;using Sample.Api.Services.Accounting.Dto;using Sample.Infrastructure.Documents;using System;

public class AccountingService : IAccountingService
    {
    summary>
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
}