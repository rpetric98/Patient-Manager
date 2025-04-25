using ClassLibrary.DataContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly MedicalDbContext _dbContext;
        public HealthController(MedicalDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> CheckHealth()
        {
            try
            {
                await _dbContext.Database.ExecuteSqlRawAsync("SELECT 1");
                return Ok("Database connection is healthy.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Database connection is not healthy: {ex.Message}");
            }
        }
    }
}
