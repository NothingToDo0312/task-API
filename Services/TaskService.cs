using WebApplication1.Models;
using WebApplication1.Repositories;
using System;
using System.Collections.Generic;

namespace WebApplication1.Services
{
    public class TaskService
    {
        private readonly GenericRepository<TaskModel> _taskRepository;

        public TaskService()
        {
            _taskRepository = new GenericRepository<TaskModel>();
        }

        // Add new task
        public TaskModel AddTask(TaskModel task)
        {
            // Force IsCompleted to false on new task
            task.IsCompleted = false;

            // Set timestamps
            task.DateCreated = DateTime.Now;
            task.DateUpdated = DateTime.Now;

            var addedTask = _taskRepository.Add(task);
            return addedTask!;
        }

        // Get all tasks
        public IEnumerable<TaskModel> GetAllTasks()
        {
            return _taskRepository.GetAll();
        }

        // Get task by Id
        public TaskModel? GetTaskById(int id)
        {
            return _taskRepository.GetById(id);
        }

        // Update task
        public bool UpdateTask(TaskModel task)
        {
            // Update timestamp
            task.DateUpdated = DateTime.Now;
            return _taskRepository.Update(task);
        }

        // Delete task
        public bool DeleteTask(TaskModel task)
        {
            return _taskRepository.Delete(task);
        }
    }
}
