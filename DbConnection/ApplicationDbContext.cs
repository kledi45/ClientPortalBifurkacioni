using System.Collections.Generic;
using System.Reflection.Emit;
using System;
using ClientPortalBifurkacioni.Models.Entities;
using Microsoft.EntityFrameworkCore;
using ClientPortalBifurkacioni.Models.CustomModels;

namespace ClientPortalBifurkacioni.DbConnection
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<PublicUsers> PublicUsers { get; set; }
        public DbSet<CustomerBillInfoDto> CustomerBillInfoDto { get; set; }
        public DbSet<ScalarIntResult> ScalarIntResult { get; set; }
        public DbSet<CustomerCardDto> CustomerCardDto { get; set; }
        public DbSet<InvoiceFlatRow> InvoiceFlatRows { get; set; }
        public DbSet<PaymentFlatRow> PaymentFlatRows { get; set; }
        public DbSet<CustomerMeterFlatRow> CustomerMeterFlatRow { get; set; }
        public DbSet<ExpenseByYear> ExpenseByYear { get; set; }
        public DbSet<RegisterMessage> RegisterMessages { get; set; }
        public DbSet<StringResult> StringResult { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerBillInfoDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<ScalarIntResult>().HasNoKey().ToView(null);
            modelBuilder.Entity<CustomerCardDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<PaymentFlatRow>().HasNoKey().ToView(null);
            modelBuilder.Entity<InvoiceFlatRow>().HasNoKey().ToView(null);
            modelBuilder.Entity<CustomerMeterFlatRow>().HasNoKey().ToView(null);
            modelBuilder.Entity<ExpenseByYear>().HasNoKey().ToView(null);
            modelBuilder.Entity<RegisterMessage>().HasNoKey().ToView(null);
            modelBuilder.Entity<StringResult>().HasNoKey().ToView(null);

            modelBuilder.Entity<PublicUsers>(entity =>
            {
                entity.ToTable("PublicUsers");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Username).HasColumnName("Username");
                entity.Property(e => e.Password).HasColumnName("Password");
                entity.Property(e => e.Salt).HasColumnName("Salt");
                entity.Property(e => e.InsertionDate).HasColumnName("InsertionDate");
                entity.Property(e => e.First).HasColumnName("First");
                entity.Property(e => e.Last).HasColumnName("Last");
                entity.Property(e => e.PersonalNumber).HasColumnName("PersonalNumber");
                entity.Property(e => e.CompleteName).HasColumnName("CompleteName");
                entity.Property(e => e.BusinessNumber).HasColumnName("BusinessNumber");
                entity.Property(e => e.PhoneNumber).HasColumnName("PhoneNumber");
                entity.Property(e => e.EmailAddress).HasColumnName("EmailAddress");
                entity.Property(e => e.IsConnectedToCustomer).HasColumnName("IsConnectedToCustomer");
                entity.Property(e => e.IsIndividual).HasColumnName("IsIndividual");
                entity.Property(e => e.Session).HasColumnName("Session");
                entity.Property(e => e.IDState).HasColumnName("IDState");
                entity.Property(e => e.LastSignInDate).HasColumnName("LastSignInDate");
                entity.Property(e => e.LastSignOutDate).HasColumnName("LastSignOutDate");
                entity.Property(e => e.Token).HasColumnName("Token");
                entity.Property(e => e.TokenGenerateDate).HasColumnName("TokenGenerateDate");
                entity.Property(e => e.TokenExpireDate).HasColumnName("TokenExpireDate");
                entity.Property(e => e.ResetPasswordTries).HasColumnName("ResetPasswordTries");
                entity.Property(e => e.FirebaseToken).HasColumnName("FirebaseToken");
                entity.Property(e => e.PhoneNumber2).HasColumnName("PhoneNumber2");
                entity.Property(e => e.IsVerified).HasColumnName("IsVerified");
            });
        }
    }
    
}
