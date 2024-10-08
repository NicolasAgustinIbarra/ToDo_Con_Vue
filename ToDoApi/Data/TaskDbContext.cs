using Microsoft.EntityFrameworkCore;
using ToDoApi.Models;

namespace ToDoApi.Data
{
    public class TaskDbContext:DbContext
    {
        public DbSet<TaskModel> Tasks { get; set; }

        public TaskDbContext(DbContextOptions<TaskDbContext> options): base(options) { }
    }
}
