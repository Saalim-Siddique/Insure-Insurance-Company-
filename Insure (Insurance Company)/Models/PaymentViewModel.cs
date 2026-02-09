namespace Insure__Insurance_Company_.Models
{
    public class PaymentViewModel
    {
        public DateOnly PaymentDate { get; set; }
        public decimal AmountPaid { get; set; }
        public string PaymentMode { get; set; }
    }

}
