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
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.FileProviders;

namespace webapi.Controllers.Admin
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
        [Authorize]
        [HttpGet("query")]
        public ActionResult<IEnumerable<Employee>> GetPage(int pageIndex,int pageSize,string? employee_id,string? username,string? gender,string? phone_number,string? salary,string? station_id,string? station_name)
        {
            int offset = (pageIndex - 1) * pageSize;
            int limit = pageSize;
            if (offset < 0 || limit <= 0)
                return BadRequest();

            var q = _context.Employees
            .Where(e => (string.IsNullOrEmpty(employee_id) || e.EmployeeId.ToString() == employee_id) &&
                (string.IsNullOrEmpty(username) || e.UserName.Contains(username)) &&
                (string.IsNullOrEmpty(gender) || e.Gender == gender) &&
                (string.IsNullOrEmpty(phone_number) || e.PhoneNumber == phone_number) &&
                (string.IsNullOrEmpty(salary) || e.Salary.ToString() == salary) &&
                (string.IsNullOrEmpty(station_id) || e.switchStation == null || e.switchStation.StationId == Convert.ToInt64(station_id)) &&
                (string.IsNullOrEmpty(station_name) || e.switchStation == null || e.switchStation.StationName.Contains(station_name))
            ).Select(f => new
            {
                employee_id = f.EmployeeId,
                username = f.UserName,
                gender = f.Gender,
                phone_number = f.PhoneNumber,
                salary = f.Salary,
                station_id = f.switchStation == null ? -1 : f.switchStation.StationId,
                station_name = f.switchStation == null ? string.Empty : f.switchStation.StationName
            })
            .OrderBy(h => h.employee_id);
            var totalData = q.Count();

            var query = q.Skip(offset).Take(limit);

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
        
        [Authorize]
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
        
        [Authorize]
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

            long snake = EasyIDCreator.CreateId(_context);
            long uid = Convert.ToInt64(IdentityType.员工.ToString() + snake.ToString());
            Employee new_employee = new Employee()
            {
                EmployeeId = uid,
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
        
        [Authorize]
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
