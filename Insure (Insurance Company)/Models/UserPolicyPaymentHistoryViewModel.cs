using System.Collections.Generic;
namespace Insure__Insurance_Company_.Models

{
    public class UserPolicyPaymentHistoryViewModel
    {
        public int UserPolicyId { get; set; }
        public string PolicyName { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Status { get; set; }
        public int DurationInMonths { get; set; }
        public int PremiumsPaidCount { get; set; }
        public int PremiumsLeftCount { get; set; }
        public List<PaymentViewModel> Payments { get; set; }
    }
}
