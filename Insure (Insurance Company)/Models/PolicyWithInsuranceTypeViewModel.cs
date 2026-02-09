using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Insure__Insurance_Company_.Models
{
    public class PolicyWithInsuranceTypeViewModel
    {
        // Policy fields
        public int PolicyId { get; set; }

        [Required(ErrorMessage = "Please select an Insurance Type.")]
        public int? InsuranceTypeId { get; set; }  // Selected existing insurance type

        [Required(ErrorMessage = "Policy Name is required.")]
        public string? PolicyName { get; set; }

        [Required(ErrorMessage = "Term Years is required.")]
        public int? TermYears { get; set; }

        [Required(ErrorMessage = "Premium Amount is required.")]
        public decimal? PremiumAmount { get; set; }

        [Required(ErrorMessage = "Coverage Amount is required.")]
        public decimal? CoverageAmount { get; set; }

        // InsuranceType fields (for new InsuranceType)
        public string? NewInsuranceName { get; set; }

        public string? NewDescription { get; set; }

        public int DurationInMonth { get; set; }

        public IEnumerable<SelectListItem>? InsuranceTypesList { get; set; }
    }
}
