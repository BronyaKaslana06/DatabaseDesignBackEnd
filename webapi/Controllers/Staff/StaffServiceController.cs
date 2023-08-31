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

//namespace webapi.Controllers.Staff
//{
//    [Route("staff/")]
//    [ApiController]
//    public class StaffService : ControllerBase
//    {
//        private readonly ModelContext _context;

//        public StaffService(ModelContext context)
//        {
//            _context = context;
//        }

//        [HttpGet("door_to_door_service/get_maintenance_item")]
//        public ActionResult<IEnumerable<Employee>> GetMaintenanceItem(long maintenance_item_id)
//        {
//            var query = _context.MaintenanceItems.Where(maintenance_item =>maintenance_item.MaintenanceItemId== maintenance_item_id).Select
//            (maintenance_item=> new
//            {
//                maintenance_location = maintenance_item.MaintenanceLocation,
//                plate_number = maintenance_item.vehicle.PlateNumber,
//                vehicle_model = maintenance_item.vehicle.vehicleParam.ModelName,
//                order_status = maintenance_item.OrderStatus,
//                title = maintenance_item.Title,
//                order_submission_time = maintenance_item.OrderSubmissionTime,
//                service_time = maintenance_item.ServiceTime,
//                remarks = maintenance_item.Note,
//                username = maintenance_item.vehicle.vehicleOwner.Username,
//                phone_number = maintenance_item.vehicle.vehicleOwner.PhoneNumber,
//            }).ToList();
//            if (query.Count()==0)
//                return NewContent(1, "id不存在");
//            else
//            {
//                var a = new
//                {
//                    code = 0,
//                    msg = "success",
//                    data = query[0]
//                };
//                return Content(JsonConvert.SerializeObject(a), "application/json");
//            }
//        }
//        [HttpGet("door_to_door_service/get_maintenance_array")]
//        public ActionResult GetMaintenanceArray(long employee_id)
//        {
//            var maintenance_array = _context.MaintenanceItems
//                .Where(e => e.OrderStatus != (int)OrderStatusEnum.已完成)
//                .Where(e => e.employees.Any(t => t.EmployeeId == employee_id))
//                .Select(maintenance_item=> new
//                {
//                    maintenance_location = maintenance_item.MaintenanceLocation,
//                    plate_number = maintenance_item.vehicle.PlateNumber,
//                    vehicle_model = maintenance_item.vehicle.vehicleParam.ModelName,
//                    order_status = maintenance_item.OrderStatusEnum,
//                    title = maintenance_item.Title,
//                    order_submission_time = maintenance_item.OrderSubmissionTime,
//                    service_time = maintenance_item.ServiceTime,
//                    remarks = maintenance_item.Note,
//                    username = maintenance_item.vehicle.vehicleOwner.Username,
//                    phone_number = maintenance_item.vehicle.vehicleOwner.PhoneNumber,
//                }
//                ).ToList();

//            var a = new
//            {
//                code = 0,
//                msg = "success",
//                switch_request_array = maintenance_array
//            };
//            return Content(JsonConvert.SerializeObject(a), "application/json");

//        }
        
//        [HttpPatch("doortodoorservice/maintance-items")]
//        public ActionResult Complete(long employee_id=-1,long repair_request_id=-1,string service_time="")
//        {
//            var maintanceItem= _context.MaintenanceItems.Find(repair_request_id);
//            var employee = _context.Employees.Find(employee_id);
//            if (employee == null)
//                return NewContent(1, "无此员工");
//            if(maintanceItem==null)
//                return NewContent(1, "无此维修项");

//            maintanceItem.ServiceTime = DateTime.Now;
//            maintanceItem.OrderStatusEnum = OrderStatusEnum.已完成;
//            return NewContent();
//        }
//        [HttpPost("doortodoorservice/switchrecords")]
//        public ActionResult PostSwitchrecords(long employee_id=-1,long switch_request_id=-1,long battery_id=-1,string switch_time="")
//        {
//            var switch_request = _context.SwitchRequests.Find(switch_request_id);
//            var employee = _context.Employees.Find(employee_id);
//            if (employee == null)
//                return NewContent(1, "无此员工");
//            if (switch_request == null)
//                return NewContent(1, "无此换电请求");
//            return NewContent();
//        }
//        ContentResult NewContent(int _code = 0, string _msg = "success")
//        {
//            var a = new
//            {
//                code = _code,
//                msg = _msg
//            };
//            return Content(JsonConvert.SerializeObject(a), "application/json");
//        }
//    }
//}
