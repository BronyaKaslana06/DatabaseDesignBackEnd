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

namespace webapi.Controllers.Administrator
{
    [Route("administrator/stationInfo")]
    [ApiController]
    public class StationInfoController : ControllerBase
    {
        private readonly ModelContext _context;

        public StationInfoController(ModelContext context)
        {
            _context = context;
        }

        [HttpGet("query")]
        public ActionResult<IEnumerable<Employee>> GetPage_(int pageIndex=1, int pageSize=1, string station_name = "", long station_id = -1, long employee_id = -1,string faliure_status="")
        {
            int offset = (pageIndex - 1) * pageSize;
            int limit = pageSize;
            if (offset < 0 || limit <= 0)
                return NewContent(0,"页码或页大小错误");

            bool? status=null;
            if(faliure_status=="是")
                status=true;
            else if(faliure_status=="否")
                status=false;
            var query = _context.SwitchStations
            .Where(e =>
            (e.StationId == station_id || station_id == -1)
            && e.StationName.Contains(station_name)
            && (e.employees.Any(a => a.EmployeeId == employee_id&&a.Position==(int)PositionEnum.换电站管理员) || employee_id == -1)
            && ((status!=null&& e.FailureStatus == status) || faliure_status == ""))
            .Select(e=>new{
                station_id=e.StationId,
                employee_id=e.employees.FirstOrDefault(a=>a.switchStation.StationId==e.StationId&&a.Position== (int)PositionEnum.换电站管理员),
                station_name=station_name,
                longitude=e.Longitude,
                latitude=e.Latitude,
                faliure_status=faliure_status,
                battety_capacity=e.BatteryCapacity,
                available_battery_count=e.AvailableBatteryCount,
                electricity_fee=e.ElectricityFee,
                service_fee=e.ServiceFee,
                station_address=e.Address
            }).Skip(offset)
            .Take(limit).ToList();


//             int offset = (pageIndex - 1) * pageSize;
//             int limit = pageSize;
//             if (offset < 0 || limit <= 0)
//             {
//                 var t = new
//                 {
//                     code = 1,
//                     msg = "页码或页大小非正",
//                     totalData = 0,
//                     data = "",
//                 };
//                 return Content(JsonConvert.SerializeObject(t), "application/json");
//             }
//             string pattern1 = "'%" + (station_name == String.Empty ? "" : station_name) + "%'";
//             string pattern2 = "'%" + (station_id == String.Empty ? "" : station_id) + "%'";
//             string pattern3 = employee_id == String.Empty ? "" : " AND " + "employee.employee_id like "+"'%" +  employee_id + "%'";
//             string pattern4 = "'%" + (faliure_status == String.Empty ? "" : faliure_status) + "%'";
//             string where_cause = "and " + "station_name like " + pattern1 +
//                 " AND " + "SWITCH_STATION.station_id like " + pattern2 +
//                   pattern3 +
//                 " AND " + "faliure_status like " + pattern4;
//             string sql_info = "SELECT SWITCH_STATION.station_id,employee.employee_id,station_name,LONGTITUDE,latitude,faliure_status,BATTERY_CAPACITY,available_battery_count,electricity_fee,service_fee " +
//                 "FROM " +
//                 @"employee Right OUTER JOIN EMPLOYEE_SWITCH_STATION ON EMPLOYEE.employee_id = EMPLOYEE_SWITCH_STATION.employee_id
// Right OUTER JOIN SWITCH_STATION ON EMPLOYEE_SWITCH_STATION.station_id = SWITCH_STATION.station_id
// WHERE positions = '管理员' or employee.employee_id IS NULL "
//  + 
//                 where_cause +
//                 "ORDER BY station_id " +
//                 "OFFSET " + offset.ToString() + " ROWS " +
//                 "FETCH NEXT " + limit.ToString() + " ROWS ONLY";
//             Console.Write(sql_info);
//             DataTable df = OracleHelper.SelectSql(sql_info);
//             string sql_total = "SELECT COUNT(*) " +
//                 "FROM EMPLOYEE " +
//                 "NATURAL JOIN EMPLOYEE_SWITCH_STATION " +
//                 "NATURAL JOIN SWITCH_STATION " + where_cause;
//             int totalNum = df.Rows.Count;
            var obj = new
            {
                code=0,
                msg="success",
                totalData = query.Count(),
                data = query,
            };
            return Content(JsonConvert.SerializeObject(obj), "application/json");
        }

