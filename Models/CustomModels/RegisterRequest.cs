namespace ClientPortalBifurkacioni.Models.CustomModels
{
    public class RegisterRequest
    {
        public string AccountType { get; set; } = string.Empty;          
        public string FirstName { get; set; } = string.Empty;            
        public string LastName { get; set; } = string.Empty;             
        public string BusinessName { get; set; } = string.Empty;         
        public string PersonalNumber { get; set; } = string.Empty;       
        public string NUI { get; set; } = string.Empty;                  
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

}
