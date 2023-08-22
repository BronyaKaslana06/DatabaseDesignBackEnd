using EntityFramework.Context;
using EntityFramework.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Transactions;
using static ASPNETCoreWebAPI_Layer.Controllers.maintenance_itemsInfoController;

namespace ASPNETCoreWebAPI_Layer.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class maintenance_itemsInfoController : ControllerBase
    {
        private readonly ModelContext modelContext;

        public maintenance_itemsInfoController(ModelContext modelContext)
        {
            this.modelContext = modelContext;
        }

        /// <summary>
        /// 订单信息查询
        /// </summary>
        /// <param name="maintenance_items_id"></param>
        /// <param name="vehicle_id"></param>
        /// <param name="maintenance_location"></param>
        /// <param name="order_status"></param>    ///取值为unhandled    handling     finish    Unknown
        /// <returns></returns>
        [HttpGet]    
        public ActionResult<object> Message(string? maintenance_items_id, string? vehicle_id,
            string? maintenance_location, string? order_status)
        {
            IEnumerable<MaintenanceItem> tmp = null;
            if (!string.IsNullOrEmpty(maintenance_items_id))
                tmp = modelContext.MaintenanceItems.Where(e => e.MaintenanceItemId == long.Parse(maintenance_items_id));
            if (!string.IsNullOrEmpty (vehicle_id) && tmp!=null)
                tmp=tmp.Where(e=>e.vehicle.VehicleId==long.Parse(vehicle_id));
            if (!string.IsNullOrEmpty(maintenance_location) && tmp != null)
                tmp = tmp.Where(e => e.MaintenanceLocation == maintenance_location);
            if (!string.IsNullOrEmpty (order_status) && tmp != null)
                tmp=tmp.Where(e=>e.OrderStatusEnum.ToString()==order_status);

            if (tmp == null)
                return NotFound("No results found.");
            var res = tmp.Select(e => new
            {
                maintenance_items_id = e.MaintenanceItemId,
                vehicle_id = e.vehicle.VehicleId,
                maintenance_location = e.MaintenanceItemId,
                order_status = e.OrderStatusEnum.ToString()
            });
            return new JsonResult(res);
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
        /// 更新订单信息
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
        /// 删除订单信息
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

        [HttpPost]
        public async Task<ActionResult<string>> Addition([FromBody]mntnc_items_add Mntnc_Items_add)
        {
            using (TransactionScope tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var items = Mntnc_Items_add.items;
                    foreach (var item in items)
                    {
                        MaintenanceItem tmp = new MaintenanceItem();
                        tmp.vehicle=modelContext.Vehicles.Single(e=>e.VehicleId==long.Parse(item.vehicle_id));
                        tmp.MaintenanceLocation = item.mntnc_loc;
                        if (Enum.TryParse(item.order_status, out OrderStatusEnum os_enum))
                            tmp.OrderStatusEnum = os_enum;
                        else
                            return BadRequest("fail to convert order_status");

                        // 定义日期时间字符串的格式化
                        string format = "yyyy-MM-dd HH:mm:ss";
                        // 尝试将字符串转换为 DateTime
                        if (DateTime.TryParseExact(item.service_time, format, null, System.Globalization.DateTimeStyles.None, out DateTime result))
                        {
                            tmp.ServiceTime = result;
                        }
                        else
                        {
                            return BadRequest("fail to convert service_time");
                        }

                        await modelContext.MaintenanceItems.AddAsync(tmp);
                    }
                    await modelContext.SaveChangesAsync();
                    tx.Complete();
                    return Ok("Success");
                }
                catch (Exception ex)
                {
                    return BadRequest("Add Fail:" + ex.Message);
                }
            }
        }

        
















        [HttpGet]
        public string tes(long id)  
        {
            return modelContext.Batteries.Where(e=>e.BatteryId==id).Select(a=>a.vehicle.VehicleId).FirstOrDefault().ToString();
        }
        [HttpPost("{id}/{id2}")] 
        public string Demo2([FromRoute(Name = "id")] string d1, [FromRoute(Name = "id")] string d2) 
        {
            Battery battery_tmp = new Battery(); 
            BatteryType batteryType_tmp = new BatteryType();
            using (TransactionScope tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    battery_tmp.AvailableStatusEnum = AvailableStatusEnum.onCar;
                    battery_tmp.CurrChargeTimes = 0;
                    battery_tmp.CurrentCapacity = 70;
                    battery_tmp.ManufacturingDate=DateTime.Now;

                    batteryType_tmp.MaxChargeTiems = 1000;
                    batteryType_tmp.TotalCapacity = 200;

                    battery_tmp.batteryType = batteryType_tmp;

                    modelContext.Batteries.Add(battery_tmp);
                    modelContext.BatteryTypes.Add(batteryType_tmp);
                    modelContext.SaveChangesAsync();
                    tx.Complete();
                }
                catch (Exception ex)
                {
                    return "Error: " + ex.Message + $"d1+d2={d1+d2}";
                }
            }
            return "success:"+battery_tmp.ToString()+batteryType_tmp.ToString() + $"d1={d1+d2}";
        }
    }
}
