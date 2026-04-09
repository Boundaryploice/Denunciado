using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DenunciaDo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public abstract class BaseApiController : ControllerBase
    {
        protected int GetUserId()
        {
            return int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        }
    }
}
