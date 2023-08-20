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

namespace webapi.Controllers.Administrator
{
    [Route("administrator/staff-info")]
    [ApiController]
    public class StaffInfoController : ControllerBase
    {
        private readonly ModelContext _context;

        public StaffInfoController(ModelContext context)
        {
            _context = context;
        }

        [HttpGet("query")]
        public ActionResult<IEnumerable<Employee>> GetPage_(int pageIndex, int pageSize, string employee_id="", string username = "", string gender = "", string phone_number = "", string salary = "", string station_id = "", string station_name="")
        {
            int offset = (pageIndex - 1) * pageSize;
            int limit = pageSize;
            if (offset < 0 || limit <= 0)
                return BadRequest();

            string pattern1 = employee_id == "" ? "" : " AND T0.employee_id = '" + employee_id;
            string pattern2 = username == "" ? "" : " AND T0.username like '%" + username + "%'";
            string pattern3 = gender == "" ? "" : " AND T0.gender = '" + gender + "'";
            string pattern4 = phone_number == "" ? " " : " AND T0.phone_number = '" + phone_number + "'";
            string pattern5 = salary == "" ? "" : " AND T0.salary = '" + salary.ToString() + "'";
            string pattern6 = station_id == "" ? "" : " AND T1.station_id = '" + station_id + "'";
            string pattern7 = station_name == "" ? "" : " AND T2.station_name like '%" + station_name + "%'";

            string where_cause = "WHERE T0.gender like '%'" +
                pattern1 + pattern2 + pattern3 + pattern4 + pattern5 + pattern6 + pattern7;
            string sql_info = "SELECT T0.employee_id,T0.username,T0.phone_number,T0.gender,T0.salary,T1.station_id,T2.station_name " +
                "FROM EMPLOYEE T0 " +
                "LEFT JOIN EMPLOYEE_SWITCH_STATION T1 " +
                "ON T0.employee_id=T1.employee_id " +
                "LEFT JOIN SWITCH_STATION T2 " +
                "on T1.station_id=T2.station_id " + where_cause +
                "ORDER BY T0.employee_id";
            DataTable df = OracleHelper.SelectSql(sql_info);
            var obj = new
            {
                totalData = df != null ? df.Rows.Count : 0,
                data = df?.AsEnumerable().Skip(offset).Take(limit).CopyToDataTable<DataRow>(),
            };
            return Content(JsonConvert.SerializeObject(obj), "application/json");
        }

        [HttpPatch]
        public IActionResult PutStaff([FromBody] dynamic _param)
        {
            dynamic param = JsonConvert.DeserializeObject(Convert.ToString(_param));
            string employee_id = $"{param.employee_id}";
            var staff = _context.Employees.Find(employee_id);
            var staff_station = _context.EmployeeSwitchStations.Find(employee_id);
            if (staff == null || staff_station == null)
            {
                return NotFound();
            }
            
            staff.PhoneNumber = $"{param.phone_number}";
            staff.Gender = $"{param.gender}";
            staff.Salary = Convert.ToDecimal(param.salary);
            staff_station.StationId = $"{param.station_id}";
            Console.WriteLine(_context.Employees);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StaffExists(employee_id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public ActionResult<string> PostStaff([FromBody] dynamic _employee)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'ModelContext.Employee' is null.");
            }
            dynamic employee = JsonConvert.DeserializeObject(Convert.ToString(_employee));
            string sql = "SELECT MAX(employee_id) FROM EMPLOYEE";
            DataTable df = OracleHelper.SelectSql(sql);
            int df_count = Convert.IsDBNull(df.Rows[0][0]) ? 1000000: Convert.ToInt32(df.Rows[0][0]) + 1;
            string uid = df_count.ToString("D7");

            Employee new_employee = new Employee()
            {
                EmployeeId = uid,
                Username = $"{employee.username}",
                Password = "123456",
                CreateTime = System.DateTime.Now,
                PhoneNumber = $"{employee.phone_number}",
                IdentityNumber = "xxxxxxxxxxxxxxxxxx",
                Gender = $"{employee.gender}",
                Positions = "新人",
                Name = "佚名",
                Salary = Convert.ToDecimal(employee.salary)
            };

            EmployeeSwitchStation new_relation = new EmployeeSwitchStation()
            {
                EmployeeId = uid,
                StationId = $"{employee.station_id}"
            };

            _context.Employees.Add(new_employee);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (StaffExists(new_employee.EmployeeId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            _context.EmployeeSwitchStations.Add(new_relation);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (StaffInSwitchStationExists(new_relation.EmployeeId) || SwitchStationExists(new_relation.StationId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            var obj = new
            {
                employee_id = uid,
            };
            return Content(JsonConvert.SerializeObject(obj), "application/json");
        }

        [HttpDelete]
        public IActionResult DeleteStaff(string employee_id)
        {
            if (_context.Employees == null)
            {
                return NotFound();
            }

            var staff = _context.Employees.Find(employee_id);
            var staff_station = _context.EmployeeSwitchStations.Find(employee_id);
            if (staff == null || staff_station == null)
            {
                return NotFound();
            }
            _context.EmployeeSwitchStations.Remove(staff_station);
            _context.SaveChanges();
            _context.Employees.Remove(staff);
            _context.SaveChanges();

            return NoContent();
        }

        private bool StaffExists(string id)
        {
            return _context.Employees?.Any(e => e.EmployeeId == id) ?? false;
        }

        private bool StaffInSwitchStationExists(string id)
        {
            return _context.EmployeeSwitchStations?.Any(e => e.EmployeeId == id) ?? false;
        }

        private bool SwitchStationExists(string id)
        {
            return _context.SwitchStations?.Any(e => e.StationId == id) ?? false;
        }
    }
}
