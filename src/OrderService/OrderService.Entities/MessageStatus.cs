using System.ComponentModel.DataAnnotations;

namespace OrderService.Entities;

public enum MessageStatus
{
    [Display(Name = "Unavailable")]
    Unknown1 = 0,
    
    [Display(Name = "Message pending")]
    Pending = 1,
    
    [Display(Name = "Message sent")]
    Processed = 2,
    
    [Display(Name = "Unavailable")]
    Unknown2 = 3
}