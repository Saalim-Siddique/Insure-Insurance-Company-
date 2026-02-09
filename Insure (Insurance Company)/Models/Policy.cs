using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Insure__Insurance_Company_.Models;

public partial class Policy
{
    public int PolicyId { get; set; }

    [Required(ErrorMessage = "Insurance Type is required")]
    public int InsuranceTypeId { get; set; }

    [Required(ErrorMessage = "Policy Name is required")]
    public string PolicyName { get; set; } = null!;

    public int? TermYears { get; set; }

    public decimal? PremiumAmount { get; set; }

    public decimal? CoverageAmount { get; set; }

    public int DurationInMonths { get; set; }

    public virtual InsuranceType InsuranceType { get; set; } = null!;

    public virtual ICollection<UserPolicy> UserPolicies { get; set; } = new List<UserPolicy>();
}




