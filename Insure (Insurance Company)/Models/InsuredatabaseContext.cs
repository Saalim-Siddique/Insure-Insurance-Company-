using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Insure__Insurance_Company_.Models;

public partial class InsuredatabaseContext : DbContext
{
    public InsuredatabaseContext()
    {
    }

    public InsuredatabaseContext(DbContextOptions<InsuredatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<InsuranceType> InsuranceTypes { get; set; }

    public virtual DbSet<Loan> Loans { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Policy> Policies { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPolicy> UserPolicies { get; set; }

    public virtual DbSet<NewsletterSubscription> NewsletterSubscriptions { get; set; }


    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Server=DESKTOP-T5FBAG3\\MSSQLSERVER01;Database=insuredatabase;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InsuranceType>(entity =>
        {
            entity.HasKey(e => e.InsuranceTypeId).HasName("PK__Insuranc__19DDD89BB1D5E9A1");

            entity.Property(e => e.InsuranceTypeId).HasColumnName("insurance_type_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.InsuranceName)
                .HasMaxLength(50)
                .HasColumnName("insurance_name");
        });

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(e => e.LoanId).HasName("PK__Loans__A1F79554AE68C4CC");

            entity.Property(e => e.LoanId).HasColumnName("loan_id");
            entity.Property(e => e.InterestRate)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("interest_rate");
            entity.Property(e => e.LoanAmount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("loan_amount");
            entity.Property(e => e.LoanDate).HasColumnName("loan_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.UserPolicyId).HasColumnName("user_policy_id");

            entity.HasOne(d => d.UserPolicy).WithMany(p => p.Loans)
                .HasForeignKey(d => d.UserPolicyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Loans_UserPolicies");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__ED1FC9EA825DA7EC");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.AmountPaid)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount_paid");
            entity.Property(e => e.PaymentDate).HasColumnName("payment_date");
            entity.Property(e => e.PaymentMode)
                .HasMaxLength(50)
                .HasColumnName("payment_mode");
            entity.Property(e => e.UserPolicyId).HasColumnName("user_policy_id");

            entity.HasOne(d => d.UserPolicy).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserPolicyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_UserPolicies");
        });

        modelBuilder.Entity<Policy>(entity =>
        {
            entity.HasKey(e => e.PolicyId).HasName("PK__Policies__47DA3F0302335A19");

            entity.Property(e => e.PolicyId).HasColumnName("policy_id");
            entity.Property(e => e.CoverageAmount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("coverage_amount");
            entity.Property(e => e.InsuranceTypeId).HasColumnName("insurance_type_id");
            entity.Property(e => e.PolicyName)
                .HasMaxLength(100)
                .HasColumnName("policy_name");
            entity.Property(e => e.PremiumAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("premium_amount");
            entity.Property(e => e.TermYears).HasColumnName("term_years");

            entity.HasOne(d => d.InsuranceType).WithMany(p => p.Policies)
                .HasForeignKey(d => d.InsuranceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Policies_InsuranceTypes");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370FDCA9CD82");

            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.HasIndex(e => e.FullName, "UQ_Users_Full_Name").IsUnique();

            entity.HasIndex(e => e.Phone, "UQ_Users_Phone").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("USER")
                .HasColumnName("role");
        });

        modelBuilder.Entity<UserPolicy>(entity =>
        {
            entity.HasKey(e => e.UserPolicyId).HasName("PK__UserPoli__7002C3A9EC4A5430");

            entity.Property(e => e.UserPolicyId).HasColumnName("user_policy_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.PolicyId).HasColumnName("policy_id");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Policy).WithMany(p => p.UserPolicies)
                .HasForeignKey(d => d.PolicyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPolicies_Policies");

            entity.HasOne(d => d.User).WithMany(p => p.UserPolicies)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserPolicies_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
