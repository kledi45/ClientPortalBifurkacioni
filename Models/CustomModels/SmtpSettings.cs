namespace ClientPortalBifurkacioni.Models.CustomModels
{
    public class SmtpSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;
        public string SmtpSecurity { get; set; } = string.Empty;
        public string EmailFrom { get; set; } = string.Empty;
        public string NameFrom { get; set; } = string.Empty;
        public string ReplyTo { get; set; } = string.Empty;
    }
}
