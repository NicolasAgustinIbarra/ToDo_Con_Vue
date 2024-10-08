using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoApi.Models
{
    public class TaskModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int taskId { get; set; }

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
