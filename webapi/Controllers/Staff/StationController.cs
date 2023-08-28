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
using System.Transactions;
using System.Collections.Generic;

namespace webapi.Controllers.Staff
{
    [Route("staff/switchstation/[action]")]
    [ApiController]
    public class StationController : ControllerBase
    {
        private readonly ModelContext _context;

        public StationController(ModelContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Employee>> info(string employee_id = "")
        {
            if (!long.TryParse(employee_id, out long id))
            {
                var obj = new
                {
                    code = 1,
                    msg = "id非法或不存在",
                    data = ""
                };
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }

            var query = _context.Employees
                    .Where(e => e.EmployeeId == id)
                    .Select(e => new
                    {
                        station_id = e.switchStation.StationId.ToString(),
                        station_name = e.switchStation.StationName,
                        longitude = e.switchStation.Longitude,
                        latitude = e.switchStation.Latitude,
                        faliure_status = e.switchStation.FailureStatus,
                        battery_capacity = e.switchStation.BatteryCapacity,
                        available_battery_count = e.switchStation.AvailableBatteryCount
                    })
                    .FirstOrDefault();

            var totalNum = _context.VehicleOwners.Count();
            var responseObj = new
            {
                code = 0,
                msg = "success",
                data = query,
            };
            return Content(JsonConvert.SerializeObject(responseObj), "application/json");
        }

        [HttpGet]
        public ActionResult<IEnumerable<Battery>> battery(string station_id = "", string available_status = "", string battery_type_id = "")
        {
            if (!long.TryParse(station_id, out long id))
            {
                var obj = new
                {
                    code = 1,
                    msg = "id非法或不存在",
                    totaldata = 0,
                    data = ""
                };
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }

            var query = _context.Batteries
                    .Where(b => b.switchStation.StationId == id && battery_type_id == "" ? true : b.batteryType.BatteryTypeId.ToString() == battery_type_id && available_status == "" ? true : Enum.GetName(typeof(AvailableStatusEnum), b.AvailableStatusEnum) == available_status)
                    .Select(b => new
                    {
                        battery_id = b.BatteryId.ToString(),
                        available_status = Enum.GetName(typeof(AvailableStatusEnum), b.AvailableStatusEnum),
                        current_capacity = b.CurrentCapacity,
                        curr_charge_times = b.CurrChargeTimes,
                        manufacturing_date = b.ManufacturingDate.ToString(),
                        battery_type_id = b.batteryType.BatteryTypeId
                    })
                    .ToList();

            var totalNum = _context.VehicleOwners.Count();
            var responseObj = new
            {
                code = 0,
                msg = "success",
                totaldata = totalNum,
                data = query,
            };
            return Content(JsonConvert.SerializeObject(responseObj), "application/json");
        }

        [HttpGet]
        public ActionResult<IEnumerable<SwitchRequest>> switchrequest(string station_id = "")
        {
            if (!long.TryParse(station_id, out long id))
            {
                var obj = new
                {
                    code = 1,
                    msg = "id非法或不存在",
                    totaldata = 0,
                    data = ""
                };
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }

            var query = _context.SwitchRequests
                    .Where(sr => sr.switchStation.StationId == id)
                    .Select(sr => new
                    {
                        switch_request_id = sr.SwitchRequestId.ToString(),
                        username = sr.vehicleOwner.Username,
                        plate_number = sr.vehicle.PlateNumber,
                        request_time = sr.RequestTime.ToString(),
                        remarks = sr.Note,
                    })
                    .ToList();

            var totalNum = _context.VehicleOwners.Count();
            var responseObj = new
            {
                code = 0,
                msg = "success",
                totaldata = totalNum,
                data = query,
            };
            return Content(JsonConvert.SerializeObject(responseObj), "application/json");
        }
        [HttpPatch]
        public IActionResult battery([FromBody] dynamic param)
        {
            dynamic _param = JsonConvert.DeserializeObject(Convert.ToString(param));
            var bty = _context.Batteries.Find(_param.battery_id);
            if (bty == null)
            {
                return NewContent(1, "查询电池不存在");
            }

            bty.available_status = _param.available_status ?? param.available_status;
            bty.CurrentCapacity = _param.current_capacity ?? param.current_capacity;
            bty.batteryType = (_param.battery_type ?? param.battery_type).Find();
            
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                return NewContent(2, e.InnerException?.Message + "");
            }

            return NewContent();
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
