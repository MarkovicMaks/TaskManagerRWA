using Humanizer.Localisation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class TaskController : Controller
    {
        private readonly TaskMgmtContext _context;

        public TaskController(TaskMgmtContext context)
        {
            _context = context;
        }
        // GET: TaskController
        public ActionResult Index()
        {
            try
            {
                var taskVms = _context.Tasks
                    .Include(x => x.Manager)
                    .Select(x => new TaskVm
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    ManagerId = x.ManagerId,
                    Status = x.Status,
                }).ToList();

                return View(taskVms);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: TaskController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TaskController/Create
        public ActionResult Create()
        {
            ViewBag.ManagerDdlItems = _context.Managers
                .Join(
                    _context.Users, 
                    manager => manager.UserId, 
                    user => user.Id,          
                    (manager, user) => new SelectListItem
                    {
                        Value = manager.Id.ToString(),
                        Text = user.Username
                    }
                )
                .ToList(); 

            return View();
        }

        // POST: TaskController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaskVm task)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ManagerDdlItems = _context.Managers
                    .Join(
                        _context.Users,
                        manager => manager.UserId,
                        user => user.Id,
                        (manager, user) => new SelectListItem
                        {
                            Value = manager.Id.ToString(),
                            Text = user.Username
                        }
                    )
                    .ToList();
                    ModelState.AddModelError("", "Failed to create a Task");
                    return View();
                }
                var newTask = new Models.Task
                {
                    Title = task.Title,
                    ManagerId = task.ManagerId,
                    Description = task.Description,
                    Status = task.Status,
                };
                _context.Tasks.Add(newTask);

                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    var innerException = ex.InnerException?.Message ?? ex.Message;
                    ModelState.AddModelError("", $"Error saving task: {innerException}");
                    return View(task);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TaskController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TaskController/Edit/5
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

        // GET: TaskController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TaskController/Delete/5
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
