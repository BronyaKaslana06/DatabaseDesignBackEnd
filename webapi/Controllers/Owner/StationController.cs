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
using Microsoft.AspNetCore.Authorization;

namespace webapi.Controllers.Owner
{
    [Route("owner/stations")]
    [ApiController]
    public class StationController : ControllerBase
    {
        private readonly ModelContext _context;

        public StationController(ModelContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<SwitchStation>> StationAround(double? longitude = null, double? latitude = null, int info_num = 0, int info_index = 0)
        {
            var stations = _context.SwitchStations.AsQueryable();
            int offset = (info_index - 1) * info_num;
            int limit = info_num;
            if (longitude == null && latitude == null || offset < 0 || limit <= 0)
            {
                var ob = new
                {
                    data = "",
                    code = 1
                };
                return Content(JsonConvert.SerializeObject(ob), "application/json");
            }
            var result = _context.SwitchStations
                        .Select(station => new
                        {
                            station_id = station.StationId.ToString(),
                            station_name = station.StationName,
                            latitude = station.Latitude,
                            longitude = station.Longitude,
                            waiting_number = station.QueueLength,
                            opening_time = station.TimeSpan,
                            distance = Math.Round(Calculator.CalculateDistanceInMeters(station.Latitude, station.Longitude, latitude.Value, longitude.Value)),
                            cell_num = station.BatteryCapacity,
                            cell_avb_num = station.AvailableBatteryCount,
                        })
                        .AsEnumerable()
                        .Where(ss => ss.distance <= 50000)
                        .OrderBy(station => station.distance)
                        .Skip(offset)
                        .Take(limit)
                        .ToList();
            var obj = new
            {
                data = result,
                code = 0
            };
            return Content(JsonConvert.SerializeObject(obj), "application/json");
        }

        [Authorize]
        [HttpGet("detailed-infos")]
        public ActionResult<IEnumerable<SwitchStation>> StationDetailed(string station_id = "", double? longitude = null, double? latitude = null)
        {
            if (!long.TryParse(station_id, out long id) || longitude == null || latitude == null)
            {
                var obj = new
                {
                    data = "",
                    code = 1,
                    msg = "站点id非法或经纬度非法",
                };
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }
            try
            {
                var filteredItems = _context.SwitchStations
                    .Where(item => item.StationId == id)
                    .OrderBy(item => item.StationId)
                    .Select(item => new
                    {
                        battery_array = item.batteries.Select(battery => new
                        {
                            battery_id = battery.BatteryId.ToString(),
                            available_status = battery.AvailableStatusEnum.ToString(),
                            current_capacity = battery.CurrentCapacity,
                            battery_type = battery.batteryType.BatteryTypeId == 1 ? "长续航级" : "标准续航级",
                        }).ToArray(),
                        service_fee = item.ServiceFee,
                        parking_fee = item.ParkingFee,
                        power_rate = item.ElectricityFee,
                        latitude = item.Latitude,
                        longitude = item.Longitude,
                        opening_time = item.TimeSpan,
                        station_name = item.StationName,
                        waiting_number = item.QueueLength,
                        address = item.Address,
                        distance = Calculator.CalculateDistanceInMeters(item.Latitude, item.Longitude, latitude.Value, longitude.Value),
                    })
                    .FirstOrDefault();
                var obj = new
                {
                    data = filteredItems,
                    code = 0,
                    msg = "success",
                };
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }
            catch (Exception ex)
            {
                var obj = new
                {
                    data = "",
                    code = 1,
                    msg = "fail",
                };
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }
        }

        [Authorize]
        [HttpGet("keyword")]
        public ActionResult<IEnumerable<SwitchStation>> StationName(string keyword = "", double? longitude = null, double? latitude = null, int info_num = 0, int info_index = 0)
        {
            try
            {
                var stations = _context.SwitchStations.AsQueryable();
                int offset = (info_index - 1) * info_num;
                int limit = info_num;
                if (longitude == null && latitude == null || offset < 0 || limit <= 0)
                {
                    var ob = new
                    {
                        data = "",
                        code = 1
                    };
                    return Content(JsonConvert.SerializeObject(ob), "application/json");
                }
                var query = _context.SwitchStations
                    .Select(item => new
                    {
                        station_id = item.StationId.ToString(),
                        station_name = item.StationName,
                        latitude = item.Latitude,
                        longitude = item.Longitude,
                        waiting_number = item.QueueLength,
                        opening_time = item.TimeSpan,
                        service_fee = item.ServiceFee,
                        power_rate = item.ElectricityFee,
                        parking_fee = item.ParkingFee,
                        distance = Math.Round(Calculator.CalculateDistanceInMeters(item.Latitude, item.Longitude, latitude.Value, longitude.Value)),
                        cell_num = item.BatteryCapacity,
                        cell_avb_num = item.AvailableBatteryCount,
                        Similarity = Calculator.ComputeSimilarityScore(item.StationName, keyword)
                    })
                    .ToList();

                var filteredItems = query
                    .Where(item => item.Similarity > (double)0)
                    .OrderByDescending(item => item.Similarity)
                    .Take(9)
                    .ToList();

                var obj = new
                {
                    data = filteredItems,
                    code = 0,
                    msg = "success"
                };
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }
            catch (Exception ex)
            {
                var obj = new
                {
                    data = "",
                    msg = ex.InnerException?.Message,
                    code = 1
                };
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }
        }
    }

}

