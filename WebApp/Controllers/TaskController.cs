using AutoMapper;
using AutoMapper.QueryableExtensions;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Configuration;
using System.Threading.Tasks;
using TM.BL.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly TaskMgmtContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public TaskController(TaskMgmtContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        //GET: TaskController
        public ActionResult PersonalTasks()
        {
            try
            {
                var username = User.Identity.Name;
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user == null)
                {
                    return BadRequest("User not found in the database.");
                }

                var taskVms = _context.TaskAssignments
                    .Where(ta => ta.UserId == user.Id && ta.Status == "accepted")
                    .Select(ta => ta.Task)
                    .ProjectTo<TaskVM>(_mapper.ConfigurationProvider)
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
                .ProjectTo<TaskVM>(_mapper.ConfigurationProvider) // Use AutoMapper projection
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
        public ActionResult Create(TaskVM taskVm)
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

            var taskVm = _mapper.Map<TaskVM>(task);
            taskVm.SelectedSkillIds = task.TaskSkills.Select(ts => ts.SkillId).ToList();

            return View(taskVm);
        }


        // POST: TaskController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TaskVM task)
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

            var taskVm = _mapper.Map<TaskVM>(task);

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


        public ActionResult Search(SearchVM searchVm)
        {
            try
            {
                if (string.IsNullOrEmpty(searchVm.Q) && string.IsNullOrEmpty(searchVm.Submit))
                {
                    searchVm.Q = Request.Cookies["query"];
                }

                IQueryable<TM.BL.Models.Task> tasks = _context.Tasks
                    .Include(x => x.Manager);

                if (!string.IsNullOrEmpty(searchVm.Q))
                {
                    tasks = tasks.Where(x => x.Title.Contains(searchVm.Q) || x.Description.Contains(searchVm.Q));
                }

                // We need this for pager
                var filteredCount = tasks.Count();

                switch (searchVm.OrderBy.ToLower())
                {
                    case "id":
                        tasks = tasks.OrderBy(x => x.Id);
                        break;
                    case "title":
                        tasks = tasks.OrderBy(x => x.Title);
                        break;
                    case "status":
                        tasks = tasks.OrderBy(x => x.Status);
                        break;
                    case "manager":
                        tasks = tasks.OrderBy(x => x.Manager.User.Username);
                        break;
                }

                tasks = tasks.Skip((searchVm.Page - 1) * searchVm.Size).Take(searchVm.Size);
                searchVm.Tasks =
                    tasks.Select(x => new TaskVM
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Description = x.Description,
                        ManagerId = x.ManagerId,
                        ManagerName = x.Manager.User.Username,
                        Status = x.Status
                    })
                    .ToList();

                var assignedTaskIds = _context.TaskAssignments
                    .Select(t => t.TaskId)
                    .Distinct()
                    .ToList();
                ViewBag.AssignedTaskIds = assignedTaskIds;

                // BEGIN PAGER
                var expandPages = _configuration.GetValue<int>("Paging:ExpandPages");
                searchVm.LastPage = (int)Math.Ceiling(1.0 * filteredCount / searchVm.Size);
                searchVm.FromPager = searchVm.Page > expandPages ?
                    searchVm.Page - expandPages :
                    1;
                searchVm.ToPager = (searchVm.Page + expandPages) < searchVm.LastPage ?
                    searchVm.Page + expandPages :
                    searchVm.LastPage;
                // END PAGER

                var option = new CookieOptions { Expires = DateTime.Now.AddMinutes(15) };
                Response.Cookies.Append("query", searchVm.Q ?? "", option);

                return View("Index", searchVm); // Ensure it returns the Index view with search results
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
