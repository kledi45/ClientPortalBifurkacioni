namespace ClientPortalBifurkacioni.Models.CustomModels
{
    public class SubmitPaymentRequest
    {
        public string CustomerCode { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Amount { get; set; }
    }

}
