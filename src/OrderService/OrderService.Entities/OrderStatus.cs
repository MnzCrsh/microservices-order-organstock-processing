using System.ComponentModel.DataAnnotations;

namespace OrderService.Entities;

public enum OrderStatus
{
    [Display(Name = "Unavailable")]
    Unavailable1 = 0,
    
    [Display(Name = "Order is created")]
    Created = 1,
    
    [Display(Name = "Order is processing")]
    Processing = 2,
    
    [Display(Name = "Processing payment")]
    PaymentPending = 3,
    
    [Display(Name = "Payment rejected")]
    PaymentRejected = 4,
    
    [Display(Name = "Payment refunded")]
    PaymentRefunded = 5,
    
    [Display(Name = "Order confirmed")]
    Confirmed = 6,
    
    [Display(Name = "Order is completed")]
    Completed = 7,
    
    [Display(Name = "Unavailable")]
    Unavailable2 = 8
}