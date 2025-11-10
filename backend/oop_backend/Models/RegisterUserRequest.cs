using System.ComponentModel.DataAnnotations;

namespace oop_backend.Models;

public class RegisterUserRequest
{
    [Required]
    public required string Photo { get; set; }
    
    [Required]
    public required string FirstName { get; set; }
    
    [Required]
    public required string LastName { get; set; }
    
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    public required string BirthDate { get; set; }
}

