using ClientPortalBifurkacioni.DbConnection;
using ClientPortalBifurkacioni.Helpers;
using ClientPortalBifurkacioni.Models.Entities;
using Microsoft.Data.SqlClient;
using System;
using Microsoft.EntityFrameworkCore;
using ClientPortalBifurkacioni.Models.CustomModels;
using Dapper;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
namespace ClientPortalBifurkacioni.Repositories
{
    public class PublicUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly SmtpSettings _smtpSettings;

        public PublicUserRepository(ApplicationDbContext context, IOptions<SmtpSettings> smtpOptions)
        {
            _context = context;
            _smtpSettings = smtpOptions.Value;
        }


        public (PublicUsers? User, string Message) Login(string email, string password)
        {
            var emailParam = new SqlParameter("@Email", email);

            var user = _context.PublicUsers
                .FromSqlRaw("SELECT TOP 1 * FROM PublicUsers WHERE EmailAddress = @Email", emailParam)
                .AsEnumerable()
                .FirstOrDefault();

            if (user == null)
                return (null, "Email adresa nuk ekziston.");

            if (!PasswordHelper.Verify(user.Salt, user.Password, password))
                return (null, "Fjalëkalimi është i pasaktë.");

            return (user, "Kyçja u krye me sukses!");
        }
        public async Task<string?> Register(RegisterRequest request)
        {
            var passwordHelper = new PasswordHelper(request.Password);
            string salt = passwordHelper.Salt;
            string hashedPassword = passwordHelper.Hash;

            var cs = _context.Database.GetDbConnection().ConnectionString;

            await using var connection = new SqlConnection(cs);
            await connection.OpenAsync();

            const string registerSql = @"EXEC PublicUser_Register 
                @AccountType, 
                @FirstName, 
                @LastName, 
                @BusinessName, 
                @PersonalNumber, 
                @NUI, 
                @PhoneNumber, 
                @EmailAddress, 
                @Password, 
                @Salt";

            var result = await connection.QueryFirstOrDefaultAsync<string>(
                registerSql,
                new
                {
                    AccountType = request.AccountType,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    BusinessName = request.BusinessName,
                    PersonalNumber = request.PersonalNumber,
                    NUI = request.NUI,
                    PhoneNumber = request.PhoneNumber,
                    EmailAddress = request.Email,
                    Password = hashedPassword,
                    Salt = salt
                });

            if (string.IsNullOrEmpty(result) || result != "Success")
                return result;

            try
            {
                var user =  GetUser(connection, request.Email);
                if (user == null) return result;

                var existingCode = await GetConfirmationCode(connection, user.Id);
                string confirmationCode;

                if (existingCode is not null)
                {
                    confirmationCode = existingCode.Code;
                }
                else
                {
                    await GenerateConfirmationCode(connection, user.Id);
                    var newCode = await GetConfirmationCode(connection, user.Id);
                    confirmationCode = newCode?.Code ?? "000000";
                }

                var htmlBody = BuildConfirmationEmailBody(user.Result!.CompleteName!, int.Parse(confirmationCode));

                await SendInvoiceEmail(
                    user.Result.EmailAddress,
                    "Bifurkacioni SHA – Konfirmimi i llogarisë",
                    htmlBody
                );
            }
            catch (Exception ex)
            {
            }

            return result;
        }


