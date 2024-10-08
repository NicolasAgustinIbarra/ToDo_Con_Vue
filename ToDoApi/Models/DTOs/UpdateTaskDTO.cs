using System.ComponentModel.DataAnnotations;

namespace ToDoApi.Models.DTOs
{
    public class UpdateTaskDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string taskName { get; set; }
        public string status { get; set; }

        [DataType(DataType.Date)]
        public DateTime? expirationDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime createdDay { get; set; }
    }
}
