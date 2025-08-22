using ClientPortalBifurkacioni.DbConnection;
using ClientPortalBifurkacioni.Models;
using ClientPortalBifurkacioni.Models.CustomModels;
using ClientPortalBifurkacioni.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace ClientPortalBifurkacioni.Controllers
{
    public class HomeController : Controller
    {
        private readonly PublicUserRepository _repo;

        public HomeController(ApplicationDbContext context, IOptions<SmtpSettings> smtpOptions)
        {
            _repo = new PublicUserRepository(context,smtpOptions);
        }

        public async Task<IActionResult> Index()
        {
            int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0");

            var result = await _repo.GetCustomerCardsByUserId(userId);

            return View(result); 
        }

        public async Task<IActionResult> Properties()
        {
            int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0");

            var result = await _repo.GetCustomerCardsByUserId(userId);

            return View(result);
        }

        public async Task<IActionResult> DeleteProperty(string CustomerCode)
        {
            int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0");
            var result = await _repo.DeleteProperty(CustomerCode, userId);
            return Json(new { success = true });
        }

        public IActionResult CRM() => View();
        public async Task<IActionResult> RegisterProperty([FromForm] RegisterPropertyRequest request)
        {
            int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0");

            var result = await _repo.RegisterCustomerCardAsync(request.CustomerCode, request.BillCode, userId);

            return Json(new
            {
                success = result.Success,
                messages = result.Messages
            });
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [AllowAnonymous]
        public IActionResult Authenticate()
        {
            return View();
        }

        //public async Task<IActionResult> GetUserCard()
        //{
        //    int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0");
        //    var customers = await _repo.GetCustomerCardsByUserId(userId);
        //    return Ok(new { success = true, data = customers });
        //}

    }
}
