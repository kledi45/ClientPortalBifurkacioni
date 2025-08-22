using Newtonsoft.Json;

namespace ClientPortalBifurkacioni.Models.CustomModels
{
    public class TransactionResponse
    {
        public string? Status { get; set; } // Maps to Quipu order status (e.g., "SUCCESS", "FAILED", "PENDING")
        public string? Url { get; set; } // HPP URL for redirect (e.g., "https://3dss2test.quipu.de:8009/flex?id=1027&password=xxx")
        public int? OrderId { get; set; } // Changed to string to match Quipu's "id" field (e.g., "1027")
        public string? OrderPassword { get; set; } // Order password from Quipu response
        public string? ApprovalCode { get; set; } // Approval code from Quipu (if provided)
        public string? CardBrand { get; set; } // Card brand (e.g., Visa, MasterCard)
        public string? MaskedPan { get; set; } // Masked card number
        public string? CustomerCode { get; set; } // Customer code passed from controller
        public string? Amount { get; set; } // Order amount (e.g., "10.00")
        public string? Currency { get; set; } // Order currency (e.g., "EUR")
        public string? CreateTime { get; set; } // Order creation time (e.g., "2023-09-26 14:35:14")
        public string? PaymentDate { get; set; } // Payment date set by controller (e.g., "26.09.2023 14:35")
        public string? ErrorMessage { get; set; } // Error message for failed transactions
        public string? LastFourDigits { get; set; }

    }


    public class QuipuResponse
    {
        [JsonProperty("order")]
        public QuipuInitOrder? Order { get; set; }
    }

    public class QuipuInitOrder
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("password")]
        public string? Password { get; set; }

        [JsonProperty("hppUrl")]
        public string? HppUrl { get; set; }
    }

    public class QuipuOrderDetailsResponse
    {
        public QuipuOrder Order { get; set; }
    }

    public class QuipuOrder
    {
        public int? Id { get; set; }
        public string? TypeRid { get; set; }
        public string? Status { get; set; }
        public string? PrevStatus { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
        public string? CreateTime { get; set; }
        public QuipuOrderType Type { get; set; }

        public List<QuipuTransaction> Trans { get; set; }
        public QuipuSrcToken SrcToken { get; set; }
    }

    public class QuipuOrderType
    {
        public string? Title { get; set; }
    }

    public class QuipuTransaction
    {
        public string? ApprovalCode { get; set; }
        public string? Description { get; set; }
    }

    public class QuipuSrcToken
    {
        public string? PaymentMethod { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
        public string? RegTime { get; set; }
        public string? EntryMode { get; set; }
        public string? DisplayName { get; set; }
        public QuipuCard Card { get; set; }
        public QuipuOwner Owner { get; set; }
    }

    public class QuipuCard
    {
        public QuipuAuthentication Authentication { get; set; }
        public string? Expiration { get; set; }
        public string? Brand { get; set; }
        public string? IssuerRid { get; set; }
    }

    public class QuipuAuthentication
    {
        public bool? NeedCvv2 { get; set; }
        public bool? NeedTds { get; set; }
    }

    public class QuipuOwner
    {
        public string? Name { get; set; }
    }
}
