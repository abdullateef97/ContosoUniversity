using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ContosoUni.Data;
using ContosoUni.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;

namespace ContosoUni.Controllers
{
    public class DepartmentController : Controller
    {
        // GET
       private readonly SchoolContext _context;

        public DepartmentController(SchoolContext context)
        {
            _context = context;
            
            
           
        }
        public  IActionResult Index()
        {

            List<Department> departmentList = new List<Department>();

            var departments = _context.Departments.AsNoTracking().ToList();
            foreach (var department in departments)
            {
                var course = _context.Courses.Where(c => c.DepartmentID == department.DepartmentID).ToList();
                department.Courses = course;
                departmentList.Add(department);
            }

            
            
            return View(departmentList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CreateNew([Bind("Name","Budget","StartDate")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            var department = _context.Departments.AsNoTracking().SingleOrDefault(d => d.DepartmentID == id);
            return View(department);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> EditDepartment(int? id)
        {
            var departmentToEdit = _context.Departments.AsNoTracking().SingleOrDefault((d => d.DepartmentID == id));

            if (await TryUpdateModelAsync<Department>(departmentToEdit, "",
                d => d.Name, d => d.Budget, d => d.StartDate))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("","Unable to update, pls try again");
                    
                }
            }
            return RedirectToAction(nameof(Index));
        }

//        public IActionResult Details(int? id)
//        {
//            var departmentName = _context.Departments.AsNoTracking().SingleOrDefault(d => d.DepartmentID == id).Name;
//            var selectCourses = _context.Courses.AsNoTracking().Where(c => c.DepartmentID == id).ToList();
//            
//            var departmentalCourse = new DepartmentalCourse();
//            departmentalCourse.DepartmentName = departmentName;
//            departmentalCourse.Courses = selectCourses;
//            return View(departmentalCourse);
//        }
        
        
   [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Delete(int? id)
        {
            var departmentToDelete = _context.Departments.AsNoTracking().SingleOrDefault(d => d.DepartmentID == id);

            if (departmentToDelete == null)
            {
               return  RedirectToAction(nameof(Index));
            }
            try
            {
                _context.Departments.Remove(departmentToDelete);
                await _context.SaveChangesAsync();
              return   RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("","Coudn't delete please try again");
              return  RedirectToAction(nameof(Index));
            }

          return  RedirectToAction(nameof(Index));
        }
    }
}