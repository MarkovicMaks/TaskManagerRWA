using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTOs;
using WebAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly TaskMgmtContext _context;

        public TaskController(IConfiguration configuration, TaskMgmtContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        // GET: api/<TaskController>
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

                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // GET api/<TaskController>/5
        
        [HttpGet("{id}")]
        public ActionResult<TaskDto> Get(int id)
        {
            try
            {
                var result =
                    _context.Tasks
                        .FirstOrDefault(x => x.Id == id);

                var mappedResult = new TaskDto
                {
                    Id = result.Id,
                    ManagerId = result.ManagerId,
                    Title = result.Title,
                    Description = result.Description,
                    Status = result.Status,
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<TaskController>
        [HttpPost]
        public ActionResult<TaskDto> Post([FromBody] TaskDto value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newTask = new Models.Task
            {
                
                ManagerId = value.ManagerId,
                Title = value.Title,
                Description = value.Description,
                Status = value.Status,
            };

            _context.Tasks.Add(newTask);

            _context.SaveChanges();

            value.Id = newTask.Id;

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
                    return BadRequest("Page and count parameters must be greater than 0.");
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

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // PUT api/<TaskController>/5
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
                    return NotFound($"Task with ID {id} not found.");
                }

                // Update the task's properties
                existingTask.ManagerId = value.ManagerId;
                existingTask.Title = value.Title;
                existingTask.Description = value.Description;
                existingTask.Status = value.Status;

                _context.Tasks.Update(existingTask);
                _context.SaveChanges();

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
                return StatusCode(500, ex.Message);
            }
        }
        // DELETE api/<TaskController>/5
        [HttpDelete("{id}")]

        public IActionResult Delete(int id)
        {
            try
            {
                var taskToDelete = _context.Tasks.FirstOrDefault(x => x.Id == id);

                if (taskToDelete == null)
                {
                    return NotFound($"Task with ID {id} not found.");
                }

                _context.Tasks.Remove(taskToDelete);
                _context.SaveChanges();

                return Ok($"Task with ID {id} has been successfully deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
