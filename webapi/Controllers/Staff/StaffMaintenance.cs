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

namespace webapi.Controllers.Staff
{
   [Route("staff/")]
   [ApiController]
   public class StaffMaintenance : ControllerBase
   {
       private readonly ModelContext _context;

       public StaffMaintenance(ModelContext context)
       {
           _context = context;
       }

       [HttpGet("maintanence/detail")]
       public ActionResult<IEnumerable<Employee>> GetMaintenanceItem(long maintenance_item_id)
       {
           var query = _context.MaintenanceItems.Where(maintenance_item => maintenance_item.MaintenanceItemId == maintenance_item_id).Select
           (maintenance_item => new
           {
               maintenance_location = maintenance_item.MaintenanceLocation,
               plate_number = maintenance_item.vehicle.PlateNumber,
               vehicle_model = maintenance_item.vehicle.vehicleParam.ModelName,
               order_status =(OrderStatusEnum)maintenance_item.OrderStatus,
               title = maintenance_item.Title,
               order_submission_time = maintenance_item.OrderSubmissionTime,
               appoint_time=maintenance_item.AppointTime,
               longitude=maintenance_item.longitude,
               latitude=maintenance_item.latitude,
               service_time = maintenance_item.ServiceTime,
               remarks = maintenance_item.Note,
               username = maintenance_item.vehicle.vehicleOwner.Username,
               phone_number = maintenance_item.vehicle.vehicleOwner.PhoneNumber,
           }).ToList();
           if (query.Count() == 0)
               return NewContent(1, "id不存在");
           else
           {
               var a = new
               {
                   code = 0,
                   msg = "success",
                   data = query[0]
               };
               return Content(JsonConvert.SerializeObject(a), "application/json");
           }
       }
       [HttpGet("maintanence/doortodoor")]
       public ActionResult GetMaintenanceArray(long employee_id , string order_status="待完成")
       {
           bool isEmployee = false;
           int orderStatusEnum;
           if (order_status == OrderStatusEnum.待完成.ToString())
           {
               isEmployee = false;
               orderStatusEnum = (int)OrderStatusEnum.待完成;
           }
           else if (order_status == OrderStatusEnum.待接单.ToString())
           {
               isEmployee = true;
               orderStatusEnum = (int)OrderStatusEnum.待接单;
           }
           else
           {
               return NewContent(1, "订单状态不是待分配或待完成");
           }

           var maintenance_array = _context.MaintenanceItems
                  .Where(e => e.OrderStatus == orderStatusEnum&&
                  (isEmployee || e.employees.Any(e => e.EmployeeId == employee_id)))
                  .OrderBy(a => a.AppointTime)
                  .Select(maintenance_item => new
                  {
                      maintenance_item_id=maintenance_item.MaintenanceItemId,
                      vehicle_model = maintenance_item.vehicle.vehicleParam.ModelName,
                      vehicle_id = maintenance_item.vehicle.VehicleId,
                      plate_number = maintenance_item.vehicle.PlateNumber,
                      title = maintenance_item.Title,
                      phone_number = maintenance_item.vehicle.vehicleOwner.PhoneNumber,
                      username = maintenance_item.vehicle.vehicleOwner.Username,
                      }
                  )
                  .ToList();
           var a = new
           {
               code = 0,
               msg = "success",
               maintanence_item_array = maintenance_array
           };
           return Content(JsonConvert.SerializeObject(a), "application/json");

       }

       [HttpPost("maintanence/receive")]
       public ActionResult Receive([FromBody] dynamic param)
       {
           dynamic body = JsonConvert.DeserializeObject(Convert.ToString(param));
           if (!long.TryParse($"{body.maintenance_item_id}", out var maintenance_item_id))
               return NewContent(1, "id非法");
           if (!long.TryParse($"{body.employee_id}", out var employee_id))
               return NewContent(3, "id非法");

           var maintanceItem = _context.MaintenanceItems.Include(a=>a.employees).FirstOrDefault(a=>a.MaintenanceItemId==maintenance_item_id);
            
           if (maintanceItem == null)
               return NewContent(2, "无此id的维修项");

           var employee = _context.Employees.Find(employee_id);

           if (employee == null)
               return NewContent(4, "无此id的员工");

           maintanceItem.employees.Add(employee);
           maintanceItem.OrderStatusEnum = OrderStatusEnum.待完成;
           _context.SaveChanges();

           return NewContent();
       }

       [HttpPost("maintanence/submit")]
       public ActionResult Complete([FromBody] dynamic param)
       {
           dynamic body = JsonConvert.DeserializeObject(Convert.ToString(param));
           if (!long.TryParse($"{body.maintenance_item_id}", out var maintenance_item_id))
               return NewContent(1, "维修项id非法");

           var maintanceItem = _context.MaintenanceItems.Find(maintenance_item_id);

           if (maintanceItem == null)
               return NewContent(2, "无此id的维修项");

           maintanceItem.ServiceTime = DateTime.Now;
           maintanceItem.OrderStatusEnum = OrderStatusEnum.待评分;
           _context.SaveChanges();
           return NewContent();
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
