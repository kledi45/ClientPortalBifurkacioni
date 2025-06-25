using Azure.Core;
using ClientPortalBifurkacioni.DbConnection;
using ClientPortalBifurkacioni.Models.CustomModels;
using ClientPortalBifurkacioni.Repositories;
using Newtonsoft.Json;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

public class PaymentHelper
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;
    private readonly ILogger<PaymentHelper> _logger;
    private readonly PublicRepository _repo;

    public PaymentHelper(IWebHostEnvironment env, IConfiguration config, ILogger<PaymentHelper> logger, ApplicationDbContext context)
    {
        _env = env;
        _config = config;
        _logger = logger;
        _repo = new PublicRepository(context);
    }

    public async Task<TransactionResponse> RouteTransactionToQuipu(
        string orderDesc,
        string orderAmount,
        string orderCurrency,
        string customerCode,
        string email,
        string number,
        int insertedPaymentId)
    {
        try
        {
            string storeKey = _config["PaymentSettings:PCB_StoreKey"];
            string storePass = _config["PaymentSettings:PCB_StorePass"];
            string storeURL = _config["PaymentSettings:PCB_StoreURL"];
            string successCallbackURL = _config["PaymentSettings:SuccessCallBackURL"];

            if (string.IsNullOrWhiteSpace(storeKey) || string.IsNullOrWhiteSpace(storePass) || string.IsNullOrWhiteSpace(storeURL))
            {
                _logger.LogWarning("Missing payment configuration settings.");
                return new TransactionResponse { Status = "CONFIG_ERROR", Url = "" };
            }

            var certPath = Path.Combine("C:\\", $"{storeKey}.pfx");
            var cert = new X509Certificate2(certPath, storePass, X509KeyStorageFlags.MachineKeySet);

            var hppRedirectUrl = successCallbackURL + "?customerCode=" + customerCode + "&amount=" + orderAmount;

            var payload = new
            {
                order = new
                {
                    typeRid = "1", // Changed from "1" to "ORD1" per document
                    amount = orderAmount,
                    currency = orderCurrency,
                    description = orderDesc,
                    language = "en",
                    hppRedirectUrl = hppRedirectUrl,
                    initiationEnvKind = "Browser", // Added per document
                    consumerDevice = new
                    {
                        browser = new
                        {
                            javaEnabled = true, // Aligned with document
                            jsEnabled = true,
                            acceptHeader = "application/json, application/jose; charset=utf-8", // Aligned with document
                            ip = "127.0.0.1",
                            colorDepth = 24,
                            screenW = 1080, // Aligned with document
                            screenH = 1920, // Aligned with document
                            tzOffset = -300, // Aligned with document
                            language = "en-EN", // Aligned with document
                            userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.80 Safari/537.36" // Aligned with document
                        }
                    }
                }
            };

            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(cert);
            handler.SslProtocols = SslProtocols.Tls12;

            using var client = new HttpClient(handler);
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(storeURL, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            var logDir = Path.Combine(_env.WebRootPath, "logs", "quipu-responses");
            Directory.CreateDirectory(logDir);
            var logFile = Path.Combine(logDir, $"response_{DateTime.Now:yyyyMMdd_HHmmss}.json");
            await File.WriteAllTextAsync(logFile, responseBody);

            // Check for error response
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Quipu Create Order failed. Status: {StatusCode}, Response: {ResponseBody}", response.StatusCode, responseBody);
                try
                {
                    var errorResponse = JsonConvert.DeserializeObject<dynamic>(responseBody);
                    return new TransactionResponse
                    {
                        Status = "ERROR",
                        Url = "",
                        ErrorMessage = $"{errorResponse?.ErrorCode}: {errorResponse?.ErrorDescription}"
                    };
                }
                catch
                {
                    return new TransactionResponse
                    {
                        Status = "ERROR",
                        Url = "",
                        ErrorMessage = $"HTTP {response.StatusCode}: {responseBody}"
                    };
                }
            }

            var data = JsonConvert.DeserializeObject<QuipuResponse>(responseBody);

            if (data?.Order != null)
            {
                var hppUrl = $"{data.Order.HppUrl}?id={data.Order.Id}&password={data.Order.Password}";
                 await _repo.InsertInitializedPaymentPublic(orderAmount, customerCode, number, email,data.Order.Id);
                _repo.InsertPaymentInitializationData(data.Order.Id, data.Order.Password!, customerCode);
                return new TransactionResponse
                {
                    Status = "OK",
                    Url = hppUrl,
                    OrderId = data.Order.Id,
                    OrderPassword = data.Order.Password,
                };
            }

            _logger.LogWarning("Invalid Quipu response: {ResponseBody}", responseBody);
            return new TransactionResponse { Status = "BAD", Url = "", ErrorMessage = "Invalid or empty response from Quipu." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Quipu initialization failed.");
            return new TransactionResponse { Status = "EXP", Url = "", ErrorMessage = $"Exception: {ex.Message}" };
        }
    }
    public async Task<TransactionResponse> GetOrderDetailsFromQuipu(int orderId, string password)
    {
        try
        {
            string storeKey = _config["PaymentSettings:PCB_StoreKey"];
            string storePass = _config["PaymentSettings:PCB_StorePass"];
            string getOrderURL = $"{_config["PaymentSettings:PCB_GetOrderDetailsURL"]}/{orderId}?password={password}";

            if (string.IsNullOrWhiteSpace(storeKey) || string.IsNullOrWhiteSpace(storePass) || string.IsNullOrWhiteSpace(getOrderURL))
            {
                _logger.LogWarning("Missing payment configuration settings for GetOrderDetails.");
                return new TransactionResponse { Status = "CONFIG_ERROR" };
            }

            var certPath = Path.Combine("C:\\", $"{storeKey}.pfx");
            var cert = new X509Certificate2(certPath, storePass, X509KeyStorageFlags.MachineKeySet);

            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(cert);
            handler.SslProtocols = SslProtocols.Tls12;

            using var client = new HttpClient(handler);
            var response = await client.GetAsync(getOrderURL);
            var body = await response.Content.ReadAsStringAsync();

            var logDir = Path.Combine(_env.WebRootPath, "logs", "quipu-order-details");
            Directory.CreateDirectory(logDir);
            var logFile = Path.Combine(logDir, $"order_{orderId}_{DateTime.Now:yyyyMMdd_HHmmss}.json");
            await File.WriteAllTextAsync(logFile, body);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("GetOrderDetails failed with status code: {StatusCode}, Response: {Body}", response.StatusCode, body);
                return new TransactionResponse { Status = "HTTP_ERROR", ErrorMessage = $"HTTP {response.StatusCode}" };
            }

            var result = JsonConvert.DeserializeObject<QuipuOrderDetailsResponse>(body);
            if (result?.Order != null)
            {
                string transactionStatus = result.Order.Status switch
                {
                    "FullyPaid" => "SUCCESS",
                    "Failed" => "FAILED",
                    _ => "FAILED"
                };

                return new TransactionResponse
                {
                    Status = transactionStatus,
                    OrderId = result.Order.Id,
                    Amount = result.Order.Amount?.ToString("F2"),
                    Currency = result.Order.Currency,
                    ApprovalCode = result.Order.ApprovalCode,
                    CardBrand = result.Order.CardBrand,
                    MaskedPan = result.Order.MaskedPan,
                    CreateTime = result.Order.CreateTime,
                    ErrorMessage = transactionStatus == "FAILED" ? "Payment failed." : null
                };
            }

            _logger.LogWarning("No order details found in Quipu response: {Body}", body);
            return new TransactionResponse { Status = "NO_DATA", ErrorMessage = "No order details returned." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get order details from Quipu for order {OrderId}.", orderId);
            return new TransactionResponse { Status = "ERROR", ErrorMessage = $"Failed to retrieve order details: {ex.Message}" };
        }
    }
}