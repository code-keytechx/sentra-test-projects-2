
namespace Sample.Api.Models
{
    public class InvoiceSummary
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ExportedDate { get; set; }
        public string ExportedBy { get; set; }
    }
}
    