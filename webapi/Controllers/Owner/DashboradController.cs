//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Http.HttpResults;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.CodeAnalysis.Elfie.Diagnostics;
//using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;
//using NuGet.ContentModel;
//using NuGet.Protocol;
//using System.Data;
//using System.Xml.Linq;
//using EntityFramework.Context;
//using EntityFramework.Models;
//using System.Transactions;

//namespace webapi.Controllers.Owner
//{
//    [Route("/owner/dashboard")]
//    [ApiController]
//    public class OwnerBoardController : ControllerBase
//    {
//        private readonly ModelContext _context;

//        public OwnerBoardController(ModelContext context)
//        {
//            _context = context;
//        }

//        [HttpGet("info/{user_id}")]
//        public ActionResult<IEnumerable<VehicleOwner>> InfoAna(string user_id)
//        {
//            if (!long.TryParse(user_id, out long id))
//            {
//                var error = new
//                {
//                    code = 1,
//                    msg = "id无效",
//                    data = ""
//                };
//                return Content(JsonConvert.SerializeObject(error), "application/json");
//            }

//            var owner = _context.VehicleOwners.Find(id);
//            if (owner == null)
//                return NewContent(1, "id不存在");
//            else
//            {
//                var a = new
//                {
//                    code = 0,
//                    msg = "success",
//                    data = new
//                    {
//                        accountSerial = owner.AccountSerial,
//                        username = owner.Username,
//                        email = owner.Email == null ? "未绑定" : owner.Email,
//                        createTime = owner.CreateTime.ToString("yyyy-MM-dd"),
//                        phoneNumber = owner.PhoneNumber,
//                        birthday = owner.Birthday == null? "未绑定" : owner.Birthday.Value.ToString("yyyy-MM-dd"),
//                        dealCount = _context.SwitchLogs.Where(sl => sl.vehicle.vehicleOwner.OwnerId == id).Count(),
//                        mileage = _context.Vehicles.Where(v => v.vehicleOwner.OwnerId == id).Sum(v => v.Mileage),
//                        unscored_maintenance = _context.MaintenanceItems.Where(m => m.OrderStatus == 3).Count(),
//                        unscored_switchlog = _context.SwitchLogs.Where(sl => sl.Score == -1).Count()
//                    }
//                };
//                return Content(JsonConvert.SerializeObject(a), "application/json");
//            }
//        }
//        [HttpGet("monthlyswitch/{user_id}")]
//        public ActionResult<IEnumerable<VehicleOwner>> MonthlyAna(string user_id)
//        {
//            if (!long.TryParse(user_id, out long id))
//            {
//                var error = new
//                {
//                    code = 1,
//                    msg = "id无效",
//                    data = ""
//                };
//                return Content(JsonConvert.SerializeObject(error), "application/json");
//            }

//            var owner = _context.VehicleOwners.Find(id);
//            if (owner == null)
//                return NewContent(1, "id不存在");
//            else
//            {
//                var a = new
//                {
//                    code = 0,
//                    msg = "success",
//                    data = _context.SwitchLogs
//                        .Where(log => log.SwitchTime.Year == DateTime.Now.Year)
//                        .GroupBy(log => log.SwitchTime.Month)
//                        .Select(group => new { Month = group.Key, Count = group.Count() })
//                        .OrderBy(result => result.Month)
//                        .Select(result => result.Count)
//                        .ToArray()
//                };
//                return Content(JsonConvert.SerializeObject(a), "application/json");
//            }
//        }
//        ContentResult NewContent(int _code = 0, string _msg = "success", string dataName = "data", object data = null)
//        {

//            var a = new Dictionary<string, object>();
//            a.Add("code", _code);
//            a.Add("msg", _msg);
//            if (data != null)
//                a.Add(dataName, data);

//            return Content(JsonConvert.SerializeObject(a), "application/json");
//        }
//    }
//}
