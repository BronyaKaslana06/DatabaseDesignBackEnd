using EntityFramework.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers.Admin
{
    [Route("administrator/[controller]/[action]")]
    [ApiController]
    public class dashboardController : ControllerBase
    {
        private readonly ModelContext modelContext;

        public dashboardController(ModelContext modelContext)
        {
            this.modelContext = modelContext;
        }

        [HttpGet]
        public ActionResult<object> Message()
        {
            int totalUsers=modelContext.VehicleOwners.Count();
            int totalOrders=modelContext.SwitchRequests.Count();
            int totalStations=modelContext.SwitchStations.Count();
            int totalStuff=modelContext.Employees.Count();
            return Ok(
                new {totalUsers=totalUsers, 
                totalOrders=totalOrders, 
                totalStations=totalStations, 
                totalStuff=totalStuff});
        }

    }
}
