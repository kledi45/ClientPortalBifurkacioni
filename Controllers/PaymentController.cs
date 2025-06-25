using ClientPortalBifurkacioni.DbConnection;
using ClientPortalBifurkacioni.Helpers;
using ClientPortalBifurkacioni.Models.CustomModels;
using ClientPortalBifurkacioni.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ClientPortalBifurkacioni.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PublicRepository _repo;
        private readonly PaymentHelper _paymentHelper;

        public PaymentController(ApplicationDbContext context, PaymentHelper paymentHelper)
        {
            _repo = new PublicRepository(context);
            _paymentHelper = paymentHelper;
        }
        [HttpPost]
        public async Task<IActionResult> SubmitPayment([FromQuery] string amount, [FromQuery] string code,[FromQuery] string email,
            [FromQuery] string phone)
        {
            try
            {
                var transactionResult = await _paymentHelper.RouteTransactionToQuipu(
                    "Pagesë për faturë",
                    amount,
                    "EUR",
                    code,
                    email,
                    phone,
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

        public IActionResult SuccessNew(string customerCode, string amount)
        {
            ViewBag.CustomerCode = customerCode;
            ViewBag.Amount = amount;
            ViewBag.PaymentDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            return View();
        }

        public IActionResult Preview(string customerCode, decimal amount)
        {
            ViewBag.CustomerCode = customerCode;
            ViewBag.Amount = amount;
            return View();
        }
        public IActionResult Failed() => View();


    }
}
