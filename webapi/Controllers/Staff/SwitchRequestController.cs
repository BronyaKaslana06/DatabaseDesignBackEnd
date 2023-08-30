using EntityFramework.Context;
using EntityFramework.Models;
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

        [HttpGet]
        public ActionResult<string> detail(long switch_request_id)
        {
            var request = _context.SwitchRequests.FirstOrDefault(s => s.SwitchRequestId == switch_request_id);
            if (request == null)
                return NotFound("Request not found.");

            var log = from  r in _context.SwitchLogs
                      where r.switchrequest == request
                      select new
                      {
                          score = r.Score,
                          evaluation = r.Evaluation,
                          batteryOnId = r.batteryOn.BatteryId,
                          batteryOffId = r.batteryOff.BatteryId,
                          switch_time = r.SwitchTime
                      };

            var query = from sr in _context.SwitchRequests
                        where sr.SwitchRequestId == switch_request_id
                        select new
                        {
                            switch_request_id = sr.SwitchRequestId,
                            switch_type = sr.SwitchTypeEnum.ToString(),
                            request_time = sr.RequestTime,
                            position = sr.Position,
                            longitude = sr.Longitude,
                            latitude = sr.Latitude,
                            username = sr.vehicleOwner.Username,
                            phone_number = sr.vehicleOwner.PhoneNumber,
                            station_name = sr.switchStation.StationName,
                            remarks = sr.Note,
                            plate_number = sr.vehicle.PlateNumber,
                            vehicle_model = sr.vehicle.vehicleParam.ModelName,
                            battery_type = sr.batteryType.Name,
                            employee_id = sr.employee.EmployeeId.ToString(),
                            switch_date = sr.Date,
                            switch_period = sr.Period,
                            order_status = sr.requestStatusEnum.ToString()
                        };

            if (query.Count()==0)
                return NotFound("无该请求！");

            var data = new
            {
                switch_request = query.ToList()[0],
                switch_log = log.Count() == 0 ? new
                {
                    score = (double)-1,
                    evaluation = "",
                    batteryOnId = (long)-1,
                    batteryOffId = (long)-1,
                    switch_time = DateTime.MaxValue
                } : log.ToList()[0]
            };

            var a = new
            {
                code = 200,
                msg = "success",
                data
            };
            return Content(JsonConvert.SerializeObject(a), "application/json");
        }

        [HttpGet]
        public ActionResult<string> doortodoor(long station_id, long employee_id, string request_status)
        {
            var station = _context.SwitchStations.FirstOrDefault(s => s.StationId == station_id);
            if (station == null)
                return NotFound("Station not found.");

            if (request_status == "待接单")
            {
                employee_id = -1;
            }
            else
            {
                var employee = _context.Employees.FirstOrDefault(s => s.EmployeeId == employee_id);
                if (employee == null)
                    return NotFound("Employee not found.");
            }

            RequestStatusEnum Ordertype = RequestStatusEnum.未知;
            if (Enum.TryParse(request_status, out RequestStatusEnum typeEnum))
                Ordertype = typeEnum;
            else
                return NotFound("Order_type error.");
            var query = _context.SwitchRequests
                .Where(a => a.switchStation.StationId == station_id && 
                a.SwitchType == (int)SwitchTypeEnum.上门换电 &&
                a.RequestStatus == (int)Ordertype &&
                (employee_id>0? a.employee.EmployeeId==employee_id:true)
                )
                .Select(switch_request => new
                {
                    switch_request.employee.EmployeeId,
                    switch_request_id = switch_request.SwitchRequestId,
                    plate_number = switch_request.vehicle.PlateNumber,
                    vehicle_model = switch_request.vehicle.vehicleParam.ModelName,
                    position = switch_request.Position,
                    username = switch_request.vehicleOwner.Username,
                    phone_number = switch_request.vehicleOwner.PhoneNumber,
                    request_time = switch_request.RequestTime,
                    order_status = switch_request.requestStatusEnum.ToString()
                }).ToList();

            if (query.Count == 0)
            {
                return NotFound("无订单！");
            }

            var a = new
            {
                code = 0,
                msg = "success",
                switch_request_array = query
            };
            return Content(JsonConvert.SerializeObject(a), "application/json");
        }

        [HttpGet]
        public ActionResult<string> reservation(long station_id, long employee_id, string request_status)
        {
            var station = _context.SwitchStations.FirstOrDefault(s => s.StationId == station_id);
            if (station == null)
                return NotFound("Station not found.");


            if (request_status == "待接单")
            {
                return NotFound("Request_status error.");
            }
            var employee = _context.Employees.FirstOrDefault(s => s.EmployeeId == employee_id);
            if (employee == null)
                return NotFound("Employee not found.");

            RequestStatusEnum Ordertype = RequestStatusEnum.未知;
            if (Enum.TryParse(request_status, out RequestStatusEnum typeEnum))
                Ordertype = typeEnum;
            else
                return NotFound("Order_type error.");
            var query = _context.SwitchRequests
                .Where(a => a.switchStation.StationId == station_id &&
                a.SwitchType == (int)SwitchTypeEnum.预约换电 &&
                a.RequestStatus == (int)Ordertype &&
                a.employee.EmployeeId == employee_id
                )
                .Select(switch_request => new
                {
                    switch_request.employee.EmployeeId,
                    switch_request_id = switch_request.SwitchRequestId,
                    plate_number = switch_request.vehicle.PlateNumber,
                    vehicle_model = switch_request.vehicle.vehicleParam.ModelName,
                    position = switch_request.Position,
                    username = switch_request.vehicleOwner.Username,
                    phone_number = switch_request.vehicleOwner.PhoneNumber,
                    request_time = switch_request.RequestTime,
                    order_status = switch_request.requestStatusEnum.ToString()
                }).ToList();

            if (query.Count == 0)
            {
                return NotFound("无订单！");
            }

            var a = new
            {
                code = 0,
                msg = "success",
                switch_request_array = query
            };
            return Content(JsonConvert.SerializeObject(a), "application/json");
        }

        [HttpPost]
        public ActionResult<string> receive([FromBody] dynamic _body)
        {
            using (TransactionScope tx = new TransactionScope())
            {

                dynamic body = JsonConvert.DeserializeObject<dynamic>(_body.ToString());
                long employee_id = Convert.ToInt64(body.employee_id);
                var employee = _context.Employees.FirstOrDefault(s => s.EmployeeId == employee_id);
                if (employee == null)
                    return NotFound("Employee not found.");
                long request_id = Convert.ToInt64(body.switch_request_id);
                var request = _context.SwitchRequests.FirstOrDefault(s => s.SwitchRequestId == request_id);
                if (request == null)
                    return NotFound("Switch request not found.");

                if(request.RequestStatus != (int)RequestStatusEnum.待接单)
                    return BadRequest("订单状态不是待接单，无法接单！");
                request.employee = employee;
                request.requestStatusEnum = RequestStatusEnum.待完成;

                string? barry_type = request.batteryType.Name;
                var battery = _context.Batteries.FirstOrDefault(s => s.batteryType.Name == barry_type);
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
                return Ok("已接单" + request.SwitchRequestId.ToString());
            }
        }

        [HttpPost]
        public ActionResult<string> submit([FromBody] dynamic _body)
        {
            using (TransactionScope tx = new TransactionScope())
            {
                dynamic body = JsonConvert.DeserializeObject<dynamic>(_body.ToString());
                long request_id = Convert.ToInt64(body.switch_request_id);
                var request = _context.SwitchRequests.FirstOrDefault(s => s.SwitchRequestId == request_id);
                if (request == null)
                    return NotFound("Switch request not found.");
                if (request.RequestStatus != (int)RequestStatusEnum.待完成)
                    return BadRequest("订单状态不是待接单，无法接单！");

                var batteryOff = request.vehicle.Battery;
                if (batteryOff == null)
                    return NotFound("Vehicle has no battery.");

                var station = request.switchStation;
                var battery_type = request.batteryType;
                var batteryOn = _context.Batteries.FirstOrDefault(s =>
                s.switchStation == station &&
                s.batteryType == battery_type &&
                s.AvailableStatus == (int)AvailableStatusEnum.已预定);
                if (batteryOn == null)
                    return NotFound("Staion has no battery.");

                batteryOn.AvailableStatusEnum = AvailableStatusEnum.汽车使用中;
                batteryOff.AvailableStatusEnum = AvailableStatusEnum.充电中;

                request.vehicle.Battery = batteryOn;
                request.requestStatusEnum = RequestStatusEnum.待评分;

                SwitchLog log = new SwitchLog()
                {
                    SwitchTime = DateTime.Now,
                    batteryOn = batteryOn,
                    batteryOff = batteryOff,
                    vehicle = request.vehicle,
                    employee = request.employee,
                    switchrequest = request,
                    Score = -1,
                    Evaluation = ""
                };

                _context.SwitchLogs.Add(log);

                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    return Conflict("数据库修改失败");
                }

                var obj = new
                {
                    code = 0,
                    msg = "success",
                    switch_log = new
                    {
                        switch_service_id = log.SwitchServiceId,
                        switch_time = log.SwitchTime,
                        batteryOnId = log.batteryOn.BatteryId,
                        batteryOffId = log.batteryOff.BatteryId,
                        vehicle_id = log.vehicle.VehicleId,
                        employee_id = log.employee.EmployeeId,
                        switch_request_id = log.switchrequest.SwitchRequestId,
                        score = log.Score,
                        evaluation = log.Evaluation
                    }
                };
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }
        }
    }
}
