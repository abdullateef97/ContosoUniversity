using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using ContosoUni.Data;
using ContosoUni.Models;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;


namespace ContosoUni.Controllers
{
    public class CourseController : Controller
    {
        private readonly SchoolContext _context;

        public CourseController(SchoolContext context)
        {
            _context = context;
        }
        
        // GET
//        public async Task<IActionResult> Index()
//        {
//         var courses = await _context.Courses
//                .Include(c => c.Department).AsNoTracking()
//                .ToListAsync();
//            
//            return View(courses); 
//        }
        public  IActionResult Index()
        {
            //

            var courses = (from m in _context.Courses
                join x in _context.Departments on m.DepartmentID equals x.DepartmentID
                select new DepartmentalCourse
                {
                    Course = m,
                    Department = x
                });
        //  return Content(JsonConvert.SerializeObject(courses));
         return View(courses);
        }

     
        public IActionResult Create()
        {
            PopulateDepartmentsDropDownList();
            return View();
        }
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseID,Credits,DepartmentID,Title")] Course course)
        {
            if (ModelState.IsValid)
            {
                
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

//            var course = await QueryableExtensions.AsNoTracking(_context.Courses)
//                .SingleOrDefaultAsync<Course>(m => m.CourseID == id);

            var course = _context.Courses.AsNoTracking().SingleOrDefault(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }
            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }
        
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseToUpdate = await _context.Courses
                .SingleOrDefaultAsync(c => c.CourseID == id);

            if (await TryUpdateModelAsync<Course>(courseToUpdate,
                "",
                c => c.Credits, c => c.DepartmentID, c => c.Title))
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                                                 "Try again, and if the problem persists, " +
                                                 "see your system administrator.");
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateDepartmentsDropDownList(courseToUpdate.DepartmentID);
            return View(courseToUpdate);
        }
        
        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = from d in _context.Departments
                orderby d.Name
                select d;
            ViewBag.DepartmentID = new SelectList(QueryableExtensions.AsNoTracking(departmentsQuery), "DepartmentID", "Name", selectedDepartment);
        } 
        
        public IActionResult Delete(int? id)
        {

//            ViewBag.course  = (from m in _context.Courses
//                join x in _context.Departments on m.DepartmentID equals x.DepartmentID
//                where  m.CourseID == id
//                select new
//                {
//                      m.CourseID,
//                    m.Title,
//                     m.Credits,
//                    m.DepartmentID,
//                    Department = x.Name
//                }).SingleOrDefault();

            var DepartmentalCourse = new DepartmentalCourse
            {
                Course = _context.Courses.AsNoTracking().SingleOrDefault(x => x.CourseID == id),
                
            };
            DepartmentalCourse.DepartmentID = DepartmentalCourse.Course.DepartmentID;
            DepartmentalCourse.Department =
                _context.Departments.AsNoTracking().SingleOrDefault(d => d.DepartmentID == DepartmentalCourse.DepartmentID);

            //return Content(JsonConvert.SerializeObject(DepartmentalCourse));
            return View(DepartmentalCourse);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public IActionResult DeleteConfirmed(int? id)
        {
            var studentToDelete =  _context.Courses
                .AsNoTracking()
                .SingleOrDefault(m => m.CourseID == id);

            if (studentToDelete == null)
            {
                RedirectToAction(nameof(Index));
            }
            try
            {
                _context.Courses.Remove(studentToDelete);
                _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("","Couldnt delete, please contact ur network provider or try again");
                
            }
            return View();
        }
    }
}