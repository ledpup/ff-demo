using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace VDS.CourseAdminMastering.WebApi.Controllers
{
    [Route("api/v1")]
    [ApiController]
    //Remove the other media types except application/json from swagger UI
    [Consumes("application/json")]
    [Produces("application/json")]

    public class BaseController : ControllerBase
    {
        
    }
}