using EntityFramework.Context;
using EntityFramework.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace webapi.Controllers.Staff
{
    [Route("staff/[controller]/[action]")]
    [ApiController]
    public class SwitchRequestController : ControllerBase
    {
        private readonly ModelContext _context;

        public SwitchRequestController(ModelContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpGet]
        public ActionResult<string> detail(string switch_request_id)
        {
            long request_id = Convert.ToInt64(switch_request_id);
            Console.WriteLine(request_id);
            var request = _context.SwitchRequests
                .Include(a=>a.vehicle)
                .Include(b=>b.vehicle.vehicleOwner)
                .Include(c=>c.vehicle.vehicleParam)
                .Include(d=>d.employee)
                .Include(e=>e.employee.switchStation)
                .Include(f=>f.batteryType)
                .Where(s => s.SwitchRequestId == request_id).FirstOrDefault();
            if (request == null)
                return NotFound("Request not found.");

            var log = _context.SwitchLogs
                .Include(a=>a.batteryOn)
                .Include(b=>b.batteryOff)
                .Include(c=>c.switchrequest)
                .Where(sl=>sl.switchRequestId == request_id).FirstOrDefault();

            var data = new
            {
                switch_request = new
                {
                    switch_request_id = request.SwitchRequestId,
                    switch_type = request.SwitchTypeEnum.ToString(),
                    request_time = request.RequestTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    position = request.Position,
                    longitude = request.Longitude,
                    latitude = request.Latitude,
                    username = request.vehicle.vehicleOwner.Username,
                    phone_number = request.vehicle.vehicleOwner.PhoneNumber,
                    station_name = request.employee.switchStation.StationName,
                    remarks = request.Note,
                    plate_number = request.vehicle.PlateNumber,
                    vehicle_model = request.vehicle.vehicleParam.ModelName,
                    battery_type = request.batteryType.Name,
                    employee_id = request.employee.EmployeeId.ToString(),
                    switch_date = request.Date.ToString("yyyy-MM-dd"),
                    switch_period = EnumDisplay.GetDisplayNameFromEnum(request.PeriodEnum),
                    order_status = request.requestStatusEnum.ToString()
                },
                switch_log = new
                {
                    score = log==null?(double)-1:log.Score,
                    evaluation = log == null ? "":log.Evaluation,
                    batteryOnId = log == null ? (long)-1:log.batteryOn.BatteryId,
                    batteryOffId = log == null ? (long)-1:log.batteryOff.BatteryId,
                    switch_time = (log == null ? DateTime.MaxValue:log.SwitchTime).ToString("yyyy-MM-dd HH:mm:ss")
                }
            };

            var a = new
            {
                code = 0,
                msg = "success",
                data
            };
            return Content(JsonConvert.SerializeObject(a), "application/json");
        }

        [Authorize]
        [HttpGet]
        public ActionResult<string> doortodoor(string station_id, string employee_id, string request_status)
        {
            var station = _context.SwitchStations.Where(e => e.StationId == Convert.ToInt64(station_id)).DefaultIfEmpty().FirstOrDefault();
            if (station == null)
                return NotFound("Station not found.");
            if (request_status == "待接单")
            {
                employee_id = "-1";
            }
            else
            {
                var employee = _context.Employees.Where(e => e.EmployeeId == Convert.ToInt64(employee_id)).DefaultIfEmpty().FirstOrDefault();
                if (employee == null)
                    return NotFound("Employee not found.");
            }

            RequestStatusEnum Ordertype = RequestStatusEnum.未知;
            if (Enum.TryParse(request_status, out RequestStatusEnum typeEnum))
                Ordertype = typeEnum;
            else
                return NotFound("Order_type error.");
            var query = _context.SwitchRequests
                .Where(a => a.employee.switchStation.StationId == Convert.ToInt64(station_id) &&
                a.SwitchType == (int)SwitchTypeEnum.上门换电 &&
                a.RequestStatus == (int)Ordertype &&
                (Convert.ToInt64(employee_id) > 0 ? a.employee.EmployeeId == Convert.ToInt64(employee_id) : true)
                )
                .OrderByDescending(a => a.RequestTime)
                .Select(switch_request => new
                {
                    switch_request.employee.EmployeeId,
                    switch_request_id = switch_request.SwitchRequestId,
                    plate_number = switch_request.vehicle.PlateNumber,
                    vehicle_model = switch_request.vehicle.vehicleParam.ModelName,
                    position = switch_request.Position,
                    username = switch_request.vehicle.vehicleOwner.Username,
                    phone_number = switch_request.vehicle.vehicleOwner.PhoneNumber,
                    request_time = switch_request.RequestTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    order_status = switch_request.requestStatusEnum.ToString(),
                    battery_type = switch_request.batteryType.Name,
                    switch_date = switch_request.Date.ToString("yyyy-MM-dd"),
                    switch_period = EnumDisplay.GetDisplayNameFromEnum(switch_request.PeriodEnum)
                }).ToList();

            var a = new
            {
                code = 0,
                msg = "success",
                switch_request_array = query
            };
            return Content(JsonConvert.SerializeObject(a), "application/json");
        }

        [Authorize]
        [HttpGet]
        public ActionResult<string> reservation(string station_id, string request_status)
        {
            if (request_status == "待接单")
            {
                return NotFound("Request_status error.");
            }
            var station = _context.SwitchStations.Where(e => e.StationId == Convert.ToInt64(station_id)).DefaultIfEmpty().FirstOrDefault();
            if (station == null)
                return NotFound("Station not found.");

            RequestStatusEnum Ordertype = RequestStatusEnum.未知;
            if (Enum.TryParse(request_status, out RequestStatusEnum typeEnum))
                Ordertype = typeEnum;
            else
                return NotFound("Order_type error.");
            var query = _context.SwitchRequests
                .Where(a => a.employee.switchStation.StationId == Convert.ToInt64(station_id) &&
                a.SwitchType == (int)SwitchTypeEnum.到店换电 &&
                a.RequestStatus == (int)Ordertype
                )
                .OrderByDescending(a => a.Date)
                .Select(switch_request => new
                {
                    switch_request.employee.EmployeeId,
                    switch_request_id = switch_request.SwitchRequestId,
                    plate_number = switch_request.vehicle.PlateNumber,
                    vehicle_model = switch_request.vehicle.vehicleParam.ModelName,
                    position = switch_request.Position,
                    username = switch_request.vehicle.vehicleOwner.Username,
                    phone_number = switch_request.vehicle.vehicleOwner.PhoneNumber,
                    request_time = switch_request.RequestTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    order_status = switch_request.requestStatusEnum.ToString(),
                    battery_type = switch_request.batteryType.Name,
                    switch_date = switch_request.Date.ToString("yyyy-MM-dd"),
                    switch_period = EnumDisplay.GetDisplayNameFromEnum(switch_request.PeriodEnum)
                }).ToList();

            var a = new
            {
                code = 0,
                msg = "success",
                switch_request_array = query
            };
            return Content(JsonConvert.SerializeObject(a), "application/json");
        }

        [Authorize]
        [HttpPost]
        public ActionResult<string> receive([FromBody] dynamic _body)
        {
            using (TransactionScope tx = new TransactionScope())
            {

                dynamic body = JsonConvert.DeserializeObject<dynamic>(_body.ToString());

                long employee_id = Convert.ToInt64(body.employee_id);
                var employee = _context.Employees.Where(s => s.EmployeeId == employee_id).DefaultIfEmpty().FirstOrDefault();
                if (employee == null)
                    return NotFound("Employee not found.");
                long request_id = Convert.ToInt64(body.switch_request_id);
                var request = _context.SwitchRequests.Include(a => a.batteryType).Where(s => s.SwitchRequestId == request_id).DefaultIfEmpty().FirstOrDefault();
                if (request == null)
                    return NotFound("Switch request not found.");

                if (request.RequestStatus != (int)RequestStatusEnum.待接单)
                    return BadRequest("订单状态不是待接单，无法接单！");
                request.employee = employee;
                request.requestStatusEnum = RequestStatusEnum.待完成;

                long barry_type_id = request.batteryType.BatteryTypeId;
                var battery = _context.Batteries.Where(s => s.batteryType.BatteryTypeId == barry_type_id).DefaultIfEmpty().ToList()[0];
                if (battery == null)
                    return NotFound("Battery not found.");
                battery.AvailableStatusEnum = AvailableStatusEnum.已预定;

                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    return Conflict();
                }
                tx.Complete();
                return Ok("已接单" + request.SwitchRequestId.ToString());
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult<string> submit([FromBody] dynamic _body)
        {
            using (TransactionScope tx = new TransactionScope())
            {
                dynamic body = JsonConvert.DeserializeObject<dynamic>(_body.ToString());
                long request_id = Convert.ToInt64(body.switch_request_id);
                var request = _context.SwitchRequests
                    .Include(a => a.batteryType)
                    .Include(b => b.employee)
                    .Include(c => c.employee.switchStation)
                    .Include(d => d.vehicle)
                    .Include(e => e.vehicle.vehicleOwner)
                    .Include(f => f.vehicle.Battery)
                    .FirstOrDefault(s => s.SwitchRequestId == request_id);
                if (request == null)
                    return NewContent(1, "Switch request not found.");
                if (request.RequestStatus != (int)RequestStatusEnum.待完成)
                    return NewContent(1, "订单状态不是待完成，无法接单！");

                var batteryOff = request.vehicle.Battery;
                if (batteryOff == null)
                    return NewContent(1, "Vehicle has no battery.");

                var station = request.employee.switchStation;
                var battery_type = request.batteryType;
                var batteryOn = _context.Batteries.Where(s =>
                s.switchStation == station &&
                s.batteryType == battery_type &&
                s.AvailableStatus == (int)AvailableStatusEnum.已预定).DefaultIfEmpty().FirstOrDefault();
                if (batteryOn == null)
                    return NewContent(1, "Staion has no battery.");

                batteryOn.AvailableStatusEnum = AvailableStatusEnum.汽车使用中;
                batteryOff.AvailableStatusEnum = AvailableStatusEnum.充电中;
                batteryOn.switchStation = null;
                batteryOff.switchStation = station;
                station.AvailableBatteryCount--;

                request.vehicle.Battery = batteryOn;
                request.requestStatusEnum = RequestStatusEnum.待评价;

                long id = _context.SwitchLogs.Max(e => e.SwitchServiceId) + 1;
                SwitchLog log = new SwitchLog()
                {
                    SwitchServiceId = id,
                    SwitchTime = DateTime.Now,
                    batteryOn = batteryOn,
                    batteryOff = batteryOff,
                    switchrequest = request,
                    switchRequestId = request.SwitchRequestId,
                    Score = -1,
                    Evaluation = ""
                };

                var isExistLog = _context.SwitchLogs.Where(s => s.switchRequestId == request_id).DefaultIfEmpty().FirstOrDefault();
                if (isExistLog == null)
                    _context.SwitchLogs.Add(log);
                else
                    isExistLog = log;

                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    return NewContent(1, "数据库修改失败");
                }

                var obj = new
                {
                    code = 0,
                    msg = "success",
                    switch_log = new
                    {
                        switch_service_id = log.SwitchServiceId,
                        switch_time = log.SwitchTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        batteryOnId = log.batteryOn.BatteryId,
                        batteryOffId = log.batteryOff.BatteryId,
                        switch_request_id = log.switchrequest.SwitchRequestId,
                        score = log.Score,
                        evaluation = log.Evaluation
                    }
                };
                tx.Complete();
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }
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
