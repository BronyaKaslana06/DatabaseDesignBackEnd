﻿using EntityFramework.Context;
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
        public ActionResult<string> detail(string switch_request_id)
        {
            long request_id = Convert.ToInt64(switch_request_id);
            Console.WriteLine(request_id);
            var request = _context.SwitchRequests.FirstOrDefault(s => s.SwitchRequestId == request_id);
            if (request == null)
                return NotFound("Request not found.");
            Console.WriteLine(request_id);
            var log = from r in _context.SwitchLogs
                      where r.switchrequest == request
                      select new
                      {
                          score = r.Score,
                          evaluation = r.Evaluation,
                          batteryOnId = r.batteryOn.BatteryId,
                          batteryOffId = r.batteryOff.BatteryId,
                          switch_time = r.SwitchTime
                      };
            long ss = request_id;
            Console.WriteLine(request_id);
            var query = from sr in _context.SwitchRequests
                        where sr.SwitchRequestId == request_id
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
                            order_status = sr.requestStatusEnum.ToString(),
                            battery_type_id = sr.batteryType.Name
                        };

            if (query == null || query.Count() == 0)
                return NotFound("无该请求！");

            var data = new
            {
                switch_request = query.ToList()[0],
                switch_log = (log == null || log.Count() == 0) ? new
                {
                    score = (double)-1,
                    evaluation = "",
                    batteryOnId = (long)-1,
                    batteryOffId = (long)-1,
                    switch_time = DateTime.MaxValue
                } : log.DefaultIfEmpty().ToList()[0]
            };

            var a = new
            {
                code = 0,
                msg = "success",
                data
            };
            return Content(JsonConvert.SerializeObject(a), "application/json");
        }

        [HttpGet]
        public ActionResult<string> doortodoor(string employee_id, string request_status)
        {
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
                .Where(a => a.SwitchType == (int)SwitchTypeEnum.上门换电 &&
                a.RequestStatus == (int)Ordertype &&
                (Convert.ToInt64(employee_id) > 0 ? a.employee.EmployeeId == Convert.ToInt64(employee_id) : true)
                )
                .OrderByDescending(a=>a.RequestTime)
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
                    order_status = switch_request.requestStatusEnum.ToString(),
                    battery_type_id = switch_request.batteryType.Name
                }).ToList();

            var a = new
            {
                code = 0,
                msg = "success",
                switch_request_array = query
            };
            return Content(JsonConvert.SerializeObject(a), "application/json");
        }

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
                .Where(a => a.switchStation.StationId == Convert.ToInt64(station_id) &&
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
                    username = switch_request.vehicleOwner.Username,
                    phone_number = switch_request.vehicleOwner.PhoneNumber,
                    request_time = switch_request.RequestTime,
                    order_status = switch_request.requestStatusEnum.ToString(),
                    battery_type_id = switch_request.batteryType.Name
                }).ToList();

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

        [HttpPost]
        public ActionResult<string> submit([FromBody] dynamic _body)
        {
            using (TransactionScope tx = new TransactionScope())
            {
                dynamic body = JsonConvert.DeserializeObject<dynamic>(_body.ToString());
                long request_id = Convert.ToInt64(body.switch_request_id);
                var request = _context.SwitchRequests
                    .Include(a => a.batteryType)
                    .Include(b => b.switchStation)
                    .Include(c => c.vehicle)
                    .Include(d => d.employee)
                    .Include(e => e.vehicleOwner)
                    .Include(f => f.vehicle.Battery)
                    .FirstOrDefault(s => s.SwitchRequestId == request_id);
                if (request == null)
                    return NewContent(1, "Switch request not found.");
                if (request.RequestStatus != (int)RequestStatusEnum.待完成)
                    return NewContent(1, "订单状态不是待完成，无法接单！");

                var batteryOff = request.vehicle.Battery;
                if (batteryOff == null)
                    return NewContent(1, "Vehicle has no battery.");

                var station = request.switchStation;
                var battery_type = request.batteryType;
                var batteryOn = _context.Batteries.Where(s =>
                s.switchStation == station &&
                s.batteryType == battery_type &&
                s.AvailableStatus == (int)AvailableStatusEnum.已预定).DefaultIfEmpty().FirstOrDefault();
                if (batteryOn == null)
                    return NewContent(1, "Staion has no battery.");

                batteryOn.AvailableStatusEnum = AvailableStatusEnum.汽车使用中;
                batteryOff.AvailableStatusEnum = AvailableStatusEnum.充电中;

                request.vehicle.Battery = batteryOn;
                request.requestStatusEnum = RequestStatusEnum.待评价;

                long id = _context.SwitchLogs.Max(e => e.SwitchServiceId) + 1;
                SwitchLog log = new SwitchLog()
                {
                    SwitchServiceId = id,
                    SwitchTime = DateTime.Now,
                    batteryOn = batteryOn,
                    batteryOff = batteryOff,
                    vehicle = request.vehicle,
                    employee = request.employee,
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
