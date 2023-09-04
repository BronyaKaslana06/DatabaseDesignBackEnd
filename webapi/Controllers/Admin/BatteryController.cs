using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using EntityFramework.Context;
using EntityFramework.Models;
using Idcreator;
using webapi.Tools;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace webapi.Controllers.Admin
{
    [Route("administrator/battery")]
    [ApiController]
    public class BatteryController : ControllerBase
    {
        private readonly ModelContext _context;

        public BatteryController(ModelContext context)
        {
            _context = context;
        }

        [HttpGet("query")]
        public ActionResult<IEnumerable<Battery>> battery(int pageIndex = 0, int pageSize = 0, string available_status = "", string battery_type_id = "", int battery_status = 0, string keyword = "")
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

            int availableStatusValue = 0;
            if (!string.IsNullOrEmpty(available_status))
            {
                availableStatusValue = (int)Enum.Parse(typeof(AvailableStatusEnum), available_status, ignoreCase: true);
            }
            var query = _context.Batteries
                   .Where(b => (battery_type_id == "" || b.batteryType.Name == battery_type_id) &&
                   (available_status == "" || b.AvailableStatus == availableStatusValue))
                   .Select(b => new
                   {
                       battery_id = b.BatteryId.ToString(),
                       available_status = ((AvailableStatusEnum)b.AvailableStatus).ToString(),
                       current_capacity = b.CurrentCapacity,
                       curr_charge_times = b.CurrChargeTimes,
                       manufacturing_date = b.ManufacturingDate.ToString(),
                       battery_type_id = b.batteryType.Name,
                       name = battery_status == 0 ? b.switchStation.StationName : b.vehicle.PlateNumber,
                       Similarity = battery_status == 0 ? Calculator.ComputeSimilarityScore(b.switchStation.StationName, keyword) : Calculator.ComputeSimilarityScore(b.vehicle.PlateNumber == null ? "" : b.vehicle.PlateNumber, keyword),
                       isEditing = false
                   })
                   .Skip(offset)
                   .Take(limit)
                   .ToList();
            var filteredItems = query
                    .Where(item => item.Similarity >= (double)0)
                    .OrderByDescending(item => item.Similarity);

            var totalNum = filteredItems.Count();
            var responseObj = new
            {
                code = 0,
                msg = "success",
                totaldata = totalNum,
                data = filteredItems,
            };
            return Content(JsonConvert.SerializeObject(responseObj), "application/json");
        }
        
    }
}