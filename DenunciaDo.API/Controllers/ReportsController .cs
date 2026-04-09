using DenunciaDo.Core.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DenunciaDo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Staff")]
    public class ReportsController : BaseApiController
    {
        private readonly IComplaintService _complaintService;

        public ReportsController(IComplaintService complaintService)
        {
            _complaintService = complaintService;
        }

        [HttpGet("complaints-by-type")]
        public async Task<ActionResult> GetComplaintsByType([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            // TODO: Implementar reporte por tipo
            return Ok(new { message = "Report endpoint - TODO: Implement" });
        }

        [HttpGet("complaints-by-status")]
        public async Task<ActionResult> GetComplaintsByStatus([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            // TODO: Implementar reporte por estado
            return Ok(new { message = "Report endpoint - TODO: Implement" });
        }

        [HttpGet("complaints-by-district")]
        public async Task<ActionResult> GetComplaintsByDistrict([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            // TODO: Implementar reporte por distrito
            return Ok(new { message = "Report endpoint - TODO: Implement" });
        }
    }
}
