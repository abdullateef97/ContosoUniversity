using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoUni.Models;

namespace ContosoUni.ViewModels
{
    public class InstrutorIndexData
    {
        public IEnumerable<Instructor> Instructors { get; set; }
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; }
    }
}