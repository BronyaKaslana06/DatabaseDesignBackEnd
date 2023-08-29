using EntityFramework.Context;
using EntityFramework.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
        public ActionResult<IEnumerable<Employee>> detail(long switch_request_id)
        {
            var request = _context.SwitchRequests.FirstOrDefault(s => s.SwitchRequestId == switch_request_id);
            if (request == null)
                return NotFound("Request not found.");

            var query = _context.SwitchRequests
                .Where(a => a.SwitchRequestId == switch_request_id)
                .Join(
                _context.SwitchLogs,
                sr => sr.SwitchRequestId,
                sl => sl.switchrequest.SwitchRequestId,
                (sr, sl) => new
                {
                    switch_request_id = sr.SwitchRequestId,
                    sr.SwitchType,
                    order_status = sr.RequestStatus,
                    request_time = sr.RequestTime,
                    position = sr.Position,
                    longtitude = sr.Longitude,
                    latitude = sr.Latitude,
                    username = sr.vehicleOwner.Username,
                    sr.vehicleOwner.PhoneNumber,
                    plate_number = sr.vehicle.PlateNumber,
                    vehicle_model = sr.vehicle.vehicleParam.ModelName,
                    switch_time = sl.SwitchTime,
                    remarks = sr.Note,
                    sl.Score,
                    sl.Evaluation,
                    barryOnId = sl.batteryOn.BatteryId,
                    barryOffId = sl.batteryOff.BatteryId
                });

            if (query.Count() == 0)
            {
                return NotFound("无该请求！");
            }

            var a = new
            {
                code = 200,
                msg = "success",
                data = query.ToList()[0]
            };
            return Content(JsonConvert.SerializeObject(a), "application/json");
        }

        [HttpGet]
        public ActionResult<IEnumerable<Employee>> doortodoor(long station_id, long employee_id, string request_status)
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
                code = 200,
                msg = "success",
                switch_request_array = query
            };
            return Content(JsonConvert.SerializeObject(a), "application/json");
        }

        [HttpGet]
        public ActionResult<IEnumerable<Employee>> reservation(long station_id, long employee_id, string request_status)
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
                code = 200,
                msg = "success",
                switch_request_array = query
            };
            return Content(JsonConvert.SerializeObject(a), "application/json");
        }
    }
}
