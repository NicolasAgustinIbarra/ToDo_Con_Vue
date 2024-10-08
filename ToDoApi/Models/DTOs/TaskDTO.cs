using System.ComponentModel.DataAnnotations;

namespace ToDoApi.Models.DTOs
{
    public class TaskDTO
    {
       
        public int taskId { get; set; }
        [Required]
        public string taskName { get; set; }
        public string status { get; set; }
        public string? expirationDate { get; set; }
        public string createdDay{ get; set; }
    }
}
