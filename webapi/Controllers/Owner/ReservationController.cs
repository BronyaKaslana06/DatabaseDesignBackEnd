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

namespace webapi.Controllers.Administrator
{
    [Route("/owner/switch-reservation")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly ModelContext _context;

        public ReservationController(ModelContext context)
        {
            _context = context;
        }

        [HttpPost]
        public ActionResult<string> Post_SwitchReservation([FromBody] dynamic _reservation)
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
            //���緽ʽ->ö����
            if (Enum.TryParse(reservation.replace_type, out SwitchTypeEnum typeEnum))
                switchRequest.SwitchTypeEnum = typeEnum;
            else
                return NotFound("Replace_type error.");
            //���ݻ��緽ʽ��������״̬
            switch (switchRequest.SwitchTypeEnum)
            {
                case SwitchTypeEnum.���Ż���:
                    switchRequest.requestStatusEnum = RequestStatusEnum.���ӵ�;
                    break;
                case SwitchTypeEnum.ԤԼ����:
                    switchRequest.requestStatusEnum = RequestStatusEnum.�����;
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

        //�ӿ��ѷ���
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