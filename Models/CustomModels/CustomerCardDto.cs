namespace ClientPortalBifurkacioni.Models.CustomModels
{
    public class CustomerCardDto
    {
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public string? PropertyAddress { get; set; }
        public decimal TotalDebt { get; set; }
        public decimal LastBillAmount { get; set; }
        public string? LastBillDate { get; set; }
        public decimal LastPaymentAmount { get; set; }
        public string? LastPaymentDate { get; set; }
    }
}
