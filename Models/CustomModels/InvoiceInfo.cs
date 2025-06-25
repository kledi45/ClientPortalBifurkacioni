namespace ClientPortalBifurkacioni.Models.CustomModels
{
    public class InvoiceInfo
    {
        public string? Period { get; set; }              // e.g. "05-2025"
        public string? PaymentReference { get; set; }    // e.g. "P93203669094214F"
        public string? InvoiceNumber { get; set; }       // e.g. "P9320366"
        public decimal Amount { get; set; }             // e.g. 7.95
    }
}
