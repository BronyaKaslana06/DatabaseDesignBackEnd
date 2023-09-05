using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.ContentModel;
using NuGet.Protocol;
using System.Data;
using System.Xml.Linq;
using EntityFramework.Context;
using EntityFramework.Models;
using Oracle.ManagedDataAccess.Client;
using Microsoft.AspNetCore.Authorization;

namespace webapi.Controllers.Admin
{
    [Route("administrator/switch-info")]
    [ApiController]
    public class SwitchInfoController : ControllerBase
    {
        private readonly ModelContext _context;

        public SwitchInfoController(ModelContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("query")]
        public ActionResult<IEnumerable<SwitchLog>> GetPage_(int page_index, int page_size, string switch_service_id = "", string employee_id = "", string vehicle_id = "")
        {
            int offset = (page_index - 1) * page_size;
            int limit = page_size;

            if (offset < 0 || limit <= 0)
            {
                var errorResponse = new
                {
                    code = 1,
                    msg = "页码或页大小非正",
                    totalData = 0,
                    data = "",
                };
                return Content(JsonConvert.SerializeObject(errorResponse), "application/json");
            }

            var pattern1 = "%" + (string.IsNullOrEmpty(switch_service_id) ? "" : switch_service_id) + "%";
            var pattern2 = "%" + (string.IsNullOrEmpty(employee_id) ? "" : employee_id) + "%";
            var pattern3 = "%" + (string.IsNullOrEmpty(vehicle_id) ? "" : vehicle_id) + "%";

            var query = _context.SwitchLogs
                .Where(sl =>
                    EF.Functions.Like(sl.SwitchServiceId.ToString(), pattern1) &&
                    EF.Functions.Like(sl.switchrequest.employee.EmployeeId.ToString(), pattern2) &&
                    EF.Functions.Like(sl.switchrequest.vehicle.VehicleId.ToString(), pattern3))
                .OrderBy(sl => sl.SwitchServiceId)
                .Select(sl => new
                {
                    switch_service_id = sl.SwitchServiceId.ToString(),
                    employee_id = sl.switchrequest.employee.EmployeeId.ToString(),
                    vehicle_id = sl.switchrequest.vehicle.VehicleId.ToString(),
                    ownerid = sl.switchrequest.vehicle.vehicleOwner.OwnerId,
                    switch_time = sl.SwitchTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    battery_id_on = sl.batteryOn.BatteryId.ToString(),
                    battery_id_off = sl.batteryOff.BatteryId.ToString(),
                    evaluations = sl.Evaluation,
                    score = sl.Score == -1 ? "未评分" : sl.Score.ToString()
                })
                .Skip(offset)
                .Take(limit)
                .DefaultIfEmpty()
                .ToList();

            var totalNum = _context.SwitchLogs.Count();

            var responseObj = new
            {
                totalData = totalNum + 25 * page_index - 25,
                data = query,
            };
            return Content(JsonConvert.SerializeObject(responseObj), "application/json");
        }

        [Authorize]
        [HttpDelete]
        public IActionResult DeleteLog(string switch_service_id = "")
        {
            if (_context.SwitchLogs == null)
            {
                return NotFound();
            }
            bool flag = long.TryParse(switch_service_id, out long id);
            var ServiceLog = _context.SwitchLogs.Find(id);
            if (ServiceLog == null)
            {
                return NotFound();
            }
            _context.SwitchLogs.Remove(ServiceLog);
            _context.SaveChanges();

            return NoContent();
        }

        private bool LogExists(string id)
        {
            return _context.SwitchLogs?.Any(e => e.SwitchServiceId.ToString() == id) ?? false;
        }
    }
}
