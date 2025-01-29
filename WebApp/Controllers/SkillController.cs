using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TM.BL.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Authorize]
    public class SkillController : Controller
    {
        private readonly TaskMgmtContext _context;
        private readonly IMapper _mapper;

        public SkillController(TaskMgmtContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        // GET: SkilController1
        public ActionResult Index()
        {
            var skills = _context.Skills.ToList();
            var skillViewModels = _mapper.Map<List<SkillVM>>(skills);
            return View(skillViewModels);
        }

        // GET: SkilController1/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SkilController1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SkillController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SkillVM skillVm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Failed to create a Skill");
                    return View(skillVm);
                }

                var newSkill = new Skill
                {
                    Name = skillVm.Name
                };

                _context.Skills.Add(newSkill);

                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    var innerException = ex.InnerException?.Message ?? ex.Message;
                    ModelState.AddModelError("", $"Error saving skill: {innerException}");
                    return View(skillVm);
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(skillVm);
            }
        }

        // GET: SkillController/Edit/5
        public ActionResult Edit(int id)
        {
            var skill = _context.Skills.Find(id);
            if (skill == null)
            {
                return NotFound();
            }

            var skillVm = new SkillVM
            {
                Id = skill.Id,
                Name = skill.Name,
                TaskSkillIds = skill.TaskSkills.Select(ts => ts.Id).ToList(),
                UserSkillIds = skill.UserSkills.Select(us => us.Id).ToList()
            };

            return View(skillVm);
        }

        // POST: SkillController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, SkillVM skillVm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Failed to update the Skill");
                    return View(skillVm);
                }

                var existingSkill = _context.Skills.Find(id);
                if (existingSkill == null)
                {
                    return NotFound();
                }

                existingSkill.Name = skillVm.Name;

                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating skill: {ex.Message}");
                    return View(skillVm);
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(skillVm);
            }
        }

        // GET: SkillController/Delete/5
        public ActionResult Delete(int id)
        {
            var skill = _context.Skills.Find(id);
            if (skill == null)
            {
                return NotFound();
            }

            var skillVm = new SkillVM
            {
                Id = skill.Id,
                Name = skill.Name
            };

            return View(skillVm);
        }

        // POST: SkillController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var dbSkil = _context.Skills.FirstOrDefault(x => x.Id == id);

                if (dbSkil == null)
                {
                    return NotFound();
                }

                _context.Skills.Remove(dbSkil);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError("", $"Error deleting task: {innerException}");
                return View();
            }
        }
    }
}
