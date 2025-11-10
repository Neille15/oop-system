using System.ComponentModel.DataAnnotations;

namespace oop_backend.Models;

public class AttendanceRequest
{
    [Required]
    public required string Photo { get; set; }
    
    public string? Status { get; set; }
}

