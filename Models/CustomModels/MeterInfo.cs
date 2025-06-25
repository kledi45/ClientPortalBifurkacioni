namespace ClientPortalBifurkacioni.Models.CustomModels
{
    public class MeterInfo
    {
        public string? SerialNumber { get; set; }
        public int CurrentReading { get; set; }
        public int PreviousReading { get; set; }
        public int LastConsumption { get; set; }
        public string? LastReadingPeriod { get; set; }
    }
}
