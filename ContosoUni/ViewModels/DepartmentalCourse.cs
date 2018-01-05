using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContosoUni.Models
{
    public class DepartmentalCourse
    {
        
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public Department Department { get; set; }
       // public List<Course> Courses { get; set; }
        public Course Course;
        

    }
}