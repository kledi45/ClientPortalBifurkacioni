using Microsoft.AspNetCore.Mvc;

namespace ClientPortalBifurkacioni.Controllers
{
    public class PrivacyPolicyController : Controller
    {
        public ActionResult Index(string language)
        {
            switch (language)
            {
                case "sq":
                    return View("PrivacyPolicySQ");
                case "en":
                    return View();
                case "sr":
                    return View("PrivacyPolicySR");
            }
            return View();
        }
    }
}
