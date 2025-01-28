using Humanizer.Localisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Authorize]
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
            // Populate Manager dropdown
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

            // Find the task by ID and map to the ViewModel
            var task = _context.Tasks.Include(x => x.Manager).FirstOrDefault(x => x.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            var taskVm = new TaskVm
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                ManagerId = task.ManagerId,
                Status = task.Status
            };

            return View(taskVm);
        }

        // POST: TaskController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TaskVm task)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Repopulate dropdown in case of errors
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
                    ModelState.AddModelError("", "Please correct the errors and try again.");
                    return View(task);
                }

                // Retrieve the existing task from the database
                var dbTask = _context.Tasks.FirstOrDefault(x => x.Id == id);
                if (dbTask == null)
                {
                    return NotFound();
                }

                // Update task properties
                dbTask.Title = task.Title;
                dbTask.Description = task.Description;
                dbTask.ManagerId = task.ManagerId;
                dbTask.Status = task.Status;

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("", $"Error updating task: {innerException}");
                return View(task);
            }
        }


        // GET: TaskController/Delete/5
        public ActionResult Delete(int id)
        {
            var task = _context.Tasks
                .Include(x => x.Manager)
                .ThenInclude(m => m.User) // Assuming Manager is linked to a User
                .FirstOrDefault(x => x.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            var taskVm = new TaskVm
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                ManagerId = task.ManagerId,
                ManagerName = task.Manager.User.Username, // Assuming Manager has a User with Username
                Status = task.Status
            };

            return View(taskVm);
        }


        // POST: TaskController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Models.Task task)
        {
            try
            {
                var dbTask = _context.Tasks.FirstOrDefault(x => x.Id == id);

                if (dbTask == null)
                {
                    return NotFound();
                }

                _context.Tasks.Remove(dbTask);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("", $"Error deleting task: {innerException}");
                return View(task);
            }
        }

    }
}
