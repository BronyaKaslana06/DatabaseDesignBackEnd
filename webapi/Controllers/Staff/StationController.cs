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
using System.Collections.Generic;
using System.Drawing;

namespace webapi.Controllers.Staff
{
   [Route("staff/switchstation")]
   [ApiController]
   public class StationController : ControllerBase
   {
       private readonly ModelContext _context;

       public StationController(ModelContext context)
       {
           _context = context;
       }


        [HttpGet("station_info")]
       public ActionResult<IEnumerable<Employee>> info(string employee_id = "")
       {
           if (!long.TryParse(employee_id, out long id))
           {
               var obj = new
               {
                   code = 1,
                   msg = "id非法或不存在",
                   data = ""
               };
               return Content(JsonConvert.SerializeObject(obj), "application/json");
           }

           var query = _context.Employees
                   .Where(e => e.EmployeeId == id)
                   .Select(e => new
                   {
                       station_id = e.switchStation.StationId.ToString(),
                       station_name = e.switchStation.StationName,
                       longitude = e.switchStation.Longitude,
                       latitude = e.switchStation.Latitude,
                       faliure_status = e.switchStation.FailureStatus == true ? "是":"否",
                       battery_capacity = e.switchStation.BatteryCapacity,
                       available_battery_count = e.switchStation.AvailableBatteryCount
                   })
                   .FirstOrDefault();

           var totalNum = _context.VehicleOwners.Count();
           var responseObj = new
           {
               code = 0,
               msg = "success",
               data = query,
           };
           return Content(JsonConvert.SerializeObject(responseObj), "application/json");
       }

       [HttpGet("battery")]
       public ActionResult<IEnumerable<Battery>> battery(int pageIndex = 0, int pageSize = 0, string station_id = "", string available_status = "", string battery_type_id = "")
       {
           int offset = (pageIndex - 1) * pageSize;
           int limit = pageSize;

           if (offset < 0 || limit <= 0)
           {
               var errorResponse = new
               {
                   code = 1,
                   msg = "页码或页大小非正",
                   totalData = 0,
                   data = "",
               };
               return Content(JsonConvert.SerializeObject(errorResponse), "application/json");
           }
           if (!long.TryParse(station_id, out long id))
           {
               var obj = new
               {
                   code = 1,
                   msg = "id非法或不存在",
                   totaldata = 0,
                   data = ""
               };
               return Content(JsonConvert.SerializeObject(obj), "application/json");
           }

    
            int availableStatusValue = 0; // 默认值为0
            if (!string.IsNullOrEmpty(available_status))
            {
                availableStatusValue = (int)Enum.Parse(typeof(AvailableStatusEnum), available_status, ignoreCase: true);
            }
            var query = _context.Batteries
                   .Where(b => b.switchStation.StationId == id &&
                   (battery_type_id == "" || b.batteryType.Name == battery_type_id) &&
                   (available_status == "" || b.AvailableStatus == availableStatusValue))
                   .Select(b => new
                   {
                       battery_id = b.BatteryId.ToString(),
                       available_status = ((AvailableStatusEnum)b.AvailableStatus).ToString(),
                       current_capacity = b.CurrentCapacity,
                       curr_charge_times = b.CurrChargeTimes,
                       manufacturing_date = b.ManufacturingDate.ToString(),
                       battery_type_id = b.batteryType.BatteryTypeId,
                       isEditing = false
                   })
                   .Skip(offset)
                   .Take(limit)
                   .ToList();

           var totalNum = query.Count();
           var responseObj = new
           {
               code = 0,
               msg = "success",
               totaldata = totalNum,
               data = query,
           };
           return Content(JsonConvert.SerializeObject(responseObj), "application/json");
       }

       [HttpPatch("battery/update")]
       public IActionResult BatteryUpdate([FromBody] dynamic param)
       {
           dynamic _param = JsonConvert.DeserializeObject(Convert.ToString(param));
           var bty = _context.Batteries.Find($"{_param.battery_id}");
           if (bty == null)
           {
               return NewContent(1, "查询电池不存在");
           }
           var station = _context.SwitchStations.Find($"{_param.station_id}");
           if (station == null)
           {
               return NewContent(1, "查询电池不存在");
           }

           if (_param.available_status != null)
           {
               if (Enum.TryParse(_param.available_status, out AvailableStatusEnum availableStatus))
               {
                   bty.AvailableStatusEnum = availableStatus;
                   bty.switchStation = station;
               }
               else
               {
                   return NewContent(2, "无效的可用状态值");
               }
           }

           try
           {
               _context.SaveChanges();
           }
           catch (DbUpdateException e)
           {
               return NewContent(3, e.InnerException?.Message + "");
           }

           return NewContent();
       }

       [HttpPost("battery/add")]
       public ActionResult<string> PostBattery([FromBody] dynamic _battery)
       {
           using (TransactionScope tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
           {
               if (_context.Batteries == null)
               {
                   return Problem("Entity set 'ModelContext.Batteries' is null.");
               }
               dynamic battery = JsonConvert.DeserializeObject(Convert.ToString(_battery));
               if(!long.TryParse($"{battery.station_id}", out long sid))
               {
                   var ob = new
                   {
                       code = 1,
                       msg = "站点id无效",
                       battery_id = "0"
                   };
                   return Content(JsonConvert.SerializeObject(ob), "application/json");
               }
               long btid = $"{battery.battery_type_id}" == "标准续航型" ? 1 : 2;
               long maxBtyId = _context.Batteries.Max(o => (long?)o.BatteryId) ?? 0;
               long newBtyId = maxBtyId + 1;
               Battery new_bty = new Battery()
               {
                   BatteryId = newBtyId,
                   AvailableStatus = 1,
                   CurrentCapacity = 100.00,
                   CurrChargeTimes = 0,
                   ManufacturingDate = battery.manufacturing_date,
                   switchStation = _context.SwitchStations.Find(sid),
                   batteryType = _context.BatteryTypes.Find(btid)
               };
               _context.Batteries.Add(new_bty);
               try
               {
                   _context.SaveChanges();
               }
               catch (DbUpdateException e)
               {
                   var a = new
                   {
                       code = 1,
                       msg = e.InnerException?.Message
                   };

                   return Conflict(a);
               }
               var obj = new
               {
                   code = 0,
                   msg = "success",
                   battery_id = newBtyId
               };
               return Content(JsonConvert.SerializeObject(obj), "application/json");
           }
       }
        
       [HttpDelete("battery/delete")]
       public IActionResult batterydelete([FromBody] dynamic param)
       {
           using (TransactionScope tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
           {
               dynamic _param = JsonConvert.DeserializeObject(Convert.ToString(param));
               if (_context.Batteries == null)
               {
                   return Problem("Entity set 'ModelContext.Batteries' is null.");
               }
               if (!long.TryParse($"{_param.battery_id}", out long bid))
               {
                   return Problem("电池id非法");
               }
               var bty = _context.VehicleOwners.Find(bid);
               if (bty == null)
               {
                   return NewContent(1, "找不到该车主");
               }
                
               _context.VehicleOwners.Remove(bty);
               try
               {
                   _context.SaveChanges();
                   tx.Complete();
               }
               catch (DbUpdateException e)
               {
                   return NewContent(1, e.InnerException?.Message);
               }
               return NewContent();
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
