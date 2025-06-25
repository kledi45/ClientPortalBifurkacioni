namespace ClientPortalBifurkacioni.Models.Entities
{
    public class PublicUsers : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        public DateTime InsertionDate { get; set; }
        public string? First { get; set; }
        public string? Last { get; set; }
        public string? PersonalNumber { get; set; }
        public string? CompleteName { get; set; }
        public string? BusinessNumber { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public bool IsConnectedToCustomer { get; set; }
        public bool IsIndividual { get; set; }
        public string? Session { get; set; }
        public int? IDState { get; set; }
        public DateTime? LastSignInDate { get; set; }
        public DateTime? LastSignOutDate { get; set; }
        public string? Token { get; set; }
        public DateTime? TokenGenerateDate { get; set; }
        public DateTime? TokenExpireDate { get; set; }
        public int? ResetPasswordTries { get; set; }
        public string? FirebaseToken { get; set; }
        public string? PhoneNumber2 { get; set; }
        public bool? IsVerified { get; set; }
    }
}
