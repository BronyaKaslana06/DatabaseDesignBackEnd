using EntityFramework.Context;
using EntityFramework.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Transactions;
using static ASPNETCoreWebAPI_Layer.Controllers.maintenance_itemsInfoController;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ASPNETCoreWebAPI_Layer.Controllers
{
    [Route("administrator/[controller]/[action]")]
    [ApiController]
    public class maintenance_itemsInfoController : ControllerBase
    {
        private readonly ModelContext modelContext;

        public maintenance_itemsInfoController(ModelContext modelContext)
        {
            this.modelContext = modelContext;
        }

        /// <summary>
        /// lgy 订单信息查询
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="maintenance_items_id"></param>
        /// <param name="vehicle_id"></param>
        /// <param name="maintenance_location"></param>    //只要包含即可
        /// <param name="order_status"></param>    ///取值为 待接单 待完成 待评分 已完成 
        /// <returns></returns>
        [HttpGet]    
        public ActionResult<object> Message(int pageIndex, int pageSize, string? maintenance_items_id, string? vehicle_id,
            string? maintenance_location, string? order_status)
        {
            var tmp = (modelContext.MaintenanceItems.Include(e => e.vehicle)).Select(a=>a);

            if (!string.IsNullOrEmpty(maintenance_items_id))
                tmp = tmp.Where(e => e.MaintenanceItemId == long.Parse(maintenance_items_id));

            if (!string.IsNullOrEmpty (vehicle_id) && tmp.Any())
                tmp=tmp.Where(e=>e.vehicle.VehicleId==long.Parse(vehicle_id));

            if (!string.IsNullOrEmpty(maintenance_location) && tmp.Any())
                tmp = tmp.Where(e => e.MaintenanceLocation == maintenance_location);

            if (!string.IsNullOrEmpty (order_status) && tmp.Any())
            {
                if (Enum.TryParse(order_status, out OrderStatusEnum os_enum))
                {
                    tmp = tmp.Where(e => e.OrderStatus ==(int)os_enum);
                }
                else
                {
                    return BadRequest("fail to convert order_status");
                }
            }

            if (!tmp.Any())
                return NotFound("No results found.");

            var res=tmp.OrderBy(c => c.MaintenanceItemId).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(e => new
            {
                maintenance_items_id = e.MaintenanceItemId.ToString(),
                vehicle_id = e.vehicle.VehicleId.ToString(),
                maintenance_location = e.MaintenanceLocation,
                order_status = e.OrderStatusEnum.ToString(),
                owner_id = e.vehicle.vehicleOwner.OwnerId.ToString(),
                employees =  e.employees.Select(employees => new
                {
                    employee_id = employees.EmployeeId.ToString()
                }).ToArray()
            });
            return Ok(res);
        }



        public class mntnc_item_update
        {
            public string mntnc_id { get; set; }
            public string vehicle_id { get; set; }
            public string mntnc_loc { get; set; }
            public string order_status { get; set; }
        }
        public class mntnc_items_update
        {
            public List<mntnc_item_update> items { get; set; } = new List<mntnc_item_update>();
        }
        /// <summary>
        /// lgy 更新订单信息
        /// </summary>
        /// <param name="Mntnc_Items"></param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<ActionResult<string>> Updates([FromBody] mntnc_items_update Mntnc_Items )
        {
            using (TransactionScope tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var items = Mntnc_Items.items;
                    foreach (var item in items)
                    {
                        var tmp = modelContext.MaintenanceItems.Single(e => e.MaintenanceItemId == long.Parse(item.mntnc_id));
                        tmp.vehicle = modelContext.Vehicles.Single(a => a.VehicleId == long.Parse(item.vehicle_id));
                        tmp.MaintenanceLocation = item.mntnc_loc;
                        if (Enum.TryParse(item.order_status, out OrderStatusEnum os_enum))
                            tmp.OrderStatusEnum = os_enum;
                        else
                            return BadRequest("fail to convert order_status");
                       tmp.OrderSubmissionTime=DateTime.Now;
                    }
                    await modelContext.SaveChangesAsync();
                    tx.Complete();
                    return Ok("Success");
                }
                catch (Exception ex)
                {
                    return BadRequest("Update Fail"+ex.Message);
                }
            }
        }


        /// <summary>
        /// lgy 删除订单信息
        /// </summary>
        /// <param name="maintenance_items_id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ActionResult<string>> Erasure(string maintenance_items_id)
        {
            using (TransactionScope tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var tmp=modelContext.MaintenanceItems.Single(e=>e.MaintenanceItemId==long.Parse(maintenance_items_id));
                    modelContext.MaintenanceItems.Remove(tmp);
                    await modelContext.SaveChangesAsync();
                    tx.Complete();
                    return Ok("Success");
                }
                catch (Exception ex)
                {
                    return BadRequest("Delete Fail:"+ex.Message);
                }
            }
        }



        public class mntnc_item_add
        {
            public string vehicle_id { get; set; }
            public string mntnc_loc { get; set; }
            public string order_status { get; set; }
            public string service_time { get; set; }
        }
        public class mntnc_items_add
        {
            public List<mntnc_item_add> items { get; set; } = new List<mntnc_item_add>();
        }
        /// <summary>
        /// lgy 添加订单信息
        /// </summary>
        /// <param name="Mntnc_Items_add"></param>
        /// <returns></returns>
        //[HttpPost]
        //public async Task<ActionResult<string>> Addition([FromBody]mntnc_items_add Mntnc_Items_add)
        //{
        //    using (TransactionScope tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //    {
        //        try
        //        {
        //            var items = Mntnc_Items_add.items;
        //            foreach (var item in items)
        //            {
        //                MaintenanceItem tmp = new MaintenanceItem();
        //                tmp.vehicle=modelContext.Vehicles.Single(e=>e.VehicleId==long.Parse(item.vehicle_id));
        //                tmp.MaintenanceLocation = item.mntnc_loc;
        //                if (Enum.TryParse(item.order_status, out OrderStatusEnum os_enum))
        //                    tmp.OrderStatusEnum = os_enum;
        //                else
        //                    return BadRequest("fail to convert order_status");

        //                // 定义日期时间字符串的格式化
        //                string format = "yyyy-MM-dd HH:mm:ss";
        //                // 尝试将字符串转换为 DateTime
        //                if (DateTime.TryParseExact(item.service_time, format, null, System.Globalization.DateTimeStyles.None, out DateTime result))
        //                {
        //                    tmp.ServiceTime = result;
        //                }
        //                else
        //                {
        //                    return BadRequest("fail to convert service_time");
        //                }

        //                await modelContext.MaintenanceItems.AddAsync(tmp);
        //                await Console.Out.WriteLineAsync(tmp.MaintenanceItemId.ToString());
        //            }
        //            await modelContext.SaveChangesAsync();
        //            tx.Complete();
        //            return Ok("Success");
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest("Add Fail:" + ex.Message);
        //        }
        //    }
        //}


        /// <summary>
        /// lgy 获取订单信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<object>> TableMessage(int pageIndex, int pageSize)
        {
            var res = modelContext.MaintenanceItems.Skip((pageIndex - 1) * pageSize).Take(pageSize).Include(c=>c.vehicle).Select(a => new
            {
                maintenance_items_id = a.MaintenanceItemId.ToString(),
                vehicle_id = a.vehicle.VehicleId.ToString(),
                maintenance_location = a.MaintenanceLocation,
                order_status = a.OrderStatusEnum.ToString()
            }).ToList();
            return Ok(res);
        }


        /// <summary>
        /// lgy 管理员查看维修订单的详细信息
        /// </summary>
        /// <param name="maintenance_item_id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<object> MessageId(string maintenance_item_id)
        {
            try
            {
                var f = modelContext.MaintenanceItems.Include(a => a.vehicle).Single(e => e.MaintenanceItemId == long.Parse(maintenance_item_id));
                var res = new
                {
                    maintenance_item_id = f.MaintenanceItemId.ToString(),
                    title = f.Title == null ? "" : f.Title,
                    vehicle_id = f.vehicle.VehicleId.ToString(),
                    order_submission_time = f.OrderSubmissionTime.ToString(),
                    appoint_time=f.AppointTime.ToString(),
                    service_time = f.ServiceTime.HasValue ? f.ServiceTime.ToString() : "",
                    remarks = f.Note == null ? "" : f.Note.ToString(),
                    evaluations = f.Evaluation == null ? "" : f.Evaluation.ToString(),
                    maintenance_location = f.MaintenanceLocation,
                    order_status = f.OrderStatusEnum.ToString(),
                    score = f.Score.ToString(),
                    owner_id = f.vehicle.vehicleOwner.OwnerId.ToString(),
                    employees = f.employees.Select(employees => new
                    {
                        employee_id = employees.EmployeeId.ToString()
                    }).ToArray()
                };
                return Ok(res);
            }
            catch
            {
                return NotFound("maintenance_item_id not exist");
            }
        }














        //[HttpGet]
        //public ActionResult<object> tes()
        //{
        //    Console.WriteLine("=======================sssssssssssstarttttttttttt\n\n");
        //    var tmp = modelContext.MaintenanceItems.Include(e => e.vehicle).Select(a => a);
        //    var res = tmp.Select(e => new
        //    {
        //        hhh = e.Title
        //    });
        //    return Ok(res);
        //}
        //    [HttpPost("{id}/{id2}")] 
        //    public string Demo2([FromRoute(Name = "id")] string d1, [FromRoute(Name = "id")] string d2) 
        //    {
        //        Battery battery_tmp = new Battery(); 
        //        BatteryType batteryType_tmp = new BatteryType();
        //        using (TransactionScope tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //        {
        //            try
        //            {
        //                battery_tmp.AvailableStatusEnum = AvailableStatusEnum.汽车使用中;
        //                battery_tmp.CurrChargeTimes = 0;
        //                battery_tmp.CurrentCapacity = 70;
        //                battery_tmp.ManufacturingDate=DateTime.Now;

        //                batteryType_tmp.MaxChargeTimes = 1000;
        //                batteryType_tmp.TotalCapacity = "98.67KWh";

        //                battery_tmp.batteryType = batteryType_tmp;

        //                modelContext.Batteries.Add(battery_tmp);
        //                modelContext.BatteryTypes.Add(batteryType_tmp);
        //                modelContext.SaveChangesAsync();
        //                tx.Complete();
        //            }
        //            catch (Exception ex)
        //            {
        //                return "Error: " + ex.Message + $"d1+d2={d1+d2}";
        //            }
        //        }
        //        return "success:"+battery_tmp.ToString()+batteryType_tmp.ToString() + $"d1={d1+d2}";
        //    }
    }
}
