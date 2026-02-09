using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Insure__Insurance_Company_.Models
{
    public partial class InsuranceType
    {
        public int InsuranceTypeId { get; set; }

        [Required(ErrorMessage = "Insurance Name is required")]
        public string? InsuranceName { get; set; }

        [Required(ErrorMessage = "Insurance Description is required")]
        public string? Description { get; set; }

        public virtual ICollection<Policy> Policies { get; set; } = new List<Policy>();
    }
}
