using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace oop_backend.Models
{
    public class Attendance
    {
        //	attendanceID INT PRIMARY KEY,
        //       userID INT,
        //       eventDate DATE NOT NULL,
        //      eventTime TIME NOT NULL,
        //   status VARCHAR(20) NOT NULL,

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? attendanceID { get; set; }
        public required int userID { get; set; }
        public required string status { get; set; }
        [DataType(DataType.Date)]
        public required string eventDate { get; set; }
        [DataType(DataType.Time)]
        public required string eventTime { get; set; }
    }
}
