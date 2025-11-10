using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace oop_backend.Models;

public class UserData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? userID{ get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    [DataType(DataType.Date)]
    public required string BirthDate { get; set; }

}
