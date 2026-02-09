using System;
using System.Collections.Generic;

namespace Insure__Insurance_Company_.Models;

public class NewsletterSubscription
{
    public int Id { get; set; }
    public string Email { get; set; }
    public DateTime SubscribedAt { get; set; } = DateTime.Now;
}


