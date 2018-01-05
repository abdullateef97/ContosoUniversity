using System;
using System.Linq;
using System.Threading.Tasks;
using ContosoUni.Data;
using ContosoUni.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace ContosoUni.Controllers
{
    public class StudentController : Controller
    {
        private readonly SchoolContext _context;

        public StudentController(SchoolContext context)
        {
            _context = context;
        }
        // GET
//        public IActionResult Index()
//        {
//            return View();
//        }
        public async Task<IActionResult> Index(string sortOrder, string SearchString)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["currentFilter"] = SearchString;

            var students = from s in _context.Students
                select s;
            if (!String.IsNullOrEmpty(SearchString))
            {
                students = students.Where(s => s.LastName.Contains(SearchString) || s.FirstName.Contains(SearchString));
            }
           
            

            switch (sortOrder)
            {
               case "name_desc":
                   students = students.OrderByDescending(s => s.LastName);
                   break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
               case "date_desc":
                   students = students.OrderByDescending(s => s.EnrollmentDate);
                   break;
               default:
                   students = students.OrderBy(s => s.LastName);
                   break;
            }
                    
            
            return View(await students.AsNoTracking().ToListAsync());
        }
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.course)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind( "FirstName, LastName, EnrollmentDate " )] Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("","couldnt add student pls try again");
            }
            return View(student);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(c => c.course)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);
           

            return View(student);

        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            var studentToUpdate = await _context.Students.SingleOrDefaultAsync(s => s.ID == id);
            if (await TryUpdateModelAsync<Student>(studentToUpdate, "",
                s => s.FirstName, s => s.LastName, s => s.EnrollmentDate))
            {
                try
                {
                  await  _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                                                 "Try again, and if the problem persists, " +
                                                 "see your system administrator.");
                }
              
            }
            return View(studentToUpdate);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentToDelete = await _context.Students
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);

            if (studentToDelete == null)
            {
                RedirectToAction(nameof(Index));
            }
            try
            {
                _context.Students.Remove(studentToDelete);
                await _context.SaveChangesAsync();
               return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("","Couldnt delete, please contact ur network provider or try again");
                
            }
            return View(studentToDelete);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(c => c.course)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.ID == id);

            return View(student);
        }
   }
}