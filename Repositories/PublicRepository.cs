using ClientPortalBifurkacioni.DbConnection;
using ClientPortalBifurkacioni.Models.CustomModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ClientPortalBifurkacioni.Repositories
{
    public class PublicRepository
    {
        private readonly ApplicationDbContext _context;

        public PublicRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public CustomerBillInfoDto? GetCustomerBill(string customerCode)
        {
            var param = new SqlParameter("@CustomerCode", customerCode);

            return _context.Set<CustomerBillInfoDto>()
                .FromSqlRaw("EXEC GetCustomerBill @CustomerCode", param)
                .AsEnumerable()
                .FirstOrDefault();
        }

        public ScalarIntResult? GetPublicUserIdFromCustomerCode(string customerCode)
        {
            var param = new SqlParameter("@CustomerCode", customerCode);
            return _context.Set<ScalarIntResult>()
               .FromSqlRaw("EXEC GetPublicUserId_FromCustomerCode @CustomerCode", param)
               .AsEnumerable()
               .FirstOrDefault();
        }

        public async void UpdatePublicPayment(int idPayment)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@ID", idPayment)
                };

           
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC UpdatePublicPayment @ID",
                    parameters);

            }
            catch (Exception ex)
            {
            }
        }


        public async Task<int> InsertInitializedPaymentPublic(string amount, string customerCode, string phone, string email,int idpayment)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@Amount", amount),
                    new SqlParameter("@CustomerCode", customerCode),
                    new SqlParameter("@Phone", phone ?? (object)DBNull.Value),
                    new SqlParameter("@Email", email ?? (object)DBNull.Value),
                    new SqlParameter("@IDPayment", idpayment ),

                };

                var result = await _context.Set<ScalarIntResult>()
                    .FromSqlRaw("EXEC InsertInitializedPaymentPublic @Amount, @CustomerCode, @Phone, @Email, @IDPayment", parameters)
                    .ToListAsync(); 

                return result.FirstOrDefault().Value;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        public async void InsertPaymentInitializationData(int IDPayment, string paymentPassword, string CustomerCode)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IDPayment", IDPayment),
                    new SqlParameter("@PaymentPassword", paymentPassword),
                    new SqlParameter("@CustomerCode", CustomerCode),
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC InsertPaymentInitializationData @IDPayment, @PaymentPassword, @CustomerCode",
                    parameters
                );
            }
            catch (Exception ex)
            {
            }
        }

        public async Task<string> GetPaymentPassword(int IDPayment)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IDPayment", IDPayment),
                };

                var result = await _context.Set<StringResult>()
                   .FromSqlRaw("EXEC GetPaymentPassword @IDPayment", parameters)
                   .ToListAsync();

                return result.FirstOrDefault().Value;  
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> GetPaymentEmail(int IDPayment)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IDPayment", IDPayment),
                };

                var result = await _context.Set<StringResult>()
                   .FromSqlRaw("EXEC GetPaymentEmail @IDPayment", parameters)
                   .ToListAsync();

                return result.FirstOrDefault().Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