        [HttpPatch]
        public IActionResult PutStaff([FromBody] dynamic _station)
        {
            dynamic station = JsonConvert.DeserializeObject(Convert.ToString(_station));
            if(!(long.TryParse($"{station.station_id}", out long station_id) ))//&& long.TryParse($"{station.employee_id}", out long EID)
            {
                return NewContent(1, "id非法");
            }
            var staff = _context.SwitchStations.Find(station_id);

            if (staff == null)
            {
                return NewContent(1,"没找到该station");
            }

            //var newEmployee = _context.Employees.Find(EID);

            // if (newEmployee == null)
            // {
            //     return NewContent(1, "没有该id的管理员");
            // }

            //station.Employees.Add(newEmployee);

            //staff.StationId = $"{station.station_id}";
            if ($"{station.station_name}" != String.Empty)
                staff.StationName = $"{station.station_name}";
            if (int.TryParse($"{station.battety_capacity}",out var battety_capacity))
                staff.BatteryCapacity = battety_capacity;
            if (double.TryParse($"{station.longitude}" ,out var longi))
                staff.Longitude = longi;
            if (double.TryParse($"{station.latitude}" ,out var lan))
                staff.Latitude = lan;
            if ($"{station.faliure_status}" != String.Empty)
                staff.FailureStatus = $"{station.faliure_status}" == "是" ? true : false;
            if (int.TryParse($"{station.available_battery_count}", out var cout))
                staff.AvailableBatteryCount = cout;

            staff.Address = $"{station.station_address}" == String.Empty ? staff.Address : $"{station.station_address}";
            staff.ServiceFee = float.TryParse($"{station.service_fee}" ,out var k)? staff.ServiceFee : k;
         

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                return NewContent(1, e.InnerException?.Message+"");
            }

            return NewContent();
        }

        [HttpPost]
        public ActionResult<string> PostStaff([FromBody] dynamic _station)
        {
            using (TransactionScope tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                dynamic station = JsonConvert.DeserializeObject(Convert.ToString(_station));
                if (!(long.TryParse($"{station.station_id}", out long station_id)))
                {
                    return NewContent(1, "id非法");
                }
                SwitchStation staff = new SwitchStation();
                if ($"{station.station_name}" != String.Empty)
                    staff.StationName = $"{station.station_name}";
                if (int.TryParse($"{station.battety_capacity}", out var battety_capacity))
                    staff.BatteryCapacity = battety_capacity;
                if (double.TryParse($"{station.longitude}", out var longi))
                    staff.Longitude = longi;
                if (double.TryParse($"{station.latitude}", out var lan))
                    staff.Latitude = lan;
                if ($"{station.faliure_status}" != String.Empty)
                    staff.FailureStatus = $"{station.faliure_status}" == "是" ? true : false;
                if (int.TryParse($"{station.available_battery_count}", out var cout))
                    staff.AvailableBatteryCount = cout;
                // var employee = _context.Employees.Find($"{station.employee_id}");
                // if (employee == null)
                // {
                //     return NewContent(1, "无该员工");
                // }
                // if (employee.Position != 1)
                // {
                //     return NewContent(2, "该员工不是管理员");
                // }

                //new_station.employees = new List<Employee> { employee };

                _context.SwitchStations.Add(staff);
                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateException e)
                {
                    String msg = e.InnerException?.Message + "";
                    if (_context.SwitchStations.Any(t => t.StationId == staff.StationId))
                        msg = "已有该id的换电站";
                    else if (_context.SwitchStations.Any(t => (t.Latitude == staff.Latitude) && (t.Longitude == staff.Longitude)))
                        msg = "已有该位置的换电站";
                    return NewContent(2, msg);
                }
                var returnMessage = new
                {
                    code = 0,
                    msg = "success"
                };
                tx.Complete();
                return Content(JsonConvert.SerializeObject(returnMessage), "application/json");
            }
        }

        [HttpDelete]
        public IActionResult DeleteStaff(string station_id)
        {

            var station = _context.SwitchStations.Find(station_id);
            if (station == null )
            {
                return NewContent(1,"找不到该station");
            }

            _context.SwitchStations.Remove(station);
            try
            {
                _context.SaveChanges();
            }
            catch(DbUpdateException e)
            {
                return NewContent(1, e.InnerException?.Message+"");
            }
            return NewContent();
        }

        private bool StaffExists(long id)
        {
            return _context.Employees?.Any(e => e.EmployeeId == id) ?? false;
        }

        private bool SwitchStationExists(long id)
        {
            return _context.SwitchStations?.Any(e => e.StationId == id) ?? false;
        }

        ContentResult NewContent(int _code=0,string _msg="success")
        {
            var a= new
            {
                code = _code,
                msg = _msg
            };
            return Content(JsonConvert.SerializeObject(a), "application/json");
        }
    }
}
