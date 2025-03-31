using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sample.Api.Requests;
using Sample.Business.Contract;
using Sample.DocGeneration;
using Sample.Model;
using Sample.Model.ListArgs;
using Sample.Utility;
using Sample.Utility.Commands;
using Sample.Utility.Data;
using Sample.Utility.DocGeneration.Contract;
using Sample.ViewModel.output;
using System;
using System.Net;

namespace Sample.Api.Controllers
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

        [HttpGet("download-invoices-csv")]
        public IActionResult DownloadInvoicesCsv([FromQuery] int[] invoiceIds)
        {
            ICsvDocument csv = _documentFactory.GetInvoicesCsv(invoiceIds);

            if (csv == null)
            {
                throw new ArgumentException("Error occurs when generating csv file.");
            }
            else
            {
                _invoiceCommandFactory.UpdateInvoiceExportedTime(invoiceIds).Execute();
                return new FileContentResult(csv.Content, MimeTypes.CSV)
                {
                    FileDownloadName = $"{csv.Title}.{csv.FileExtension}"
                };
            }
        }

        [HttpGet("invoices/{id}", Name = "GetInvoiceById")]
        public InvoiceDetailViewModel GetInvoiceById([FromRoute] int id)
        {
            ICommand<InvoiceDetailViewModel> command = _invoiceCommandFactory.GetInvoiceDetail(id);
            command.Execute();
            return command.ResultData;
        }

        [HttpPost("invoices/{id}/calculate-total")]
        public IActionResult CalculateInvoiceTotal([FromRoute] int id)
        {
            ICommand command = _invoiceCommandFactory.UpdateInvoiceTotal(id);
            command.Execute();
            return Ok();
        }

        [HttpPut("invoices/{id}")]
        public InvoiceDetailViewModel UpdateInvoice([FromBody] DtoUpdateInvoiceInput dtoInvoice, [FromRoute] int id)
        {
            Invoice invoice = _mapper.Map<Invoice>(dtoInvoice);
            ICommand command = _invoiceCommandFactory.UpdateInvoice(invoice);
            command.Execute();

            ICommand<InvoiceDetailViewModel> getCommand = _invoiceCommandFactory.GetInvoiceDetail(id);
            getCommand.Execute();
            return getCommand.ResultData;
        }

        [HttpGet("download-invoice-detail/{invoiceId}")]
        public IActionResult DownloadInvoiceDetail([FromRoute] int invoiceId)
        {
            IPdfDocument pdf = _documentFactory.GetInvoiceDetailPdf(invoiceId);

            if (pdf == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new FileContentResult(pdf.Content, MimeTypes.PDF);
            }
        }

        [AllowAnonymous]
        [HttpGet("preview-invoice-detail/{invoiceId}")]
        public IActionResult PreviewInvoiceDetail([FromRoute] int invoiceId)
        {
            var html = _documentFactory.GetInvoiceDetailHtml(invoiceId);

            return new ContentResult { Content = html, ContentType = MimeTypes.HTML };
        }
    }
}