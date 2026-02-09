using System;
using System.Collections.Generic;

namespace Insure__Insurance_Company_.Models;

public partial class UserPolicy
{
    public int UserPolicyId { get; set; }

    public int UserId { get; set; }

    public int PolicyId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Status { get; set; }

    public decimal? FinalPremium { get; set; }

    public int? AgeAtPurchase { get; set; }

    public int PaymentsPaid { get; set; }
    public int PaymentsLeft { get; set; }

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Policy Policy { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
