namespace ClientPortalBifurkacioni.Models.CustomModels
{
    public class RegisterPropertyRequest
    {
        public string CustomerCode { get; set; } = string.Empty;
        public string BillCode { get; set; } = string.Empty;
    }

    public class RegisterPropertyResponse
    {
        public List<string> Messages { get; set; } = new();
        public bool Success => Messages.All(m => m.StartsWith("OK"));
    }
    public class RegisterMessage
    {
        public string Message { get; set; } = string.Empty;
    }
}
