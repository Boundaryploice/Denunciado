using DenunciaDo.Core.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DenunciaDo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : BaseApiController
    {
        private readonly IComplaintService _complaintService;
        private readonly IUserService _userService;

        public DashboardController(IComplaintService complaintService, IUserService userService)
        {
            _complaintService = complaintService;
            _userService = userService;
        }

        [HttpGet("stats")]
        [AllowAnonymous]
        public async Task<ActionResult> GetDashboardStats()
        {
            // Implementar estadísticas básicas
            var stats = new
            {
                TotalComplaints = 0, // TODO: Implementar conteo real
                ResolvedComplaints = 0,
                PendingComplaints = 0,
                TotalUsers = 0
            };

            return Ok(stats);
        }

        [HttpGet("recent-activity")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult> GetRecentActivity()
        {
            var recentComplaints = await _complaintService.GetRecentComplaintsAsync(10);
            return Ok(recentComplaints);
        }
    }

}
