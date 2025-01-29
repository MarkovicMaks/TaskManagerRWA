using AutoMapper;
using AutoMapper.QueryableExtensions;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using TM.BL.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly TaskMgmtContext _context;
        private readonly IMapper _mapper;

        public TaskController(TaskMgmtContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        // GET: TaskController
        public ActionResult Index()
        {
            try
            {
                var taskVms = _context.Tasks.Include(x => x.Manager) 
                 .ProjectTo<TaskVm>(_mapper.ConfigurationProvider) 
                 .ToList();

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
            var task = _context.Tasks
                .Where(x => x.Id == id) // Filter only the task we need
                .Include(x => x.Manager)
                .Include(x => x.TaskSkills)
                .ThenInclude(ts => ts.Skill)
                .ProjectTo<TaskVm>(_mapper.ConfigurationProvider) // Use AutoMapper projection
                .FirstOrDefault();

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
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

            ViewBag.SkillDdlItems = _context.Skills
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList();

            return View();
        }

        // POST: TaskController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaskVm taskVm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ManagerDdlItems = _context.Managers
                        .Join(_context.Users,
                            manager => manager.UserId,
                            user => user.Id,
                            (manager, user) => new SelectListItem
                            {
                                Value = manager.Id.ToString(),
                                Text = user.Username
                            }
                        ).ToList();

                    taskVm.SkillOptions = _context.Skills
                        .Select(s => new SelectListItem
                        {
                            Value = s.Id.ToString(),
                            Text = s.Name
                        }).ToList();

                    ModelState.AddModelError("", "Failed to create a Task.");
                    return View(taskVm);
                }

                var newTask = new TM.BL.Models.Task
                {
                    Title = taskVm.Title,
                    ManagerId = taskVm.ManagerId,
                    Description = taskVm.Description,
                    Status = taskVm.Status
                };

                _context.Tasks.Add(newTask);
                _context.SaveChanges();

                if (taskVm.SelectedSkillIds?.Any() == true)
                {
                    foreach (var skillId in taskVm.SelectedSkillIds)
                    {
                        _context.TaskSkills.Add(new TaskSkill
                        {
                            TaskId = newTask.Id,
                            SkillId = skillId
                        });
                    }
                    _context.SaveChanges();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("", $"Error saving task: {innerException}");
                return View(taskVm);
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

            ViewBag.SkillDdlItems = _context.Skills
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                })
                .ToList();

            var task = _context.Tasks
                .Include(x => x.Manager)
                .Include(x => x.TaskSkills) 
                .ThenInclude(ts => ts.Skill)
                .FirstOrDefault(x => x.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            var taskVm = _mapper.Map<TaskVm>(task);
            taskVm.SelectedSkillIds = task.TaskSkills.Select(ts => ts.SkillId).ToList();

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
                    // Repopulate dropdowns in case of errors
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

                    ViewBag.SkillDdlItems = _context.Skills
                        .Select(s => new SelectListItem
                        {
                            Value = s.Id.ToString(),
                            Text = s.Name
                        })
                        .ToList();

                    ModelState.AddModelError("", "Please correct the errors and try again.");
                    return View(task);
                }

                // Retrieve the existing task from the database
                var dbTask = _context.Tasks
                    .Include(x => x.TaskSkills) // Include the skills
                    .FirstOrDefault(x => x.Id == id);

                if (dbTask == null)
                {
                    return NotFound();
                }

                // Update task properties
                dbTask.Title = task.Title;
                dbTask.Description = task.Description;
                dbTask.ManagerId = task.ManagerId;
                dbTask.Status = task.Status;

                // Update TaskSkills (Remove old skills and add new ones)
                _context.TaskSkills.RemoveRange(dbTask.TaskSkills); // Remove existing skills

                if (task.SelectedSkillIds != null && task.SelectedSkillIds.Any())
                {
                    foreach (var skillId in task.SelectedSkillIds)
                    {
                        _context.TaskSkills.Add(new TaskSkill
                        {
                            TaskId = dbTask.Id,
                            SkillId = skillId
                        });
                    }
                }

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
                .ThenInclude(m => m.User)
                .FirstOrDefault(x => x.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            var taskVm = _mapper.Map<TaskVm>(task);

            return View(taskVm);
        }


        // POST: TaskController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, TM.BL.Models.Task task)
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
