using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TM.BL.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Authorize]
    public class TaskAssignmentController : Controller
    {
        private readonly TaskMgmtContext _context;
        private readonly IMapper _mapper;
        public TaskAssignmentController(TaskMgmtContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: TaskAssignmentController
        public IActionResult Index()
        {
            var taskAssignments = _context.TaskAssignments
                .Include(t => t.Task)
                .Include(u => u.User)
                .ToList();

            var viewModel = _mapper.Map<List<TaskAssignmentVM>>(taskAssignments);

            var acceptedTasks = _context.TaskAssignments
                .Where(t => t.Status == "Accepted")
                .Select(t => t.TaskId)
                .Distinct()
                .ToHashSet();  

            var deniedTasks = _context.TaskAssignments
                .Where(t => t.Status == "Denied")
                .Select(t => t.TaskId)
                .Distinct()
                .ToHashSet();

            ViewBag.AcceptedTasks = acceptedTasks;
            ViewBag.DeniedTasks = deniedTasks;

            return View(viewModel);
        }


        // GET: TaskAssignmentController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TaskAssignmentController/Create
        public IActionResult Create(int taskId)
        {
            var username = User.Identity.Name;

            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("User not found in cookies.");
            }

            // Get the corresponding UserId from the database
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return BadRequest("User not found in database.");
            }
            

            // Create a new TaskAssignment
            var taskAssignment = new TaskAssignment
            {
                TaskId = taskId,
                UserId = user.Id,
                Status = "Pending"
            };

            _context.TaskAssignments.Add(taskAssignment);
            _context.SaveChanges();

            // Redirect back to the Task Index page
            return RedirectToAction("Index", "Task");
        }

        public IActionResult Accept(int id)
        {
            var taskAssignment = _context.TaskAssignments.FirstOrDefault(t => t.Id == id);

            if (taskAssignment == null)
            {
                return NotFound();
            }

            taskAssignment.Status = "Accepted";
            _context.SaveChanges();

            return RedirectToAction("Index"); // Refresh task assignments list
        }

        // Deny Task Assignment
        public IActionResult Deny(int id)
        {
            var taskAssignment = _context.TaskAssignments.FirstOrDefault(t => t.Id == id);

            if (taskAssignment == null)
            {
                return NotFound();
            }

            taskAssignment.Status = "Denied";
            _context.SaveChanges();

            return RedirectToAction("Index"); // Refresh task assignments list
        }


    }
}
