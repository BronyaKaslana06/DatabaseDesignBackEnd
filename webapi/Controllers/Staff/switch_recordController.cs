using EntityFramework.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace webapi.Controllers.Staff
{
    [Route("staff/[controller]/[action]")]
    [ApiController]
    public class switch_recordController : ControllerBase
    {
        private readonly ModelContext modelContext;

        public switch_recordController(ModelContext context)
        {
            modelContext = context;
        }

        [Authorize]
        [HttpGet]
        public ActionResult<object> itemDetail(string switch_record_id)
        {
            try
            {
                var tmp = modelContext.SwitchLogs.Include(f => f.switchrequest).
                    Include(b => b.batteryOn).Include(c => c.batteryOff).
                    Include(b=>b.batteryOn.batteryType).Include(b => b.batteryOff.batteryType).Single(e => e.SwitchServiceId == long.Parse(switch_record_id));
                var switch_request = tmp.switchrequest;         
                var battery_on = tmp.batteryOn;
                var batteryOff = tmp.batteryOff;
                var vehicle = modelContext.SwitchRequests.Include(f => f.vehicle).Single(e => e.SwitchRequestId == switch_request.SwitchRequestId).vehicle;
                var vehicle_param = modelContext.VehicleParams.Include(e => e.vehicles).Single(f => f.vehicles.Any(g => g.VehicleId == vehicle.VehicleId));
                var owner = modelContext.Vehicles.Include(e => e.vehicleOwner).Single(f => f.VehicleId == vehicle.VehicleId).vehicleOwner;
                var employee = modelContext.SwitchRequests.Include(h => h.employee.switchStation).Single(i => i.SwitchRequestId == switch_request.SwitchRequestId).employee;

                var res = new
                {
                    username = owner.Username == null ? "" : owner.Username,
                    phone_number = owner.PhoneNumber,
                    vehicle_model = vehicle_param.ModelName,
                    plate_number = vehicle.PlateNumber == null ? "" : vehicle.PlateNumber,
                    switch_time = tmp.SwitchTime.ToString(),
                    battery_id_on = battery_on.BatteryId.ToString(),
                    battery_id_off = batteryOff.BatteryId.ToString(),
                    battery_type_on = battery_on.batteryType.Name,
                    battery_type_off = batteryOff.batteryType.Name,
                    station_name = employee.switchStation.StationName,
                    switch_type = switch_request.SwitchTypeEnum.ToString(),
                    evaluations = tmp.Evaluation == null ? "" : tmp.Evaluation,
                    score = tmp.Score,
                    request_status = switch_request.requestStatusEnum.ToString(),
                    switch_request_id=switch_request.SwitchRequestId
                };
                return Ok(res);
            }
            catch (Exception ex)
            {
                return NotFound("id error:" + ex.Message);
            }
        }
    }
}
