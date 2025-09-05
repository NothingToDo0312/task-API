using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    [Table("TaskTbl")] // Table name in SQL Server
    public class TaskModel
    {
        [Key]
        [Column("Id")] // Column name in DB
        public int Id { get; set; }

        [Column("Name")]
        public string? Name { get; set; } 

        [Column("Category")]
        public string? Category { get; set; } 

        [Column("Priority")]
        public string? Priority { get; set; }

        [Column("DateCreated")]
        public DateTime DateCreated { get; set; }

        [Column("DateUpdated")]
        public DateTime DateUpdated { get; set; }

        [Column("Deadline")]
        public DateTime Deadline { get; set; }

        [Column("IsCompleted")]
        public bool IsCompleted { get; set; }
    }
}
