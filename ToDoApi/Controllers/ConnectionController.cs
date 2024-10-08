using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ToDoApi.Data;

namespace TuNamespace
{
    [ApiController]
    [Route("api/connection")]
    public class ConnectionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public ConnectionController(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        [HttpPost("actualizar-conexion")]
        public IActionResult UpdateConnectionString([FromBody] ConnectionUpdateModel model)
        {
            try
            {
                // Actualizar la configuración en tiempo de ejecución
                _configuration["ConnectionStrings:Cadena"] = model.NewConnectionString;

                // Reinicializar el DbContext con la nueva cadena de conexión
                var optionsBuilder = new DbContextOptionsBuilder<TaskDbContext>();
                optionsBuilder.UseSqlServer(model.NewConnectionString);
                var context = new TaskDbContext(optionsBuilder.Options);

                _serviceProvider.GetRequiredService<TaskDbContext>().Database.GetDbConnection().ConnectionString = model.NewConnectionString;

                return Ok(new { message = "Cadena de conexión actualizada correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error al actualizar la cadena de conexión: {ex.Message}" });
            }
        }

        public class ConnectionUpdateModel
        {
            public string NewConnectionString { get; set; }
        }
    }
}