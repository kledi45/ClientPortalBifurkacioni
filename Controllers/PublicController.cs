using ClientPortalBifurkacioni.DbConnection;
using ClientPortalBifurkacioni.Helpers;
using ClientPortalBifurkacioni.Models.CustomModels;
using ClientPortalBifurkacioni.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;

namespace ClientPortalBifurkacioni.Controllers
{
    public class PublicController : Controller
    {
        private readonly PublicRepository _repo;
        private readonly PaymentHelper _paymentHelper;
        private readonly SmtpSettings _smtpSettings;

        public PublicController(ApplicationDbContext context, PaymentHelper paymentHelper, IOptions<SmtpSettings> smtpOptions)
        {
            _repo = new PublicRepository(context);
            _paymentHelper = paymentHelper;
            _smtpSettings = smtpOptions.Value;
        }

        public IActionResult GetCustomerBill([FromQuery] string CustomerCode)
        {
            if (string.IsNullOrWhiteSpace(CustomerCode))
                return BadRequest(new { success = false, message = "Kodi i konsumatorit mungon." });

            var data = _repo.GetCustomerBill(CustomerCode);

            if (data == null)
                return NotFound(new { success = false, message = "Konsumatori nuk u gjet." });

            return Ok(new { success = true, data });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitPayment(SubmitPaymentRequest request)
        {
            try
            {

                var transactionResult = await _paymentHelper.RouteTransactionToQuipu(
                    "Pagesë për faturë",
                    request.Amount,
                    "EUR",
                    request.CustomerCode,
                    request.Email,
                    request.Phone,
                    1
                );

                if (transactionResult.Status == "OK")
                {
                    return Ok(new { success = true, url = transactionResult.Url });
                }

                return BadRequest(new { success = false, message = "Nuk u inicializua pagesa." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Gabim në server: {ex.Message}" });
            }
        }

        public async Task<IActionResult> PaymentSuccess(string customerCode, string amount, int id)
        {
            try
            {
                
                string password = await _repo.GetPaymentPassword(id);
                string email = await _repo.GetPaymentEmail(id);
                var result = await _paymentHelper.GetOrderDetailsFromQuipu(id, password);

                if (result.Status == "SUCCESS")
                {
                    result.CustomerCode = customerCode;
                    result.Amount = amount;
                    result.PaymentDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                    await _repo.UpdatePublicPayment(id);
                    string html = GenerateInvoiceHtml(
                        id.ToString(),
                        result.PaymentDate,
                        customerCode,
                        amount,
                        result.CardBrand!,
                        result.LastFourDigits!,
                        result.ApprovalCode!
                    );
                    Task.Run(() => SendInvoiceEmail(email, "Faturë - Pagesë", html));
                    return View("~/Views/Payment/SuccessNew.cshtml", result);
                }

                return View("~/Views/Payment/Failed.cshtml", result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GenerateInvoiceHtml
        (
            string orderId, string transactionDate,string customerCode, 
            string amount,  string cardBrand, string lastFourDigits,string approvalCode
        )
        {
            return $@"
            <div style='font-family:Arial; font-size:14px; color:#000;'>
            <h4>Pagesa është kryer me sukses!</h4>
            <div style='border:1px solid #ccc; padding:15px;'>
                <p><strong>Merchant:</strong> Bifurkacioni SHA</p>
                <p><strong>Adresa:</strong> Rr.Enver Topalli, Ferizaj ,Kosovë</p>
                <p><strong>Website, Email, Telefoni:</strong> www.bifurkacioni.com, info@bifurkacioni.com</p>
                <br/>
                <table border='1' cellspacing='0' cellpadding='6'>
                    <tr><th>Order ID</th><td>#{orderId}</td></tr>
                    <tr><th>Data e Pagesës</th><td>{transactionDate}</td></tr>
                    <tr><th>Kodi i Konsumatorit</th><td>{customerCode}</td></tr>
                </table>
                <br/>
                <table border='1' cellspacing='0' cellpadding='6'>
                    <tr><th>Përshkrimi</th><th>Sasia</th><th>Çmimi</th></tr>
                    <tr><td>Pagesë për faturë për klientin {customerCode}</td><td>1</td><td>{amount}€</td></tr>
                    <tr><td colspan='2' style='text-align:right;'><strong>Totali:</strong></td><td><strong>{amount}€</strong></td></tr>
                </table>
                <br/>
                <p><strong>Mënyra e Pagesës:</strong> Me kartelë</p>
                <ul>
                    <li><strong>Data e Transaksionit:</strong> {transactionDate}</li>
                    <li><strong>Brandi i Kartelës:</strong> {cardBrand}</li>
                    <li><strong>4 Shifrat e Fundit:</strong> {lastFourDigits}</li>
                    <li><strong>Kodi i Aprovimit:</strong> {approvalCode}</li>
                </ul>
            </div>
            </div>";
        }

        public async Task SendInvoiceEmail(string toEmail, string subject, string bodyHtml)
        {
            try
            {
                var message = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.EmailFrom, _smtpSettings.NameFrom),
                    Subject = subject,
                    Body = bodyHtml,
                    IsBodyHtml = true
                };

                message.To.Add(toEmail);

                if (!string.IsNullOrEmpty(_smtpSettings.ReplyTo))
                    message.ReplyToList.Add(_smtpSettings.ReplyTo);

                using var smtp = new SmtpClient(_smtpSettings.SmtpServer, _smtpSettings.SmtpPort)
                {
                    Credentials = new NetworkCredential(_smtpSettings.SmtpUsername, _smtpSettings.SmtpPassword),
                    EnableSsl = true

                };

                await smtp.SendMailAsync(message);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
      
    }
}
