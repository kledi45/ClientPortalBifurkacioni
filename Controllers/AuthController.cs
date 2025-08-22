using ClientPortalBifurkacioni.DbConnection;
using ClientPortalBifurkacioni.Models.CustomModels;
using ClientPortalBifurkacioni.Models.Entities;
using ClientPortalBifurkacioni.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ClientPortalBifurkacioni.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IOptions<SmtpSettings> _smtpOptions;

        public AuthController(ApplicationDbContext context, IOptions<SmtpSettings> smtpOptions)
        {
            _context = context;
            _smtpOptions = smtpOptions;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login([FromForm] LoginRequest request)
        {
            try
            {
                var repo = new PublicUserRepository(_context, _smtpOptions);
                var (user, message) = repo.Login(request.Email, request.Password);

                if (user == null)
                {
                    return Json(new { success = false, message = message ?? "Invalid login credentials." });
                }

                var token = GenerateJwtToken(user);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                };

                Response.Cookies.Append("access_token", token, cookieOptions);

                return Json(new
                {
                    success = true,
                    message = "Login successful",
                    token
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private string GenerateJwtToken(PublicUsers user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("vZ1XphGJ1g@Rxw9jT7&m$4kCpHn#V!2Lu^fKqA63BZnF5UdMNb");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("ID", user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.First + " " + user.Last),
                    new Claim(ClaimTypes.Email, user.EmailAddress ?? ""),
                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? ""),

                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public IActionResult Signout()
        {
            Response.Cookies.Delete("access_token");
            return Json(new { success = true });
        }


        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterRequest request)
        {
            var repo = new PublicUserRepository(_context, _smtpOptions);
            var result = await repo.Register(request);
            string message = result switch
            {
                "EmailAddressAlreadyExists" => "Kjo email adresë është e regjistruar më parë.",
                "PersonalNumberAlreadyExists" => "Ky numër personal është i regjistruar më parë.",
                "Success" => "Regjistrimi u krye me sukses!",
                _ => "Regjistrimi dështoi!"
            };

            return Json(new { success = result == "Success", message });
        }
    }
}
