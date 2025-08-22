namespace ClientPortalBifurkacioni.Models.CustomModels
{
    public class MeterInfo
    {
        public string? SerialNumber { get; set; }
        public decimal CurrentReading { get; set; }
        public decimal PreviousReading { get; set; }
        public decimal LastConsumption { get; set; }
        public string? LastReadingPeriod { get; set; }
    }
}
