using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;
using ToDoApi.Data;
using ToDoApi.Models;
using ToDoApi.Models.DTOs;

namespace ToDoApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly TaskDbContext _dbContext;
        public TaskController(TaskDbContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<TaskDTO>>> GetAllTasks()
        {
            var tasks = await _dbContext.Tasks.ToListAsync();

            if (tasks == null || !tasks.Any())
            {
                return NotFound("Aún no hay tareas creadas...");
            }

           
            var taskDtos = tasks.Select(task => new TaskDTO
            {   createdDay = task.createdDay.ToString("dd/MM/yyyy"),
                taskId = task.taskId,
                taskName = task.taskName,
                status = task.status,
                expirationDate = task.expirationDate?.ToString("dd/MM/yyyy"), 
            }).ToList();

            return Ok(taskDtos);
        }


        [HttpPost]
        public async Task<ActionResult<TaskModel>> CreateTask([FromBody] CreateTaskDTO newTask)
        {
            if (newTask == null)
            {
                return BadRequest("Debes completar la tarea");
            }

            if (newTask.taskName.Length < 3)
            {
                return BadRequest("El nombre de la tarea debe tener al menos 3 caracteres");
            }

            if (newTask.taskName.Length > 100)
            {
                return BadRequest("El nombre de la tarea no debe tener más de 100 caracteres");
            }

     
            var invalidCharsRegex = new Regex("[^a-zA-Z0-9#$%&()\\[\\]; ]");

            if (invalidCharsRegex.IsMatch(newTask.taskName))
            {
                return BadRequest("El nombre de la tarea solo puede contener letras, números y símbolos: $ # %& ( ) [ ] ;");
            }

          
            if (newTask.taskName.All(c => char.IsUpper(c)))
            {
                return BadRequest("El nombre de la tarea no debe estar escrito todo en mayúsculas");
            }

            DateTime? expirationDate = null;

            if (!string.IsNullOrEmpty(newTask.expirationDate.ToString()))
            {
                try
                {
                    expirationDate = DateTime.Parse(newTask.expirationDate.ToString());
                }
                catch (FormatException)
                {
                    return BadRequest("La fecha de expiración inválida. Debe estar en el formato dd/mm/yyyy.");
                }

                if (expirationDate.Value.DayOfWeek == DayOfWeek.Saturday || expirationDate.Value.DayOfWeek == DayOfWeek.Sunday)
                {
                    return BadRequest("La fecha de expiración no puede ser un fin de semana");
                }

                if (expirationDate.Value > DateTime.Now.AddDays(30))
                {
                    return BadRequest("La fecha de expiración no puede ser más de 30 días posterior a la fecha de creación");
                }
            }

            
            var task = new TaskModel
            {
                taskName = newTask.taskName,
                status = "activa",
                createdDay = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                expirationDate = expirationDate
            };

            await _dbContext.AddAsync(task);
            await _dbContext.SaveChangesAsync();

            return Ok(task);
            }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask(int id)
        {
      
            if (id == 0)
            {
                return BadRequest("No se ha especificado una tarea para eliminar");
            }

       
            var taskToDelete = await _dbContext.Tasks.FirstOrDefaultAsync(x => x.taskId == id);

          
            if (taskToDelete == null)
            {
                return NotFound("La tarea con el ID especificado no existe");
            }


            _dbContext.Tasks.Remove(taskToDelete);
            await _dbContext.SaveChangesAsync();

            return Ok("La tarea se ha eliminado exitosamente");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TaskModel>> UpdateTask(int id, UpdateTaskDTO updatedTask)
        {
            if (id == 0)
            {
                return BadRequest("No se ha especificado una tarea para actualizar");
            }
            
            var task = await _dbContext.Tasks.FirstOrDefaultAsync(x=> x.taskId == id);

            if (task == null)
            {
                return NotFound("La tarea con el ID especificado no existe");
            }

            if (updatedTask.taskName.Length < 3)
            {
                return BadRequest("El nombre de la tarea debe tener al menos 3 caracteres");
            }

            if (updatedTask.taskName.Length > 100)
            {
                return BadRequest("El nombre de la tarea no debe tener más de 100 caracteres");
            }

           
            var invalidCharsRegex = new Regex("[^a-zA-Z0-9#$%&()\\[\\]; ]");

            if (invalidCharsRegex.IsMatch(updatedTask.taskName))
            {
                return BadRequest("El nombre de la tarea solo puede contener letras, números y símbolos: $ # %& ( ) [ ] ;");
            }

          
            if (updatedTask.taskName.All(c => char.IsUpper(c)))
            {
                return BadRequest("El nombre de la tarea no debe estar escrito todo en mayúsculas");
            }



            DateTime? expirationDate = null;

            if (!string.IsNullOrEmpty(updatedTask.expirationDate.ToString()))
            {
                try
                {
                    expirationDate = DateTime.Parse(updatedTask.expirationDate.ToString());
                }
                catch (FormatException)
                {
                    return BadRequest("Fecha de expiración inválida. Debe estar en el formato dd/mm/yyyy.");
                }

                if (expirationDate.Value.DayOfWeek == DayOfWeek.Saturday || expirationDate.Value.DayOfWeek == DayOfWeek.Sunday)
                {
                    return BadRequest("La fecha de expiración no puede ser un fin de semana");
                }

                if (expirationDate.Value > DateTime.Now.AddDays(30))
                {
                    return BadRequest("La fecha de expiración no puede ser más de 30 días posterior a la fecha de creación");
                }

              
            }

            if(updatedTask.status == "diferida")
            {
                task.expirationDate = null;
            }
            else
            {
                task.expirationDate = expirationDate;
            }

            task.taskName = updatedTask.taskName;
            task.status = updatedTask.status;

            await _dbContext.SaveChangesAsync();

            
            return Ok(task);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskModel>> GetTaskById(int id)
        {
            if (id == 0)
            {
                return BadRequest("No se ha seleccionado una tarea.");
            }

            var task = await _dbContext.Tasks.FirstOrDefaultAsync(x => x.taskId == id);

            if (task == null)
            {
                return NotFound("La tarea no existe.");
            }

            var taskDto = new TaskDTO
            {
                taskId = task.taskId,
                taskName = task.taskName,
                status = task.status,
                createdDay = task.createdDay.ToString("dd/MM/yyyy"),
                expirationDate = task.expirationDate?.ToString("dd/MM/yyyy"),
            };

            return Ok(taskDto);
        }
        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateTaskStatus(int id, string newStatus)
        {
            if (id == 0)
            {
                return BadRequest("No se ha especificado un ID de tarea válido.");
            }

            var task = await _dbContext.Tasks.FirstOrDefaultAsync(x => x.taskId == id);

            if (task == null)
            {
                return NotFound("La tarea con el ID especificado no existe.");
            }

            if (!newStatus.ToLower().Equals("activa") && !newStatus.ToLower().Equals("completada") && !newStatus.ToLower().Equals("diferida"))
            {
                return BadRequest("El estado especificado no es válido. Los estados permitidos son: Activa, Completada, Diferida.");
            }

            if (newStatus.ToLower().Equals("diferida") && task.expirationDate.HasValue)
            {
                task.expirationDate = null; 
            }

            task.status = newStatus;

            await _dbContext.SaveChangesAsync();

            return Ok("El estado de la tarea se ha actualizado correctamente.");
        }

    }
}
