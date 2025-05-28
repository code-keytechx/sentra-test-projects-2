using AutoMapper;using Microsoft.AspNetCore.Mvc;using Sample.Api.Commands;using Sample.Api.Factories;using Sample.Api.Models;using Sample.Api.Services.Accounting.Dto;using Sample.Infrastructure.Documents;using System;

public class AccountingService : IAccountingService
    {
    summary>
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
}