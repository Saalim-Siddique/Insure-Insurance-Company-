using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Insure__Insurance_Company_.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Role { get; set; }

    public DateTime? CreatedAt { get; set; }

    [Required(ErrorMessage = "Date of Birth is required")]
    [DataType(DataType.Date)]  
    public DateTime? DateOfBirth { get; set; }

    public virtual ICollection<UserPolicy> UserPolicies { get; set; } = new List<UserPolicy>();
}
