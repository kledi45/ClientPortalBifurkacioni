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
        [HttpGet]
        public async Task<IActionResult> MakeDirectPayment(
            [FromQuery] string amount,
            [FromQuery] string code,
            [FromQuery] string email,
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
                    return Redirect(transactionResult.Url);
                }

                return Content("<h2 style='color:red'>Nuk u inicializua pagesa.</h2>", "text/html");
            }
            catch (Exception ex)
            {
                return Content($"<h2 style='color:red'>Gabim në server: {ex.Message}</h2>", "text/html");
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
