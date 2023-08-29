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

namespace webapi.Controllers.Administrator
{
    [Route("/owner/[action]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly ModelContext _context;

        public ReservationController(ModelContext context)
        {
            _context = context;
        }

        [HttpPost]
        public ActionResult<string> switch_reservation([FromBody] dynamic _reservation)
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

            SwitchRequest switchRequest = new SwitchRequest()
            {
                vehicleOwner = owner,
                RequestTime = DateTime.Now,
                vehicle = vehicle,
                Position = reservation.owner_address,
                Longitude = Convert.ToDouble(reservation.longitude),
                Latitude = Convert.ToDouble(reservation.latitude),
                Note = reservation.additional_info,
                Date = Convert.ToDateTime(reservation.date),
                Period = reservation.period,
                switchStation = station
            };
            //换电方式->枚举类
            if (Enum.TryParse(reservation.replace_type, out SwitchTypeEnum typeEnum))
                switchRequest.SwitchTypeEnum = typeEnum;
            else
                return NotFound("Replace_type error.");
            //根据换电方式设置请求状态
            switch (switchRequest.SwitchTypeEnum)
            {
                case SwitchTypeEnum.上门换电:
                    switchRequest.requestStatusEnum = RequestStatusEnum.待接单;
                    break;
                case SwitchTypeEnum.预约换电:
                    switchRequest.requestStatusEnum = RequestStatusEnum.待完成;
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
            return Content(JsonConvert.SerializeObject(obj), "application/json");
        }

        [HttpGet]
        public ActionResult<string> switch_history(int pageIndex, int pageSize, string owner_id, string order_type)
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
            if (Enum.TryParse(order_type, out RequestStatusEnum typeEnum))
                Ordertype = typeEnum;
            else
                return NotFound("Order_type error.");

            var query = _context.SwitchRequests
            .Where(
                sr => sr.vehicleOwner.OwnerId == long.Parse(owner_id) &&
                sr.SwitchType == (int)Ordertype
            )
            .Join(
                _context.SwitchLogs,
                sr => sr.SwitchRequestId,
                sl => sl.switchrequest.SwitchRequestId,
                (sr, sl) => new
                {
                    sr.SwitchRequestId,
                    sr.SwitchType,
                    sr.RequestTime,
                    sr.Position,
                    sr.switchStation.StationName,
                    sr.vehicle.PlateNumber,
                    sr.vehicle.vehicleParam.ModelName,
                    sr.batteryType.Name,
                    sl.SwitchTime,
                    sl.Score,
                    sl.Evaluation
                })
            .Skip(offset)
            .Take(limit);

            var totalData = query.Count();
            var data = query.ToList();

            if (data == null)
                return BadRequest();
            var obj = new
            {
                totalData,
                data
            };

            return Content(JsonConvert.SerializeObject(obj), "application/json");
        }

        [HttpPost]
        public ActionResult<string> review([FromBody] dynamic _review)
        {
            using (TransactionScope tx = new TransactionScope())
            {
                dynamic review = JsonConvert.DeserializeObject<dynamic>(_review.ToString());

                long switchRequestId = Convert.ToInt64(review.switch_request_id);
                var switchLog = _context.SwitchLogs.FirstOrDefault(s => s.switchrequest.SwitchRequestId == switchRequestId);
                switchLog.Score = (double)review.score;
                switchLog.Evaluation = review.evaluation;

                var switchRequest = _context.SwitchRequests.FirstOrDefault(s => s.SwitchRequestId == switchRequestId);
                switchRequest.requestStatusEnum = RequestStatusEnum.已完成;

                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }

                var query = _context.SwitchRequests
                .Where(sr => sr.SwitchRequestId == switchRequestId)
                .Join(
                    _context.SwitchLogs,
                    sr => sr.SwitchRequestId,
                    sl => sl.switchrequest.SwitchRequestId,
                    (sr, sl) => new
                    {
                        SwitchRequest = sr,
                        sl.SwitchTime,
                        sl.Score,
                        sl.Evaluation
                    })
                .FirstOrDefault();

                var obj = new
                {
                    switch_request = query,
                };
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }
        }

        //接口已废弃
        //[HttpPatch("{id}")]
        //public ActionResult<string> Patch_SwitchReservation(int reservation_id, [FromBody] dynamic _reservation)
        //{

        //    var request = _context.SwitchRequests.FirstOrDefault(s => s.SwitchRequestId == reservation_id);
        //    if (request == null)
        //        return NotFound("Reservation not found.");

        //    dynamic reservation = JsonConvert.DeserializeObject<dynamic>(_reservation.ToString());

        //    request.RequestTime = reservation.updatetime;
        //    request.Date = reservation.reservation_date;
        //    request.Period = reservation.period;
        //    request.Position = reservation.owner_address;
        //    request.Longitude = reservation.longitude;
        //    request.Latitude = reservation.latitude;

        //    try
        //    {
        //        _context.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        return NotFound();
        //    }

        //    return NoContent();
        //}
    }
}
