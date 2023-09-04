using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.ContentModel;
using NuGet.Protocol;
using System.Data;
using System.Xml.Linq;
using EntityFramework.Context;
using EntityFramework.Models;
using System.Transactions;

namespace webapi.Controllers.Owner
{
    [Route("/owner/dashboard")]
    [ApiController]
    public class OwnerBoardController : ControllerBase
    {
        private readonly ModelContext _context;

        public OwnerBoardController(ModelContext context)
        {
            _context = context;
        }

        [HttpGet("min_capacity")]
        public ActionResult<IEnumerable<Vehicle>> current_quantity(string user_id = "")
        {
            bool flag = long.TryParse(user_id, out var id);
            if (!flag)
            {
                var obj = new
                {
                    code = 1,
                    msg = "id非法",
                    totalData = 0,
                    data = "",
                };
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }
            var owner = _context.VehicleOwners.Find(id);
            if (owner == null)
                return NewContent(1, "id不存在");
            else
            {
                var filteredItem = _context.Vehicles
                    .Where(item => item.vehicleOwner == owner)
                    .OrderByDescending(item => item.Battery.CurrentCapacity)
                    .Select(item => new
                    {
                        plate_number = item.PlateNumber,
                        current_capacity = item.Battery.CurrentCapacity
                    }).FirstOrDefault();
                var a = new
                {
                    code = 0,
                    msg = "success",
                    totaldata = 1,
                    data = filteredItem
                };
                return Content(JsonConvert.SerializeObject(a), "application/json");
            }
        }
        [HttpGet("base_info")]
        public ActionResult<IEnumerable<VehicleOwner>> InfoAna(string user_id)
        {
            if (!long.TryParse(user_id, out long id))
            {
                var error = new
                {
                    code = 1,
                    msg = "id无效",
                    data = ""
                };
                return Content(JsonConvert.SerializeObject(error), "application/json");
            }

            var owner = _context.VehicleOwners.Find(id);
            if (owner == null)
                return NewContent(1, "id不存在");
            else
            {
                var a = new
                {
                    code = 0,
                    msg = "success",
                    data = new
                    {
                        accountSerial = owner.AccountSerial,
                        username = owner.Username,
                        email = owner.Email == null ? "未绑定" : owner.Email,
                        createTime = owner.CreateTime.ToString("yyyy-MM-dd"),
                        phoneNumber = owner.PhoneNumber,
                        birthday = owner.Birthday == null ? "未绑定" : owner.Birthday.Value.ToString("yyyy-MM-dd"),
                        dealCount = _context.SwitchLogs.Where(sl => sl.switchrequest.vehicle.vehicleOwner.OwnerId == id).Count(),
                        mileage = _context.Vehicles.Where(v => v.vehicleOwner.OwnerId == id).Sum(v => v.Mileage),
                        unscored_maintenance = _context.MaintenanceItems.Where(m => m.OrderStatus == 3).Count(),
                        unscored_switchlog = _context.SwitchLogs.Where(sl => sl.Score == -1).Count()
                    }
                };
                return Content(JsonConvert.SerializeObject(a), "application/json");
            }
        }
        [HttpGet("monthlyswitch")]
        public ActionResult<IEnumerable<VehicleOwner>> MonthlyAna(string user_id)
        {
            if (!long.TryParse(user_id, out long id))
            {
                var error = new
                {
                    code = 1,
                    msg = "id无效",
                    data = ""
                };
                return Content(JsonConvert.SerializeObject(error), "application/json");
            }

            var owner = _context.VehicleOwners.Find(id);
            if (owner == null)
                return NewContent(1, "id不存在");
            else
            {
                var currentYear = DateTime.Now.Year;
                var allMonths = Enumerable.Range(1, 12).ToArray();
                var monthlySwitchCounts = _context.SwitchLogs
                    .Where(log => log.SwitchTime.Year == currentYear && log.switchrequest.vehicle.vehicleOwner.OwnerId == id)
                    .GroupBy(log => log.SwitchTime.Month)
                    .Select(group => new { Month = group.Key, Count = group.Sum(log => log.ServiceFee) }) 
                    .OrderBy(result => result.Month)
                    .ToArray();
                var monthlySwitchCountsArray = allMonths
                    .Select(month => monthlySwitchCounts.FirstOrDefault(item => item.Month == month)?.Count ?? 0)
                    .ToArray();

                var a = new
                {
                    code = 0,
                    msg = "success",
                    data = monthlySwitchCountsArray
                };
                return Content(JsonConvert.SerializeObject(a), "application/json");
            }
        }

        ContentResult NewContent(int _code = 0, string _msg = "success", string dataName = "data", object data = null)
        {

            var a = new Dictionary<string, object>();
            a.Add("code", _code);
            a.Add("msg", _msg);
            if (data != null)
                a.Add(dataName, data);

            return Content(JsonConvert.SerializeObject(a), "application/json");
        }
    }
}
