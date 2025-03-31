using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pawp.Api.Requests;
using Pawp.Business.Contract;
using Pawp.DocGeneration;
using Pawp.Model;
using Pawp.Model.ListArgs;
using Pawp.Utility;
using Pawp.Utility.Commands;
using Pawp.Utility.Data;
using Pawp.Utility.DocGeneration.Contract;
using Pawp.ViewModel.output;
using System;
using System.Net;

namespace Pawp.Api.Controllers
{
    [ApiController]
    [Route("api/accounting")]
    public class AccountingController : ApiBaseController
    {
        private readonly IInvoiceCommandFactory _invoiceCommandFactory;
        private readonly IMapper _mapper;
        private readonly IDocumentFactory _documentFactory;

        public AccountingController(IInvoiceCommandFactory invoiceCommandFactory, IMapper mapper, IDocumentFactory documentFactory)
        {
            _invoiceCommandFactory = invoiceCommandFactory ?? throw new ArgumentNullException(nameof(invoiceCommandFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _documentFactory = documentFactory ?? throw new ArgumentNullException(nameof(documentFactory));
        }

        [HttpGet("invoices")]
        public PagedResults<InvoiceSummary> Get([FromQuery] InvoiceListArgs listArgs)
        {
            ICommand<PagedResults<InvoiceSummary>> command = _invoiceCommandFactory.GetInvoiceSummaries(listArgs);
            command.Execute();
            return command.ResultData;
        }

        [HttpPost("invoices")]
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(int))]
        public ActionResult<int> AddInvoice([FromBody] DtoAddInvoiceInput dtoInvoice)
        {
            Invoice invoice = _mapper.Map<Invoice>(dtoInvoice);

            var command = _invoiceCommandFactory.AddInvoiceVoid(invoice);
            command.Execute();
            return CreatedAtRoute("GetInvoiceById", new { Id = command.ResultData }, command.ResultData);
        }
    }
}