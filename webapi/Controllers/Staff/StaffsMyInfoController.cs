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
