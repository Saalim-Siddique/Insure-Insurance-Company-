using System;
using System.Collections.Generic;

namespace Insure__Insurance_Company_.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int UserPolicyId { get; set; }

    public DateOnly? PaymentDate { get; set; }

    public decimal? AmountPaid { get; set; }

    public string? PaymentMode { get; set; }

    public int PaymentMonth { get; set; }
    public int PaymentYear { get; set; }

    public virtual UserPolicy UserPolicy { get; set; } = null!;
}
