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
using Idcreator;

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
        public ActionResult<IEnumerable<Employee>> GetPage(int pageIndex,int pageSize,string employee_id = "",string username = "",string gender = "",string phone_number = "",string salary = "",string station_id = "",string station_name = "")
        {
            int offset = (pageIndex - 1) * pageSize;
            int limit = pageSize;
            if (offset < 0 || limit <= 0)
                return BadRequest();
            if (!string.IsNullOrEmpty(employee_id) && !long.TryParse(employee_id, out long EID) ||
                !string.IsNullOrEmpty(station_id) && !long.TryParse(station_id, out long SID))
            {
                return BadRequest();
            }
            var query = _context.Employees
            .Where(e => (string.IsNullOrEmpty(employee_id) || e.EmployeeId == Convert.ToInt64(employee_id)) &&
                (string.IsNullOrEmpty(username) || e.UserName.Contains(username)) &&
                (string.IsNullOrEmpty(gender) || e.Gender == gender) &&
                (string.IsNullOrEmpty(phone_number) || e.PhoneNumber == phone_number) &&
                (string.IsNullOrEmpty(salary) || e.Salary.ToString() == salary) &&
                (string.IsNullOrEmpty(station_id) || e.switchStation.StationId == Convert.ToInt64(station_id)) &&
                (string.IsNullOrEmpty(station_name) || e.switchStation.StationName.Contains(station_name))
            ).Select(e => new
            {
                employee_id=e.EmployeeId,
                username=e.UserName,
                gender=e.Gender,
                phone_number=e.PhoneNumber,
                salary=e.Salary,
                station_id=e.switchStation.StationId,
                station_name=e.switchStation.StationName
            })
            .OrderBy(e => employee_id)
            .Skip(offset)
            .Take(limit);

            var totalData = query.Count();
            var data = query.ToList();

            if(data == null)
                return BadRequest();
            var obj = new
            {
                totalData,
                data
            };

            return Content(JsonConvert.SerializeObject(obj), "application/json");
        }

        [HttpPatch]
        public IActionResult PutStaff([FromBody] dynamic _param)
        {
            dynamic param = JsonConvert.DeserializeObject<dynamic>(_param.ToString());
            long EID = Convert.ToInt64(param.employee_id);
            long SID = Convert.ToInt64(param.station_id);

            var staff = _context.Employees.FirstOrDefault(e => e.EmployeeId == EID);

            if (staff == null)
            {
                return NotFound();
            }

            staff.PhoneNumber = param.phone_number;
            staff.Gender = param.gender;
            staff.Salary = Convert.ToInt32(param.salary);

            var switchStation = _context.SwitchStations.FirstOrDefault(s => s.StationId == SID);
            if (switchStation != null)
            {
                staff.switchStation = switchStation;
            }

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StaffExists(EID))
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

            dynamic employee = JsonConvert.DeserializeObject<dynamic>(_employee.ToString());

            long switchStationId = Convert.ToInt64(employee.station_id);
            var switchStation = _context.SwitchStations.FirstOrDefault(s => s.StationId == switchStationId);

            Employee new_employee = new Employee()
            {
                Email = employee.Email,
                UserName = employee.username,
                Password = "123456",
                CreateTime = DateTime.Now,
                PhoneNumber = employee.phone_number,
                IdentityNumber = "xxxxxxxxxxxxxxxxxx",
                Gender = employee.gender,
                Position = 3,
                Name = "佚名",
                Salary = Convert.ToInt32(employee.salary),
                switchStation = switchStation
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

            var obj = new
            {
                employee_id = new_employee.EmployeeId,
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
            bool flag = long.TryParse(employee_id, out long EID);
            if (!flag)
            {
                return BadRequest();
            }
            var staff = _context.Employees.Find(EID);
            if (staff == null)
            {
                return NotFound();
            }
            _context.SaveChanges();
            _context.Employees.Remove(staff);
            _context.SaveChanges();

            return NoContent();
        }

        private bool StaffExists(long id)
        {
            return _context.Employees?.Any(e => e.EmployeeId == id) ?? false;
        }

        private bool SwitchStationExists(long id)
        {
            return _context.SwitchStations?.Any(e => e.StationId == id) ?? false;
        }
    }
}