        private string BuildConfirmationEmailBody(string completeName, int confirmationCode)
        {
            return $@"
            <p>I/E nderuar {System.Net.WebUtility.HtmlEncode(completeName)},</p>
            <p>Për të aktivizuar llogarinë tuaj, ju lutemi përdorni kodin e konfirmimit të mëposhtëm:</p>
            <p><strong>Kodi i konfirmimit: {confirmationCode}</strong></p>
            <p><br/>Me respekt,</p>
            <p>Bifurkacioni SHA</p>
            <p><em>Ky email është dërguar automatikisht. Ju lutemi mos përgjigjeni.</em></p>";
        }
        private async Task SendInvoiceEmail(string toEmail, string subject, string bodyHtml)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpSettings.NameFrom, _smtpSettings.EmailFrom));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = bodyHtml
            };

            message.Body = builder.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient();

            var secureOption = _smtpSettings.SmtpSecurity switch
            {
                "SSL/TLS" => SecureSocketOptions.SslOnConnect,
                "STARTTLS" => SecureSocketOptions.StartTls,
                _ => SecureSocketOptions.Auto
            };

            await client.ConnectAsync(_smtpSettings.SmtpServer, _smtpSettings.SmtpPort, secureOption);
            await client.AuthenticateAsync(_smtpSettings.SmtpUsername, _smtpSettings.SmtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task<CustomerCardListResponse> GetCustomerCardsByUserId(int userId)
        {
            var loggedUserParam = new SqlParameter("@LoggedUserId", userId);
            var customerCardList = new CustomerCardListResponse();

            var summaryResult = await _context.Set<CustomerCardDto>()
                .FromSqlRaw("EXEC GetUserCustomerCards @LoggedUserId", loggedUserParam)
                .ToListAsync();

            foreach (var customer in summaryResult)
            {
                var card = new CustomerCardResponse
                {
                    CustomerCode = customer.CustomerCode,
                    CustomerName = customer.CustomerName,
                    PropertyAddress = customer.PropertyAddress,
                    TotalDebt = customer.TotalDebt,
                    LastBillAmount = customer.LastBillAmount,
                    LastBillDate = customer.LastBillDate,
                    LastPaymentAmount = customer.LastPaymentAmount,
                    LastPaymentDate = customer.LastPaymentDate
                };

                var customerCodeParam = new SqlParameter("@CustomerCode", customer.CustomerCode);

                // Fetch meter readings
                var meterRows = await _context.Set<CustomerMeterFlatRow>()
                    .FromSqlRaw("EXEC GetCustomerMeterReadings @CustomerCode", customerCodeParam)
                    .ToListAsync();

                if (meterRows.Any())
                {
                    var first = meterRows.First();
                    card.Meter = new MeterInfo
                    {
                        SerialNumber = first.SerialNumber,
                        CurrentReading = first.CurrentReading,
                        PreviousReading = first.PreviousReading,
                        LastConsumption = first.LastConsumption,
                        LastReadingPeriod = first.LastReadingPeriod
                    };

                    card.MeterReadings = meterRows.Select(r => new MeterReading
                    {
                        Period = r.Period,
                        Reading = r.Reading,
                        Consumption = r.Consumption
                    }).ToList();
                }

                // Fetch invoices
                var invoiceRows = await _context.Set<InvoiceFlatRow>()
                    .FromSqlRaw("EXEC GetCustomerInvoices @CustomerCode", customerCodeParam)
                    .ToListAsync();

                card.Invoices = invoiceRows.Select(i => new InvoiceInfo
                {
                    Period = i.Period,
                    PaymentReference = i.PaymentReference,
                    InvoiceNumber = i.InvoiceNumber,
                    Amount = i.Amount
                }).ToList();

                // Fetch payments
                var paymentRows = await _context.Set<PaymentFlatRow>()
                    .FromSqlRaw("EXEC GetCustomerPayments @CustomerCode", customerCodeParam)
                    .ToListAsync();

                card.Payments = paymentRows.Select(p => new PaymentInfo
                {
                    Date = p.Date,
                    Bank = p.Bank,
                    Amount = p.Amount
                }).ToList();

                // Fetch expenses by year
                var expenseRows = await _context.Set<ExpenseByYear>()
                    .FromSqlRaw("EXEC GetCustomerExpensesByYear @CustomerCode", customerCodeParam)
                    .ToListAsync();

                card.Expenses = expenseRows.ToList();

                customerCardList.Customers.Add(card);
            }

            return customerCardList;
        }

        public async Task<bool> DeleteProperty(string customerCode, int userId)
        {
            var customerCodeParam = new SqlParameter("@CustomerCode", customerCode);
            var userIdParam = new SqlParameter("@LoggedUserId", userId);

            var result = (await _context.Set<ScalarIntResult>().FromSqlRaw(
                "EXEC DeleteProperty @CustomerCode, @LoggedUserId",
                customerCodeParam, userIdParam).ToListAsync()).FirstOrDefault();

            return result.Value > 0;
        }

        public async Task<RegisterPropertyResponse> RegisterCustomerCardAsync(string customerCode, string billCode, int userId)
        {
            try
            {
                var customerParam = new SqlParameter("@CustomerCode", customerCode);
                var invoiceParam = new SqlParameter("@BillCode", billCode);
                var userParam = new SqlParameter("@LoggedUserId", userId);

                var messages = new List<string>();

                var result = await _context.Set<RegisterMessage>().FromSqlRaw(
                    "EXEC RegisterUserCustomerCard @CustomerCode, @BillCode, @LoggedUserId",
                    customerParam, invoiceParam, userParam).ToListAsync();

                messages = result.Select(x => x.Message).ToList();

                return new RegisterPropertyResponse { Messages = messages };

            }
            catch(Exception ex)
            {
                throw ex;
            }
          
        }

        public async Task<PublicUsers?> GetUser(SqlConnection conn, string email)
        {
            const string sql = @"
            SELECT TOP (1) *
            FROM PublicUsers
            WHERE EmailAddress = @Email
            ORDER BY Id DESC;";

            return await conn.QueryFirstOrDefaultAsync<PublicUsers>(sql, new { Email = email });
        }


        public async Task<EmailConfirmationCode?> GetConfirmationCode(SqlConnection conn, int userId)
        {
            const string sql = @"
                SELECT TOP (1) *
                FROM EmailConfirmationCode
                WHERE IdPublicUser = @Id AND IsActive = 1
                ORDER BY Id DESC;";

            return await conn.QueryFirstOrDefaultAsync<EmailConfirmationCode>(sql, new { Id = userId });
        }

        public async Task<bool> UpdateCode(int codeId)
        {
            using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
            await connection.OpenAsync();

            var sql = @"UPDATE EmailConfirmationCode 
                SET UsedDate = GETDATE(), IsActive = 0 
                WHERE ID = @Id";

            var affected = await connection.ExecuteAsync(sql, new { Id = codeId });
            return affected > 0;
        }

        public async Task<int> GenerateConfirmationCode(SqlConnection conn, int userId)
        {
            var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

            const string sql = @"
            INSERT INTO EmailConfirmationCode (IdPublicUser, Code, CreatedDate, IsActive)
            VALUES (@UserId, @Code, GETDATE(), 1);
            SELECT CAST(SCOPE_IDENTITY() as int);";

            return await conn.QuerySingleAsync<int>(sql, new { UserId = userId, Code = code });
        }

    }
}
