using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TM.BL.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
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

            return View(viewModel);
        }
        
        // GET: TaskAssignmentController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TaskAssignmentController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TaskAssignmentController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TaskAssignmentController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TaskAssignmentController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TaskAssignmentController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TaskAssignmentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
