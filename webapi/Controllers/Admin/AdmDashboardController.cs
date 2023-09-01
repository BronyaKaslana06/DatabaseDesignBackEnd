
using EntityFramework.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace webapi.Controllers.Admin
{
    [Route("administrator/dashboard")]
    [ApiController]
    public class AdmDashboardController : ControllerBase
    {
        private readonly ModelContext context;

        public AdmDashboardController(ModelContext modelContext)
        {
            this.context = modelContext;
        }

        [HttpGet("stats")]
        public ActionResult<object> Message()
        {
            int totalUsers = context.VehicleOwners.Count();
            int totalOrders = context.SwitchRequests.Count();
            int totalStations = context.SwitchStations.Count();
            int totalStuff = context.Employees.Count();
            object data=
                new
                {
                    owner_count = totalUsers,
                    maintenance_count = totalOrders,
                    station_count = totalStations,
                    staff_count = totalStuff
                };

            var a = new
            {
                code = 0,
                msg = "success",
                data
            };
            return Content(JsonConvert.SerializeObject(a), "application/json");
        }
        [HttpGet("city_stats")]
        public ActionResult City(int top_count=int.MaxValue)
        {
            var query = context.SwitchLogs
            .GroupBy(a => a.switchrequest.Position.Substring(2, 2)).Select(a => new
            {
                city = a.Key,
                switch_count = a.Count()
            }).OrderByDescending(a=>a.switch_count).
            //Take(top_count).
            ToList();
            List<object> final = new List<object>();
            int sum = 0;
            foreach(var a in query)
            {
                sum += a.switch_count;
            }

            foreach (var a in query)
            {
                final.Add(new
                {
                    city = a.city,
                    switch_count = a.switch_count,
                    percentage = (float)a.switch_count / sum
                });
            }

            var obj = new
            {
                code = 0,
                msg = "success",
                data = final,
            };
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            return NewContent();
        }
        [HttpGet("time_span_stats")]
        public ActionResult Time_span()
        {
            var query = context.SwitchLogs
           .GroupBy(a => a.switchrequest.switchStation.TimeSpan).Select(a => new
           {
               time_span = a.Key,
               switch_count = a.Count()
           }).OrderByDescending(a => a.switch_count);
           var obj = new
            {
                code = 0,
                msg = "success",
                data = query,
            };
                return Content(JsonConvert.SerializeObject(obj), "application/json");
        }
        ContentResult NewContent(int _code = 0, string _msg = "success")
        {
            var a = new
            {
                code = _code,
                msg = _msg
            };
            return Content(JsonConvert.SerializeObject(a), "application/json");
        }
    }
}
