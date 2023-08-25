using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using EntityFramework.Context;
using EntityFramework.Models;
using Idcreator;
using System.Transactions;
using System.Globalization;

namespace webapi.Controllers.Administrator
{
    [Route("owner/repair_reservation/[action]")]
    [ApiController]
    public class RepairController : ControllerBase
    {
        private readonly ModelContext _context;

        public RepairController(ModelContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Vehicle>> own_query(string owner_id = "")
        {
            if (!long.TryParse(owner_id, out long id))
            {
                var obj = new
                {
                    code = 1,
                    msg = "车主id非法",
                    totalData = 0,
                    data = "",
                };
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }
            try
            {
                var filteredItems = _context.VehicleOwners
                    .Where(item => item.OwnerId == id)
                    .OrderBy(item => item.OwnerId)
                    .SelectMany(item => item.vehicles)
                    .Select(item => new
                    {
                        vehicle_id = item.VehicleId,
                        plate_number = item.PlateNumber
                    })
                    .ToList();

                int totalNum = filteredItems.Count();
                var obj = new
                {
                    code = 0,
                    msg = "success",
                    totalData = totalNum,
                    data = filteredItems,
                };
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }
            catch (Exception ex)
            {
                var obj = new
                {
                    code = 1,
                    msg = "fail",
                    totalData = 0,
                    data = "",
                };
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }
        }
        [HttpGet]
        public ActionResult<IEnumerable<Vehicle>> info_query(string vehicle_id = "")
        {
            if (!long.TryParse(vehicle_id, out long id))
            {
                var obj = new
                {
                    code = 1,
                    msg = "车辆id非法",
                    totalData = 0,
                    data = "",
                };
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }
            try
            {
                var filteredItems = _context.Vehicles
                    .Where(item => item.VehicleId == id)
                    .OrderBy(item => item.VehicleId)
                    .Select(item => new
                    {
                        vehicle_model = item.vehicleParam.ModelName,
                        snip = item.vehicleParam.Sinp,
                        purchase_date = item.PurchaseDate,
                        battery_id = item.Battery.BatteryId.ToString(),
                        current_capacity = item.Battery.CurrentCapacity
                    })
                    .ToList();
                int totalNum = filteredItems.Count();
                var obj = new
                {
                    code = 0,
                    msg = "success",
                    totalData = totalNum,
                    data = filteredItems,
                };
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }
            catch (Exception ex)
            {
                var obj = new
                {
                    code = 1,
                    msg = "fail",
                    totalData = 0,
                    data = "",
                };
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }
        }
        [HttpPost]
        public IActionResult submit([FromBody] dynamic _acm)
        {
            using (TransactionScope tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                _acm = JsonConvert.DeserializeObject(Convert.ToString(_acm));
                bool flag = long.TryParse($"{_acm.vehicle_id}", out long VId);
                if (!flag)
                {
                    var ob = new
                    {
                        code = 1,
                        msg = "VehicleId无效",
                        maintenance_item_id = 0.ToString(),
                    };
                    return Content(JsonConvert.SerializeObject(ob), "application/json");
                }
                long maxMId = _context.MaintenanceItems.Max(o => (long?)o.MaintenanceItemId) ?? 0;
                long newMId = maxMId + 1;
                var acm = new MaintenanceItem()
                {
                    MaintenanceItemId = newMId,
                    vehicle = _context.Vehicles.FirstOrDefault(v => v.VehicleId == VId) ?? throw new Exception("未找到匹配的车辆"),
                    Title = _acm.order_status,
                    MaintenanceLocation = _acm.maintenance_location,
                    Note = _acm.remarks,
                    ServiceTime = DateTime.Now,
                    OrderSubmissionTime = DateTime.Now,
                    OrderStatus = 0,
                    Score = -1,
                };

                _context.Add(acm);

                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateException e)
                {
                    return NewContent(1, e.InnerException?.Message + "");
                }

                var obj = new
                {
                    code = 0,
                    msg = "success",
                    maintenance_item_id = acm.MaintenanceItemId.ToString(),
                };
                tx.Complete();
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }
        }
       
        [HttpPatch]
        public IActionResult update([FromBody] dynamic _acm)
        {
            _acm = JsonConvert.DeserializeObject(Convert.ToString(_acm));
            bool flag = long.TryParse($"{_acm.vehicle_id}", out long vid);
            if (!(long.TryParse($"{_acm.maintenance_item_id}", out long id) && (flag || _acm.vehicle_id == null)))
                return NewContent(1, "记录标记无效");

            var acm = _context.MaintenanceItems.Find(id);
            var Vehicle = _context.Vehicles.Find(vid);
            if (acm == null)
                return NewContent(1, "无记录");
            if (Vehicle == null)
                return NewContent(1, "车辆不存在");
            else if (_acm.evaluations == null)
            {
                acm.vehicle = Vehicle;
                acm.MaintenanceLocation = _acm.maintenance_location;
                acm.Note = _acm.remarks;
                acm.OrderStatus = _acm.order_status == "是" ? 1 : 0;
            }
            else
            {
                if (!double.TryParse($"{_acm.evaluations}", out double s))
                    return NewContent(1, "提交的评价无效");
                acm.Score = s;
            }
            try
            {
                _context.SaveChanges();
            }
            catch(DbUpdateException e)
            {
                return NewContent(1, e.InnerException?.Message+"");
            }
            
            return NewContent(0,"success");
        }
        [HttpDelete]
        public IActionResult delete([FromBody] dynamic _acm)
        {
            _acm = JsonConvert.DeserializeObject(Convert.ToString(_acm));

            bool flag = long.TryParse($"{_acm.maintenance_item_id}", out long id);
            if (!flag)
                return NewContent(1, "记录标记无效");

            var acm = _context.MaintenanceItems.Find(id);

            if (acm == null)
                return NewContent(1, "无记录");
            else
            {
                _context.MaintenanceItems.Remove(acm);
                try
                {
                    _context.SaveChanges();
                }
                catch(DbUpdateException e)
                {
                    return NewContent(1, e.InnerException?.Message + "");
                }
                return NewContent(0,"success");
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