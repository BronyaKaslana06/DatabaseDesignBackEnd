using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using EntityFramework.Context;
using EntityFramework.Models;
using Idcreator;

namespace webapi.Controllers.Administrator
{
    [Route("owner/repair_reservation")]
    [ApiController]
    public class RepairController : ControllerBase
    {
        private readonly ModelContext _context;

        public RepairController(ModelContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult submit([FromBody] dynamic _acm)
        {
            _acm = JsonConvert.DeserializeObject(Convert.ToString(_acm));

            long id = SnowflakeIDcreator.nextId();
            var acm = new MaintenanceItem()
            {
                MaintenanceItemId = id,
                VehicleId = _acm.vehicle_id,
                Title = _acm.order_status,
                MaintenanceLocation = _acm.maintenance_location,
                Note = _acm.remarks,
                ServiceTime = DateTime.Now,
                OrderSubmissionTime = DateTime.Now,
                OrderStatus = 0,
                Evaluations = null,
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
                maintenance_item_id = id,
            };
            return Content(JsonConvert.SerializeObject(obj), "application/json");
        }
        [HttpPatch]
        public IActionResult submit_evaluations([FromBody] dynamic _acm)
        {
            _acm = JsonConvert.DeserializeObject(Convert.ToString(_acm));

            bool flag = long .TryParse($"{_acm.maintenance_item_id}",out long id);
            if (!flag)
                return NewContent(1, "记录标记非法");
            if (id == null)
                return NewContent(1, "记录标记为空");

            var acm = _context.MaintenanceItems.Find(id);

            if (acm == null)
                return NewContent(1, "无记录");
            else
            {
                acm.Evaluations = _acm.evaluations;
            }
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                return NewContent(1, e.InnerException?.Message + "");
            }

            return NewContent(0, "success");
        }
        [HttpPatch]
        public IActionResult update([FromBody] dynamic _acm)
        {
            _acm = JsonConvert.DeserializeObject(Convert.ToString(_acm));
            
            string id = $"{_acm.maintenance_item_id}";
            if(id == null)
                return NewContent(1, "记录标记为空");

            var acm = _context.MaintenanceItems.Find(id);

            if(acm==null)
                return NewContent(1, "无记录");
            else
            {
                acm.vehicle.VehicleId = _acm.vehicle_id;
                acm.MaintenanceLocation = _acm.maintenance_location;
                acm.Remarks = _acm.remarks;
                acm.OrderStatus = _acm.order_status;
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

            string id = $"{_acm.maintenance_item_id}";
            if (id == null)
                return NewContent(1, "记录标记为空");

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