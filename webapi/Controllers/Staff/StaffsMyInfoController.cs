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
    [Route("staff/my-info/")]
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
            var employee = _context.Employees.Include(a=>a.kpi).FirstOrDefault(a=>a.EmployeeId==employeeId);
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
                            phone_number = employee.PhoneNumber,
                            name = employee.Name,
                            gender = employee.Gender,
                            postions = employee.PositionEnum.ToString()
                        },
                        performance = new
                        {
                            total_performance= employee.kpi==null? -1: employee.kpi.TotalPerformance,
                            score= employee.kpi == null ? -1 : employee.kpi.Score
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
            var employee = _context.Employees.Find(employeeId);
            if (employee == null)
            {
                return NewContent(1, "无该用户");
            }
            employee.Gender = _employee.gender ?? employee.Gender;
            employee.PhoneNumber = _employee.phone_number ?? employee.PhoneNumber;
            employee.Name= _employee.name ?? employee.Name;
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

        /// <summary>
        /// lgy  员工查看个人换电记录
        /// </summary>
        /// <param name="employee_id"></param>
        /// <param name="switch_type"></param>
        /// <param name="request_status"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>     
        [HttpGet("switch-records/query")]
        public ActionResult<object> SRQuery(string employee_id, string? switch_type, string? startDate, string? endDate)
        {
            var tmp = _context.SwitchLogs.Include(e => e.employee).Include(f => f.switchrequest).Where(c => c.employee.EmployeeId == long.Parse(employee_id));
            if (!string.IsNullOrEmpty(switch_type))
            {
                if (Enum.TryParse(switch_type, out SwitchTypeEnum st_enum))
                    tmp = tmp.Where(c => c.switchrequest.SwitchTypeEnum == st_enum);
                else
                    return BadRequest("fail to convert switch_type");
            }

            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                // 定义日期时间字符串的格式化
                string format = "yyyy-MM-dd";
                // 尝试将字符串转换为 DateTime
                if (DateTime.TryParseExact(startDate, format, null, System.Globalization.DateTimeStyles.None, out DateTime result1) &&
                    DateTime.TryParseExact(endDate, format, null, System.Globalization.DateTimeStyles.None, out DateTime result2))
                {
                    tmp = tmp.Where(e => e.SwitchTime < result2 && e.SwitchTime > result1);
                }
                else
                {
                    return BadRequest("fail to convert startDate or endDate");
                }
            }


            tmp = tmp.OrderBy(e => e.SwitchTime);
            var res = tmp.Select(a => new
            {
                switch_request_id = a.SwitchServiceId,
                service_time = a.SwitchTime,
            });
            return Ok(res);

        }


        /// <summary>
        /// lgy  员工查看个人维修记录
        /// </summary>
        /// <param name="employee_id"></param>
        /// <param name="maintenance_location"></param>    //只要包含即可查出
        /// <param name="order_status"></param>      //已完成或待评分 维修项按照serviceTime排序，其他按照appointTime
        /// <param name="startDate"></param>   //已完成或待评分 维修项按照serviceTime查找，其他按照appointTime查找
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet("repair-records/query")]
        public ActionResult<object> RRQuery(string employee_id, string? maintenance_location, string? order_status,
            string? startDate, string? endDate)
        {
            var tmp = _context.MaintenanceItems.Include(a => a.employees)
                .Where(e => e.employees.Exists(f => f.EmployeeId == long.Parse(employee_id)));

            if (!string.IsNullOrEmpty(maintenance_location))
            {
                tmp = tmp.Where(e => e.MaintenanceLocation.Contains(maintenance_location));
            }

            OrderStatusEnum os_enum = OrderStatusEnum.未知;
            if (!string.IsNullOrEmpty(order_status))
            {
                if (Enum.TryParse(order_status, out os_enum))
                {
                    tmp = tmp.Where(c => c.OrderStatusEnum == os_enum);
                    if (os_enum == OrderStatusEnum.已完成 || os_enum == OrderStatusEnum.待评分)
                    {
                        tmp = tmp.OrderBy(e => e.ServiceTime).ThenBy(c => c.AppointTime);
                    }
                    else
                    {
                        tmp = tmp.OrderBy(e => e.AppointTime).ThenBy(f => f.OrderSubmissionTime);
                    }
                }
                else
                {
                    return BadRequest("fail to convert order_status");
                }
            }

            if (startDate != null && endDate != null)
            {
                // 定义日期时间字符串的格式化
                string format = "yyyy-MM-dd";
                // 尝试将字符串转换为 DateTime
                if (DateTime.TryParseExact(startDate, format, null, System.Globalization.DateTimeStyles.None, out DateTime result1) &&
                    DateTime.TryParseExact(endDate, format, null, System.Globalization.DateTimeStyles.None, out DateTime result2))
                {
                    if (os_enum == OrderStatusEnum.已完成 || os_enum == OrderStatusEnum.待评分)
                        tmp = tmp.Where(e => e.ServiceTime < result2 && e.ServiceTime > result1);
                    else if (os_enum != OrderStatusEnum.未知)
                        tmp = tmp.Where(e => e.AppointTime < result2 && e.AppointTime > result1);
                }
                else
                {
                    return BadRequest("fail to convert startDate or endDate");
                }
            }

            var res = tmp.Select(e => new
            {
                maintenance_items_id = e.MaintenanceItemId,
                maintenance_location = e.MaintenanceLocation,
                remarks = e.Note,
                evaluations = e.Evaluation,
                score = e.Score,
                order_submission_time = e.OrderSubmissionTime,
                appoint_time = e.AppointTime,
                service_time = e.ServiceTime,
                order_status = e.OrderStatusEnum
            });
            return Ok(res);
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
