using System;
using System.Collections.Generic;

namespace Insure__Insurance_Company_.Models;

public partial class Loan
{
    public int LoanId { get; set; }
    public int UserPolicyId { get; set; }
    public decimal? LoanAmount { get; set; }
    public decimal? InterestRate { get; set; }
    public DateOnly? LoanDate { get; set; }
    public string? Status { get; set; }
    public virtual UserPolicy UserPolicy { get; set; } = null!;
}
