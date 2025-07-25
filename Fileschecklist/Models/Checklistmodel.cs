using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fileschecklist.Models
{
    public class Checklistmodel
    {
        public class Campus
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int CampusID { get; set; }

            public string? CampusName { get; set; }
        }
        
    }
    public class Studentfiles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? StudentID { get; set; }

        public string? Studentname { get; set; }

        public string? Gradelevel { get; set; }

        public string? Remarks { get; set; }

        public string? Status { get; set; }

        public string? Section { get; set; }

        public string? Lrn { get; set; }

        public string? Formerschool { get; set; }

        public string? Location { get; set; }

        public string? PSA { get; set; }

        public string? Card { get; set; }

        public string? Enrollmentform { get; set; }

        public string? Passportpicture { get; set; }

        public string? Academichonors { get; set; }

        public string? Indegency { get; set; }

        public string? Form137 { get; set; }

        public string? SchoolyearName { get; set; }

        public string? CampusName { get; set; }
    }
    public class User
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Role { get; set; }
        public string? Profilepic { get; set; }
        public string? CampusName { get; set; }


    }

    public class Schoolyear
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SchoolyearID { get; set; }

        public string? SchoolyearName { get; set; }

        public string? CampusName { get; set; }
    }


}
