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
    [Route("staffs/my-info/")]
    [ApiController]
    public class StaffMyInfo : ControllerBase
    {
        private readonly ModelContext _context;

        public StaffMyInfo(ModelContext context)
        {
            _context = context;
        }

        [HttpGet("{employeeId}")]
        public ActionResult<IEnumerable<Employee>> GetOwner(long employeeId)
        {
            var employee = _context.Employees.Find(employeeId);
            var kpi = _context.Kpis.FirstOrDefault(e => e.employeeId == employeeId);
            if (employee == null)
                return NewContent(1, "id不存在");
            else
            {
                var a = new
                {
                    code = 0,
                    msg = "success",
                    data = new
                    {
                        personalInfo = new
                        {
                            username = employee.UserName,
                            phone_number = employee.PhoneNumber,
                            identify_number = employee.IdentityNumber,
                            name = employee.Name,
                            gender = employee.Gender,
                            postions = employee.PositionEnum
                        },
                        performance = new
                        {
                            total_performance=employee.kpi.TotalPerformance,
                            score=employee.kpi.Score
                        }
                    }
                };
                return Content(JsonConvert.SerializeObject(a), "application/json");
            }
        }

        [HttpPatch("{employeeId}/edit")]
        public IActionResult ChangeData(long employeeId, [FromBody] dynamic param)
        {
            dynamic _employee = JsonConvert.DeserializeObject(Convert.ToString(param));
            var employee = _context.VehicleOwners.Find(employeeId);
            if (employee == null)
            {
                return NewContent(1, "无该用户");
            }
            employee.Gender = _employee.gender ?? employee.Gender;
            employee.PhoneNumber = _employee.phone_number ?? employee.PhoneNumber;
            employee.Username = _employee.name ?? employee.Username;
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                return NewContent(2, e.InnerException?.Message + "");
            }

            return NewContent();
        }

        [HttpGet("switch-records/query")]
        public ActionResult<object> Query(string employee_id,List<string> switch_request_id,string vehicle_id,
            string switch_type,string startDate,string endDate)
        {
            var tmp = _context.SwitchLogs.Include(e => e.employee).Include(f=>f.switchrequest).Where(c => c.employee.EmployeeId == long.Parse(employee_id));
            if (switch_type != null)
            {
                if (Enum.TryParse(switch_type, out SwitchTypeEnum os_enum))
                    tmp = tmp.Where(c => c.switchrequest.SwitchTypeEnum == os_enum);
                else
                    return BadRequest("fail to convert order_status");
            }
            if (startDate != null && endDate != null) 
            {
                // 定义日期时间字符串的格式化
                string format = "yyyy-MM-dd";
                // 尝试将字符串转换为 DateTime
                if (DateTime.TryParseExact(startDate, format, null, System.Globalization.DateTimeStyles.None, out DateTime result1) &&
                    DateTime.TryParseExact(endDate, format, null, System.Globalization.DateTimeStyles.None, out DateTime result2))
                {
                    ;
                }
                else
                {
                    return BadRequest("fail to convert service_time");
                }
            }

            var res = tmp.Select(a => new
            {
                switch_request_id = a.SwitchServiceId,
                request_time = a.SwitchTime
            });
            return null;
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
