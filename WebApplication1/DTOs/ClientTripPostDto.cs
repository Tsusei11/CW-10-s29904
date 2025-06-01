using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs;

public class ClientTripPostDto
{
    [Required]
    [MaxLength(120)]
    public string FirstName { get; set; }
    
    [Required]
    [MaxLength(120)]
    public string LastName { get; set; }
    
    [Required]
    [MaxLength(120)]
    [RegularExpression("^[a-zA-Z0-9]+@[a-z]+.[a-z]+$")]
    public string Email { get; set; }
    
    [Required]
    [MaxLength(120)]
    [RegularExpression("^[0-9]+-[0-9]+-[0-9]+$")]
    public string Telephone { get; set; }
    
    [Required]
    [MaxLength(120)]
    public string Pesel { get; set; }
    
    public DateTime? PaymentDate { get; set; }
}