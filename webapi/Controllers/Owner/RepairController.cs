using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using EntityFramework.Context;
using EntityFramework.Models;
using Idcreator;
using System.Transactions;
using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace webapi.Controllers.Owner
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
        public ActionResult<IEnumerable<MaintenanceItem>> query(string maintenance_item_id = "")
        {
            bool flag = long.TryParse(maintenance_item_id, out var id);
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
            try
            {
                var filteredItems = _context.MaintenanceItems
                    .SelectMany(mi => mi.employees, (mi, emp) => new { MaintenanceItem = mi, Employee = emp })
                    .Join(_context.Employees, miEmp => miEmp.Employee, emp => emp, (miEmp, emp) => new { miEmp.MaintenanceItem, Employee = emp })
                    .Join(_context.Vehicles, miEmp => miEmp.MaintenanceItem.vehicle.VehicleId, veh => veh.VehicleId, (miEmp, veh) => new { miEmp.MaintenanceItem, miEmp.Employee, Vehicle = veh })
                    .Where(joinedData => maintenance_item_id == "" || joinedData.MaintenanceItem.MaintenanceItemId == id)
                    .OrderBy(joinedData => joinedData.MaintenanceItem.MaintenanceItemId)
                    .Select(joinedData => new
                    {
                        maintenance_location = joinedData.MaintenanceItem.MaintenanceLocation,
                        plate_number = joinedData.Vehicle.PlateNumber,
                        title = joinedData.MaintenanceItem.Title,
                        order_submission_time = joinedData.MaintenanceItem.OrderSubmissionTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        service_time = joinedData.MaintenanceItem.ServiceTime != null ? joinedData.MaintenanceItem.ServiceTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "未完成服务",
                        longtitude = joinedData.MaintenanceItem.longitude,
                        latitude = joinedData.MaintenanceItem.latitude,
                        appoint_time = joinedData.MaintenanceItem.AppointTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        order_status = joinedData.MaintenanceItem.OrderStatusEnum.ToString(),
                        remarks = joinedData.MaintenanceItem.Note,
                        evaluations = joinedData.MaintenanceItem.Evaluation,
                        score = joinedData.MaintenanceItem.Score,
                        name = joinedData.Employee.Name,
                        phone_number = joinedData.Employee.PhoneNumber
                    })
                    .FirstOrDefault();


                var obj = new
                {
                    code = 0,
                    msg = "success",
                    totalData = 1,
                    data = filteredItems,
                };

                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    code = 1,
                    msg = ex.Message,
                    totalData = 0,
                    data = new DataTable(),
                };

                return Content(JsonConvert.SerializeObject(errorResponse), "application/json");
            }
        }
        [HttpGet]
        public ActionResult<IEnumerable<MaintenanceItem>> rough_query(string vehicle_id = "", string start_time = "", string end_time = "")
        {
            DateTime parsedStartTime, parsedEndTime;

            if (!DateTime.TryParseExact(start_time, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedStartTime) ||
                !DateTime.TryParseExact(end_time, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedEndTime))
            {
                var errorObj = new
                {
                    code = -1,
                    msg = "日期格式不合法。请使用yyyy-MM-dd。",
                    totalData = 0,
                    data = new DataTable(),
                };
                return Content(JsonConvert.SerializeObject(errorObj), "application/json");
            }
            string orderStatusName;
            var filteredItems = _context.MaintenanceItems
                .Where(item => item.OrderSubmissionTime >= parsedStartTime && item.OrderSubmissionTime <= parsedEndTime && item.vehicle.VehicleId.ToString() == vehicle_id)
                .OrderBy(item => item.MaintenanceItemId)
                .Select(item => new
                {
                    maintenance_item_id = item.MaintenanceItemId,
                    title = item.Title,
                    order_submission_time = item.OrderSubmissionTime,
                    order_status = item.OrderStatusEnum.ToString(),
                    maintenance_location = item.MaintenanceLocation
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
                    vehicle = _context.Vehicles.DefaultIfEmpty().FirstOrDefault(v => v.VehicleId == VId) ?? throw new Exception("未找到匹配的车辆"),
                    Title = _acm.title,
                    MaintenanceLocation = _acm.maintenance_location,
                    Note = _acm.remarks,
                    AppointTime = DateTime.Parse(_acm.appoint_time),
                    OrderStatus = 1,
                    ServiceTime = null,
                    OrderSubmissionTime = DateTime.Now,
                    longitude = _acm.longitude,
                    latitude = _acm.latitude
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
            if (!(long.TryParse($"{_acm.maintenance_item_id}", out long id)))
                return NewContent(1, "记录标记无效");

            var acm = _context.MaintenanceItems.Find(id);
            if (acm == null)
                return NewContent(1, "无记录");
            else if (_acm.evaluations == null)
            {
                acm.MaintenanceLocation = _acm.maintenance_location;
                acm.Note = _acm.remarks;
                acm.OrderStatus = _acm.order_status;
                acm.AppointTime = DateTime.Parse(_acm.appoint_time);
                acm.latitude = _acm.latitude;
                acm.longitude = _acm.longitude;
            }
            else
            {
                acm.Evaluation = _acm.evaluations;
                acm.Score = _acm.score;
                acm.OrderStatus = 4;
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
        public IActionResult delete(string maintenance_item_id = "")
        {
            bool flag = long.TryParse(maintenance_item_id, out long id);
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