using System.Reflection;

namespace ClientPortalBifurkacioni.Models.CustomModels
{
    public class CustomerCardResponse
    {
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public string? PropertyAddress { get; set; }
        public decimal TotalDebt { get; set; }
        public decimal LastBillAmount { get; set; }
        public string? LastBillDate { get; set; }
        public decimal LastPaymentAmount { get; set; }
        public string? LastPaymentDate { get; set; }
        public MeterInfo Meter { get; set; } = new();
        public List<MeterReading> MeterReadings { get; set; } = new();
        public List<InvoiceInfo> Invoices { get; set; } = new();
        public List<PaymentInfo> Payments { get; set; } = new();
        public List<ExpenseByYear> Expenses { get; set; } = new(); 
    }

    public class CustomerMeterFlatRow
    {
        public string? SerialNumber { get; set; }        
        public int CurrentReading { get; set; }          
        public int PreviousReading { get; set; }         
        public int LastConsumption { get; set; }         
        public string? LastReadingPeriod { get; set; }   

        public string? Period { get; set; }              
        public int Reading { get; set; }                 
        public int Consumption { get; set; }             
    }

    public class InvoiceFlatRow
    {
        public string? Period { get; set; }             
        public string? PaymentReference { get; set; }   
        public string? InvoiceNumber { get; set; }      
        public decimal Amount { get; set; }             
    }

    public class PaymentFlatRow
    {
        public string? Date { get; set; }         
        public string? Bank { get; set; }         
        public decimal Amount { get; set; }       
    }

    public class ExpenseByYear
    {
        public string? Month { get; set; }
        public int Year { get; set; }
        public decimal Amount { get; set; }
    }



}
