using ClientPortalBifurkacioni.DbConnection;
using ClientPortalBifurkacioni.Helpers;
using ClientPortalBifurkacioni.Models.Entities;
using Microsoft.Data.SqlClient;
using System;
using Microsoft.EntityFrameworkCore;
using ClientPortalBifurkacioni.Models.CustomModels;
using Dapper;
namespace ClientPortalBifurkacioni.Repositories
{
    public class PublicUserRepository
    {
        private readonly ApplicationDbContext _context;

        public PublicUserRepository(ApplicationDbContext context)
        {
            _context = context;
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

            if (!PasswordHelper.VerifyPassword(password, user.Salt, user.Password))
                return (null, "Fjalëkalimi është i pasaktë.");

            return (user, "Kyçja u krye me sukses!");
        }
        public async Task<string?> Register(RegisterRequest request)
        {
            string salt = PasswordHelper.GenerateSalt();
            string hashedPassword = PasswordHelper.HashPassword(request.Password, salt);

            using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                await connection.OpenAsync();

                var query = "EXEC PublicUser_Register @FirstName, @LastName, @PersonalNumber, @PhoneNumber, @EmailAddress, @Password, @Salt";

                var result = await connection.QueryFirstOrDefaultAsync<string>(query, new
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PersonalNumber = request.PersonalNumber,
                    PhoneNumber = request.PhoneNumber,
                    EmailAddress = request.Email,
                    Password = hashedPassword,
                    Salt = salt
                });

                return result; 
            }
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

       


    }
}
