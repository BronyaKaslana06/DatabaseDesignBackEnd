using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using EntityFramework.Context;
using EntityFramework.Models;
using Idcreator;
using System.Transactions;
using System.Globalization;
using static System.Collections.Specialized.BitVector32;
using System.Drawing.Printing;
using webapi.Tools;
using System.Security.Cryptography;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace webapi.Controllers.Owner
{
    [Route("/owner")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly ModelContext _context;

        public ReservationController(ModelContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost("switch_request")]
        public ActionResult<string> switch_request([FromBody] dynamic _reservation)
        {
            using (TransactionScope tx = new TransactionScope())
            {
                if (_context.Employees == null)
                    return Problem("Entity set 'ModelContext.Employee' is null.");

                dynamic reservation = JsonConvert.DeserializeObject<dynamic>(_reservation.ToString());

                long owner_id = Convert.ToInt64(reservation.owner_id);
                var owner = _context.VehicleOwners.FirstOrDefault(s => s.OwnerId == owner_id);
                if (owner == null)
                    return NotFound("Owner not found.");

                long vehicle_id = Convert.ToInt64(reservation.vehicle_id);
                var vehicle = _context.Vehicles.FirstOrDefault(s => s.VehicleId == vehicle_id);
                if (vehicle == null)
                    return NotFound("Vehicle not found.");

                long station_id = Convert.ToInt64(reservation.station_id);
                var station = _context.SwitchStations.FirstOrDefault(s => s.StationId == station_id);
                if (station == null)
                    return NotFound("Station not found.");

                string battery_type = reservation.battery_type;
                var batteryType = _context.BatteryTypes.FirstOrDefault(s => s.Name == battery_type);
                if (batteryType == null)
                    return NotFound("Battery_type not found.");

                var employee = _context.Employees.FirstOrDefault(e => e.switchStation == station);

                long id = _context.SwitchRequests.Max(e => e.SwitchRequestId) + 1;
                SwitchRequest switchRequest = new SwitchRequest()
                {
                    SwitchRequestId = id,
                    RequestTime = DateTime.Now,
                    vehicle = vehicle,
                    Position = reservation.owner_address,
                    Longitude = Convert.ToDouble(reservation.longitude),
                    Latitude = Convert.ToDouble(reservation.latitude),
                    Note = reservation.additional_info,
                    Date = Convert.ToDateTime(reservation.date),
                    PeriodEnum = EnumDisplay.GetEnumFromDisplayName(Convert.ToString(reservation.period)),
                    batteryType = batteryType,
                    employee = employee
                };
                //换电方式->枚举类
                if (Enum.TryParse(reservation.replace_type.ToString(), out SwitchTypeEnum typeEnum))
                    switchRequest.SwitchTypeEnum = typeEnum;
                else
                    return NotFound("Replace_type error.");
                //根据换电方式设置请求状态
                switch (switchRequest.SwitchTypeEnum)
                {
                    case SwitchTypeEnum.上门换电:
                        switchRequest.requestStatusEnum = RequestStatusEnum.待接单;
                        break;
                    case SwitchTypeEnum.到店换电:
                        switchRequest.requestStatusEnum = RequestStatusEnum.待完成;
                        //预约换电可以直接预定电池
                        var battery = _context.Batteries.FirstOrDefault(s => s.batteryType.Name == battery_type && s.switchStation == station);
                        if (battery == null)
                            return NotFound("Battery not found.");
                        battery.AvailableStatusEnum = AvailableStatusEnum.已预定;

                        break;
                    default:
                        return NotFound("Replace_type error.");
                }

                _context.SwitchRequests.Add(switchRequest);
                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    return Conflict();
                }

                var obj = new
                {
                    reservation_id = switchRequest.SwitchRequestId,
                };
                tx.Complete();
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }
        }

        [Authorize]
        [HttpGet("switch_history")]
        public ActionResult<string> switch_history(int pageIndex, int pageSize, string owner_id, string order_status = "已完成")
        {
            int offset = (pageIndex - 1) * pageSize;
            int limit = pageSize;
            if (offset < 0 || limit <= 0)
                return BadRequest();
            if (!string.IsNullOrEmpty(owner_id) && !long.TryParse(owner_id, out long OID))
            {
                return BadRequest();
            }

            RequestStatusEnum Ordertype = RequestStatusEnum.未知;
            if (Enum.TryParse(order_status, out RequestStatusEnum typeEnum))
                Ordertype = typeEnum;
            else
                return NotFound("Order_type error.");

            var q = (
                from sr in _context.SwitchRequests
                where sr.vehicle.vehicleOwner.OwnerId == long.Parse(owner_id) && sr.RequestStatus == (int)Ordertype
                orderby sr.RequestTime descending
                select new
                {
                    switch_request_id = sr.SwitchRequestId,
                    switch_type = sr.SwitchTypeEnum.ToString(),
                    request_time = sr.RequestTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    position = sr.Position,
                    station_name = sr.employee.switchStation.StationName,
                    station_address = sr.employee.switchStation.Address,
                    licence_plate = sr.vehicle.PlateNumber,
                    vehicle_model = sr.vehicle.vehicleParam.ModelName,
                    battery_type = sr.batteryType.Name,
                    employee_id = (order_status == "待接单" ? "" : sr.employee.EmployeeId.ToString()),
                    switch_date = sr.Date.ToString("yyyy-MM-dd"),
                    switch_period = EnumDisplay.GetDisplayNameFromEnum(sr.PeriodEnum),
                    order_type = sr.requestStatusEnum.ToString()
                });

            var totalData = q.Count();
            var query = q.Skip(offset).Take(limit);
            
            var data = query.ToList();

            if (data == null)
                return BadRequest();
            var obj = new
            {
                code = 0,
                msg = "success",
                data
            };

            return Content(JsonConvert.SerializeObject(obj), "application/json");
        }

        [Authorize]
        [HttpPost("review")]
        public ActionResult<string> review([FromBody] dynamic _review)
        {
            using (TransactionScope tx = new TransactionScope())
            {
                dynamic review = JsonConvert.DeserializeObject<dynamic>(_review.ToString());
                Console.WriteLine("######");
                Console.Write(review.evaluation == null);
                Console.WriteLine("######");
                long switchRequestId = review.switch_request_id;

                var switchRequest = _context.SwitchRequests.Where(s => s.SwitchRequestId == switchRequestId).FirstOrDefault();
                if (switchRequest == null)
                    return NotFound("Switch request is not exist.");
                if (switchRequest.requestStatusEnum != RequestStatusEnum.待评价)
                    return NotFound("订单状态非‘待评价’，无法评价");
                switchRequest.requestStatusEnum = RequestStatusEnum.已完成;

                var switchLog = _context.SwitchLogs
                    .Include(a => a.batteryOn)
                    .Include(b => b.batteryOff)
                    .Include(c => c.switchrequest)
                    .Where(s => s.switchRequestId == switchRequestId).DefaultIfEmpty().FirstOrDefault();
                if (switchLog == null)
                    return NotFound("订单状态为‘待评价’，但无完成记录");
                switchLog.Score = (double)review.score;
                switchLog.Evaluation = review.evaluation == null ? "默认好评" : review.evaluation;

                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }

                var query = (from sl in _context.SwitchLogs
                             where sl.switchRequestId == switchRequestId
                             select new
                             {
                                 sl.SwitchServiceId,
                                 sl.SwitchTime,
                                 sl.switchRequestId,
                                 batteryOnId = sl.batteryOn.BatteryId,
                                 batteryOffId = sl.batteryOff.BatteryId,
                                 sl.Score,
                                 sl.Evaluation
                             }).FirstOrDefault();

                var obj = new
                {
                    switch_request = query,
                };
                tx.Complete();
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }
        }

        [Authorize]
        [HttpDelete("switch_history")]
        public ActionResult<string> delete_request(string switch_request_id, string owner_id)
        {
            long requestid = Convert.ToInt64(switch_request_id);
            var request = _context.SwitchRequests
                .Include(a => a.vehicle.vehicleOwner)
                .FirstOrDefault(s => s.SwitchRequestId == requestid);
            if (request == null)
                return NotFound("Request not found.");

            long ownerid = Convert.ToInt64(owner_id);
            var owner = _context.VehicleOwners.FirstOrDefault(s => s.OwnerId == ownerid);
            if (owner == null)
                return NotFound("Owner not found.");
            if (request.vehicle.vehicleOwner != owner)
                return NotFound("非本人，无法撤销");

            if (request.RequestStatus != (int)RequestStatusEnum.待接单 && request.RequestStatus != (int)RequestStatusEnum.待完成)
            {
                return NotFound("已完成订单无法撤销");
            }

            _context.SwitchRequests.Remove(request);
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
                msg = "success"
            };
            return Content(JsonConvert.SerializeObject(obj), "application/json");
        }
    }
}
