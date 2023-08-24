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

namespace webapi.Controllers.Administrator
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

        [HttpGet]
        public ActionResult<IEnumerable<SwitchStation>> StationAround(double? longtitude = null, double? latitude = null, int info_num = 0, int info_index = 0)
        {
            var stations = _context.SwitchStations.AsQueryable();
            int offset = (info_index - 1) * info_num;
            int limit = info_num;
            if (longtitude == null && latitude == null || offset < 0 || limit <= 0)
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
                            station.StationId,
                            station.StationName,
                            station.Latitude,
                            station.Longtitude,
                            station.QueueLength,
                            Distance = Calculator.CalculateDistanceInMeters(station.Latitude, station.Longtitude, latitude.Value, longtitude.Value),
                            station.BatteryCapacity,
                            station.AvailableBatteryCount,
                        })
                        .OrderBy(station => station.Distance)
                        .Where(station => station.Distance <= 5000)
                        .Skip(offset)
                        .Take(limit)
                        .ToArray();
            var obj = new
            {
                data = result,
                code = 0
            };
            return Content(JsonConvert.SerializeObject(obj), "application/json");
        }

        [HttpGet("detailed-infos")]
        public ActionResult<IEnumerable<SwitchStation>> StationDetailed(string station_id = "", double? longtitude = null, double? latitude = null)
        {
            if (!long.TryParse(station_id, out long id) || longtitude == null || latitude == null)
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
                        Batteries = item.batteries.Select(battery => new
                        {
                            BatteryId = battery.BatteryId.ToString(),
                            AvailableStatus = battery.AvailableStatus == 1 ? "可用" : "充电中",
                            CurrentCapa = battery.CurrentCapacity,
                            BatteryType = battery.batteryType.BatteryTypeId % 10 == 1 ? "长续航级" : "标准续航级",
                        }).ToList(),
                        item.ServiceFee,
                        item.ParkingFee,
                        item.ElectricityFee,
                        item.Latitude,
                        item.Longtitude,
                        item.TimeSpan,
                        item.StationName,
                        Distance = Calculator.CalculateDistanceInMeters(item.Latitude, item.Longtitude, latitude.Value, longtitude.Value),
                    })
                    .ToArray();
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
        [HttpGet("name-fitting")]
        public ActionResult<IEnumerable<SwitchStation>> StationName(string keyword = "")
        {
            try
            {

                var filteredItems = _context.SwitchStations
                    .Select(item => new
                    {
                        ID = item.StationId.ToString(),
                        item.StationName,
                        item.Latitude,
                        item.Longtitude,
                        item.QueueLength,
                        item.TimeSpan,
                        item.ServiceFee,
                        item.ElectricityFee,
                        item.ParkingFee,
                        Similarity = Calculator.ComputeSimilarityScore(item.StationName, keyword)
                    })
                    .Where(item => item.Similarity >= 0.8)
                    .OrderByDescending(item => item.Similarity)
                    .Take(25)
                    .ToArray();
                var obj = new
                {
                    switch_stationArray = filteredItems,
                };
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }
            catch (Exception ex)
            {
                var obj = new
                {
                    switch_stationArray = "",
                };
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }
        }
    }

}
public class Calculator
{
    private const double EarthRadiusMeters = 6371000.0; // 地球半径 米

    public static double CalculateDistanceInMeters(double lat1, double lon1, double lat2, double lon2)
    {
        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusMeters * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
    public static double ComputeSimilarityScore(string s, string keyword)
    {
        int[,] dp = new int[s.Length + 1, keyword.Length + 1];

        for (int i = 0; i <= s.Length; i++)
        {
            for (int j = 0; j <= keyword.Length; j++)
            {
                if (i == 0)
                {
                    dp[i, j] = j;
                }
                else if (j == 0)
                {
                    dp[i, j] = i;
                }
                else if (s[i - 1] == keyword[j - 1])
                {
                    dp[i, j] = dp[i - 1, j - 1];
                }
                else
                {
                    dp[i, j] = 1 + Math.Min(Math.Min(dp[i - 1, j], dp[i, j - 1]), dp[i - 1, j - 1]);
                }
            }
        }

        // 关键词相似性分数，归一化
        double similarity = 1.0 - (double)dp[s.Length, keyword.Length] / keyword.Length;

        return similarity;
    }
}
