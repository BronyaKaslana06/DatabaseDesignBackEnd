using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using EntityFramework.Context;
using EntityFramework.Models;
using Idcreator;
using System.Drawing.Printing;
using System.Linq;
using System.Globalization;

namespace webapi.Controllers.Administrator
{
    [Route("owner/vehicle_maintenance_info/[action]")]
    [ApiController]
    public class VMInfoController : ControllerBase
    {
        private readonly ModelContext _context;

        public VMInfoController(ModelContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<MaintenanceItem>> query(string maintenance_item_id = "")
        {
            try
            {
                var filteredItems = _context.MaintenanceItems
                        .Join(_context.MaintenanceItemEmployees, mi => mi.MaintenanceItemId, mie => mie.MaintenanceItemId, (mi, mie) => new { mi, mie })
                        .Join(_context.Employees, j => j.mie.EmployeeId, emp => emp.EmployeeId, (j, emp) => new { j.mi, j.mie, emp })
                        .Join(_context.Vehicles, j => j.mi.VehicleId, veh => veh.VehicleId, (j, veh) => new { j.mi, j.mie, j.emp, veh })
                        .Where(j => maintenance_item_id == "" || j.mi.MaintenanceItemId == maintenance_item_id)
                        .OrderBy(j => j.mi.MaintenanceItemId)
                        .Select(j => new
                        {
                            j.mi.MaintenanceLocation,
                            j.veh.PlateNumber,
                            j.mi.Title,
                            j.mi.OrderSubmissionTime,
                            j.mi.ServiceTime,
                            j.mi.OrderStatus,
                            j.mi.Remarks,
                            j.mi.Evaluations,
                            j.emp.Name,
                            j.emp.PhoneNumber
                        });

                var totalNum = filteredItems.Count();

                var obj = new
                {
                    code = 0,
                    msg = "success",
                    totalData = totalNum,
                    data = filteredItems,
                };

                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    code = 1,
                    msg = "error",
                    totalData = 0,
                    data = new DataTable(),
                };

                return Content(JsonConvert.SerializeObject(errorResponse), "application/json");
            }
        }
        [HttpGet]
        public ActionResult<IEnumerable<MaintenanceItem>> rough_query(string vehicle_id = "", string start_time = "", string end_time = "")
        {
            DateTime parsedStartTime, parsedEndTime;

            if (!DateTime.TryParseExact(start_time, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedStartTime) ||
                !DateTime.TryParseExact(end_time, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedEndTime))
            {
                var errorObj = new
                {
                    code = -1,
                    msg = "日期格式不合法。请使用yyyy-MM-dd。",
                    totalData = 0,
                    data = new DataTable(),
                };
                return Content(JsonConvert.SerializeObject(errorObj), "application/json");
            }

            var filteredItems = _context.MaintenanceItems
                .Where(item => item.OrderSubmissionTime >= parsedStartTime && item.OrderSubmissionTime <= parsedEndTime && item.VehicleId == vehicle_id)
                .OrderBy(item => item.MaintenanceItemId)
                .Select(item => new
                {
                    item.MaintenanceItemId,
                    item.Title,
                    item.OrderSubmissionTime,
                    item.MaintenanceLocation
                });
            int totalNum = filteredItems.Count();
            var obj = new
            {
                code = 0,
                msg = "success",
                totalData = totalNum,
                data = filteredItems,
            };
            return Content(JsonConvert.SerializeObject(obj), "application/json");
        }
    }
}