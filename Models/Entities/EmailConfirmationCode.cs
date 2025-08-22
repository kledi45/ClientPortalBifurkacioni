namespace ClientPortalBifurkacioni.Models.Entities
{
    public class EmailConfirmationCode
    {
        public int ID { get; set; }
        public int IDPublicUser { get; set; }
        public string Code { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? UsedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
