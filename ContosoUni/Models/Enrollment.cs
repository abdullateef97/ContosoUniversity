using System.ComponentModel.DataAnnotations;

namespace ContosoUni.Models
{
    public enum Grade
    {
        A,B,C,D,E,F
    }
    
    public class Enrollment
    {
        public int EnrollmentId { get; set; }
        public int CourseID { get; set; }
        public int StudentID { get; set; }
        [DisplayFormat(NullDisplayText = "No grade")]
        public Grade? Grade { get; set; }
        
        public Student student { get; set; }
        public Course course { get; set; }
    }
}