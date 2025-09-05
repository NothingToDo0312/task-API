using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;
using System.Collections.Generic;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly TaskService _service;

        public TaskController()
        {
            _service = new TaskService();
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_service.GetAllTasks());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var task = _service.GetTaskById(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        [HttpPost]
        public IActionResult Add([FromBody] TaskModel task)
        {
            var added = _service.AddTask(task);
            return Ok(added);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] TaskModel task)
        {
            task.Id = id;
            if (!_service.UpdateTask(task)) return NotFound();
            return Ok(task);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var task = _service.GetTaskById(id);
            if (task == null) return NotFound();
            if (!_service.DeleteTask(task)) return StatusCode(500);
            return Ok(task);
        }
    }
}
