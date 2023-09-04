
using EntityFramework.Context;
using EntityFramework.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace webapi.Controllers.Admin
{
    [Route("administrator/[controller]/[action]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ModelContext context;

        public DashboardController(ModelContext modelContext)
        {
            this.context = modelContext;
        }
        [HttpGet]
        public ActionResult<object> Message()
        {
            int totalUsers = context.VehicleOwners.Count();
            int totalOrders = context.SwitchRequests.Count();
            int totalStations = context.SwitchStations.Count();
            int totalStuff = context.Employees.Count();
            return Ok(
                new
                {
                    totalUsers = totalUsers,
                    totalOrders = totalOrders,
                    totalStations = totalStations,
                    totalStuff = totalStuff
                });
        }

        [HttpGet]
        public ActionResult<object> stats()
        {
            int totalUsers = context.VehicleOwners.Count();
            int totalOrders = context.MaintenanceItems.Count();
            int totalStations = context.SwitchStations.Count();
            int totalStuff = context.Employees.Count();
            object data=
                new
                {
                    station_count = totalStations,
                    staff_count = totalStuff,
                    owner_count = totalUsers,
                    maintenance_count = totalOrders,
                    avg_switch_score=context.SwitchRequests.Average(a=>a.switchLog.Score),
                    avg_repair_score=context.MaintenanceItems.Average(a=>a.Score),

                    switch_count=context.SwitchRequests.Count(),
                    cur_switch_count=context.SwitchRequests.Where(
                        a=>a.RequestTime.CompareTo(DateTime.Today)>=0&&a.RequestStatus>=3).Count(),
                    switch_benefit=context.SwitchRequests.Sum(a=>a.switchLog.ServiceFee)
                };

            var a = new
            {
                code = 0,
                msg = "success",
                data=data
            };
            return Content(JsonConvert.SerializeObject(a), "application/json");
        }
        [HttpGet]
        public ActionResult city_stats(int top_count=int.MaxValue)
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
        [HttpGet]
        public ActionResult time_span_stats(string query_range="")
        {
            DateTime selectTime;
            int mode = 0;
            int min = 0;
            int max = 0;
            if (query_range.Contains("year"))
            {
                min = 1;
                max = 12;
                selectTime = Convert.ToDateTime(DateTime.Today.Year + "-" + "01" + "-" + "01");
                mode = 1;
            }
            else if (query_range.Contains("month"))
            {
                min = 1;
                max = DateTime.DaysInMonth(DateTime.Today.Year,DateTime.Today.Month);
                selectTime = DateTime.Today.AddDays(1 - DateTime.Today.Day).Date;
                mode = 2;
            }
            else if (query_range.Contains("day"))
            {
                min = 0;
                max = 23;
                selectTime = DateTime.Today;
                mode = 3;
            }
            else selectTime = DateTime.MinValue;
            var query = context.SwitchLogs
           .Where(a => a.SwitchTime.CompareTo(selectTime) >= 0)
           .GroupBy(a =>
           mode == 1 ? a.switchrequest.RequestTime.Month : (
           mode == 2 ? a.switchrequest.RequestTime.Day :
           (mode == 3 ? a.switchrequest.RequestTime.Hour : a.switchrequest.RequestTime.Year))
           )
           .OrderBy(a => a.Key)
           .Select(a => new
           {
               time_span = a.Key,
               switch_count = a.Count(),
               switch_benefit = a.Sum(a=>a.ServiceFee)
           }).ToList();


            List<object> data = new List<object>();

            for(int i=min;i<=max;i++)
            {
                string j = i + (mode == 3 ? ":00" : "");
                var target = query.Find(a => a.time_span == i);
                data.Add(new{
                    time_span=j,
                    switch_count= target == null ? 0 : target.switch_count
                }
                );
            }

           var obj = new
            {
                code = 0,
                msg = "success",
                data = mode==0? query:(object)data,
            };
            return Content(JsonConvert.SerializeObject(obj), "application/json");
        }
        [HttpGet]
        public ActionResult vehicles(int top_count=int.MaxValue)
        {
            var query = context.Vehicles.Include(a => a.switchRequests).GroupBy(a => a.vehicleParam.ModelName)
            .OrderByDescending(
                a => a.Count()//Sum(a => a.switchRequests.Count())
            )
            .Select(a => new
            {
                vehicle_type = a.Key,
                vehicle_num = a.Count()
            }).Take(top_count);
            var obj = new
            {
                code = 0,
                msg = "success",
                data = query,
            };
            return Content(JsonConvert.SerializeObject(obj), "application/json");
        }
        [HttpGet]
        public ActionResult batteries()
        {
            var query = context.Batteries.GroupBy(a => new {a.AvailableStatus,a.batteryType.Name })
            .ToList()
            .Select(a => new {
                battery_type = ((AvailableStatusEnum) a.Key.AvailableStatus).ToString()+" "+a.Key.Name,
                battery_num = a.Count()
            });
            
            var obj = new
            {
                code = 0,
                msg = "success",
                data = query,
            };
            return Content(JsonConvert.SerializeObject(obj), "application/json");

        }
        [HttpGet]
        public ActionResult week_benefits()
        {
            var query = context.SwitchRequests.
            Where(a => a.RequestTime.CompareTo(DateTime.Today.AddDays(-7)) >= 0).
            OrderByDescending(a => a.RequestTime).
            GroupBy(a => a.RequestTime.Date).
            Select(a => new
            {
                day = a.Key.ToShortDateString(),
                benefits = a.Sum(a => a.switchLog.ServiceFee)
            }).ToList();
            var data = new List<object>();
            for (int i = 0; i < 7; i++)
            {
                float benefits = 0;
                string day = DateTime.Today.AddDays(i - 7).ToShortDateString();
                int index = query.FindIndex(a => a.day == day);
                if (index != -1)
                    benefits = query[index].benefits;
                data.Add(new
                {
                    day,
                    benefits
                });
            }
            var obj = new
            {
                code = 0,
                msg = "success",
                data = data,
            };
            return Content(JsonConvert.SerializeObject(obj), "application/json");
        }
        [HttpGet]
        public ActionResult growth()
        {
            var query = new
            {
                owner_growth = context.VehicleOwners.Where(a => a.CreateTime.CompareTo(DateTime.Today) >= 0).Count(),
                staff_growth = context.Employees.Where(a => a.CreateTime.CompareTo(DateTime.Today) >= 0).Count(),
                request_growth = context.SwitchRequests.Where(a => a.RequestTime.CompareTo(DateTime.Today) >= 0).Count()
                - context.SwitchRequests.Where(a => a.RequestTime.CompareTo(DateTime.Today) < 0 && a.RequestTime.CompareTo(DateTime.Today.AddDays(-1)) >= 0).Count(),
                benefit_growth = context.SwitchRequests.Where(a => a.RequestTime.CompareTo(DateTime.Today) >= 0).Sum(a => a.switchLog.ServiceFee)
                - context.SwitchRequests.Where(a => a.RequestTime.CompareTo(DateTime.Today) < 0 && a.RequestTime.CompareTo(DateTime.Today.AddDays(-1)) >= 0).Sum(a => a.switchLog.ServiceFee)
            };
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
