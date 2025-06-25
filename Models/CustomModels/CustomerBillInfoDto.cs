namespace ClientPortalBifurkacioni.Models.CustomModels
{
    public class CustomerBillInfoDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public decimal LastBillAmount { get; set; }
        public string? LastBillDate { get; set; }
        public decimal LastPaymentAmount { get; set; }
        public string? LastPaymentDate { get; set; }
        public decimal TotalDebt { get; set; }
    }
}
