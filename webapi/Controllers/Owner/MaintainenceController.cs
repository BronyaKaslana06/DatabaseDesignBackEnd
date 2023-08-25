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
            bool flag = long.TryParse(maintenance_item_id, out var id);
            if(!flag)
            {
                var obj = new
                {
                    code = 1,
                    msg = "id非法",
                    totalData = 0,
                    data = "",
                };
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }
            try
            {
                var filteredItems = _context.MaintenanceItems
                    .SelectMany(mi => mi.employees, (mi, emp) => new { MaintenanceItem = mi, Employee = emp })
                    .Join(_context.Employees, miEmp => miEmp.Employee, emp => emp, (miEmp, emp) => new { miEmp.MaintenanceItem, Employee = emp })
                    .Join(_context.Vehicles, miEmp => miEmp.MaintenanceItem.vehicle.VehicleId, veh => veh.VehicleId, (miEmp, veh) => new { miEmp.MaintenanceItem, miEmp.Employee, Vehicle = veh })
                    .Where(joinedData => maintenance_item_id == "" || joinedData.MaintenanceItem.MaintenanceItemId == id)
                    .OrderBy(joinedData => joinedData.MaintenanceItem.MaintenanceItemId)
                    .Select(joinedData => new
                    {
                        maintenance_location = joinedData.MaintenanceItem.MaintenanceLocation,
                        plate_number = joinedData.Vehicle.PlateNumber,
                        title = joinedData.MaintenanceItem.Title,
                        order_submission_time = joinedData.MaintenanceItem.OrderSubmissionTime,
                        service_time = joinedData.MaintenanceItem.ServiceTime,
                        order_status = joinedData.MaintenanceItem.OrderStatus,
                        remarks = joinedData.MaintenanceItem.Note,
                        evaluations = joinedData.MaintenanceItem.Score,
                        name = joinedData.Employee.Name,
                        phone_number = joinedData.Employee.PhoneNumber
                    })
                    .ToList();

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
                .Where(item => item.OrderSubmissionTime >= parsedStartTime && item.OrderSubmissionTime <= parsedEndTime && item.vehicle.VehicleId.ToString() == vehicle_id)
                .OrderBy(item => item.MaintenanceItemId)
                .Select(item => new
                {
                    maintenance_item_id = item.MaintenanceItemId,
                    title = item.Title,
                    order_submission_time = item.OrderSubmissionTime,
                    maintenance_location = item.MaintenanceLocation
                })
                .ToList();
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