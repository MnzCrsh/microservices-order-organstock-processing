using System.ComponentModel.DataAnnotations;

namespace OrderService.Entities;

public enum OrderStatus
{
    [Display(Name = "Unavailable")]
    NoneStatus1 = 0,
    
    [Display(Name = "Order is created")]
    Created = 10,
    
    [Display(Name = "Order is processing")]
    Processing = 20,
    
    [Display(Name = "Processing payment")]
    PaymentPending = 30,
    
    [Display(Name = "Payment rejected")]
    PaymentRejected = 40,
    
    [Display(Name = "Payment refunded")]
    PaymentRefunded = 50,
    
    [Display(Name = "Order confirmed")]
    Confirmed = 60,
    
    [Display(Name = "Order is completed")]
    Completed = 70,
    
    [Display(Name = "Unavailable")]
    NoneStatus2 = 80
}