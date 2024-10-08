using System.ComponentModel.DataAnnotations;

namespace ToDoApi.Models.DTOs
{
    public class CreateTaskDTO
    {

        public int taskId { get; set; }
        [Required]
        public string taskName { get; set; }
        public string status { get; set; }
        public DateTime? expirationDate { get; set; }
        public string createdDay { get; set; }
    }
}
