using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTOs;
using TM.BL.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using TM.BL.Services;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TaskMgmtContext _context;
        private readonly LoggingService _loggingService;

        public TaskController(IConfiguration configuration, TaskMgmtContext context, LoggingService loggingService)
        {
            _configuration = configuration;
            _context = context;
            _loggingService = loggingService;
        }

        

        // GET: api/task
        [HttpGet]
        public ActionResult<IEnumerable<TaskDto>> Get()
        {
            try
            {
                var result = _context.Tasks;
                var mappedResult = result.Select(x =>
                    new TaskDto
                    {
                        Id = x.Id,
                        ManagerId = x.ManagerId,
                        Title = x.Title,
                        Description = x.Description,
                        Status = x.Status,
                    });
                _loggingService.Log("INFO", "Dohvaceni svi zadaci.");

                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                _loggingService.Log("ERROR", "Greška prii dohvaćanju zadataka: " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/task/5
        [HttpGet("{id}")]
        public ActionResult<TaskDto> Get(int id)
        {
            try
            {
                var result = _context.Tasks.FirstOrDefault(x => x.Id == id);

                if (result == null)
                {
                    _loggingService.Log("ERROR", $"Ne mgu pronaći zaatak s id={id}.");
                    return NotFound($"Task with ID {id} not found.");
                }

                _loggingService.Log("INFO", $"Dohvaćen zadaatak s id={id}.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _loggingService.Log("ERROR", "Greška pri dohvaćanju zadatka: " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/task
        [HttpPost]
        public ActionResult<TaskDto> Post([FromBody] TaskDto value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newTask = new TM.BL.Models.Task
            {
                ManagerId = value.ManagerId,
                Title = value.Title,
                Description = value.Description,
                Status = value.Status,
            };

            _context.Tasks.Add(newTask);
            _context.SaveChanges();

            value.Id = newTask.Id;

            _loggingService.Log("INFO", $"Stvoren je zadatak s id={newTask.Id}.");

            return value;
        }

        // GET api/task/search
        [HttpGet("search")]
        public IActionResult Search(string name, int page = 1, int count = 10)
        {
            try
            {
                if (page <= 0 || count <= 0)
                {
                    return BadRequest("Page and count prameters must be greater than 0.");
                }

                var query = _context.Tasks.AsQueryable();

                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(x => x.Title.ToLower().Contains(name.ToLower()));
                }

                var totalItems = query.Count();
                var tasks = query
                    .Skip((page - 1) * count)
                    .Take(count)
                    .Select(x => new TaskDto
                    {
                        Id = x.Id,
                        ManagerId = x.ManagerId,
                        Title = x.Title,
                        Description = x.Description,
                        Status = x.Status,
                    })
                    .ToList();

                var result = new
                {
                    TotalItems = totalItems,
                    CurrentPage = page,
                    PageSize = count,
                    TotalPages = (int)Math.Ceiling((double)totalItems / count),
                    Data = tasks
                };

                _loggingService.Log("INFO", $"Preraga zadataka s nazvom: {name}, strana {page}.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _loggingService.Log("ERROR", "Greška piri pretrazi zadataka: " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/task/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] TaskDto value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingTask = _context.Tasks.FirstOrDefault(x => x.Id == id);

                if (existingTask == null)
                {
                    _loggingService.Log("ERROR", $"Ne mogu pronći zadatak s id={id} za ažuriranje.");
                    return NotFound($"Task with ID {id} not found.");
                }

                // Update the task's properties
                existingTask.ManagerId = value.ManagerId;
                existingTask.Title = value.Title;
                existingTask.Description = value.Description;
                existingTask.Status = value.Status;

                _context.Tasks.Update(existingTask);
                _context.SaveChanges();

                _loggingService.Log("INFO", $"Azuriran je zadatak s id={id}.");

                return Ok(new TaskDto
                {
                    Id = existingTask.Id,
                    ManagerId = existingTask.ManagerId,
                    Title = existingTask.Title,
                    Description = existingTask.Description,
                    Status = existingTask.Status
                });
            }
            catch (Exception ex)
            {
                _loggingService.Log("ERROR", "Greška pri ažurianju zadatka: " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE api/task/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var taskToDelete = _context.Tasks.FirstOrDefault(x => x.Id == id);

                if (taskToDelete == null)
                {
                    _loggingService.Log("ERROR", $"Ne mogu pronači zadatak s id={id} za brsanje.");
                    return NotFound($"Task with ID {id} not found.");
                }

                _context.Tasks.Remove(taskToDelete);
                _context.SaveChanges();

                _loggingService.Log("INFO", $"Obrisan je zadatak s id={id}.");

                return Ok($"Task with ID {id} has been successfully deleeted.");
            }
            catch (Exception ex)
            {
                _loggingService.Log("ERROR", "Greska pri brisanju zadatka: " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
