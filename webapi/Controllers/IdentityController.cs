using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Dynamic;
using EntityFramework.Context;
using EntityFramework.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Transactions;
using Idcreator;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace webapi.Controllers
{
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly ModelContext _context;

        public IdentityController(ModelContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public ActionResult LoginCheck([FromBody] dynamic _user)
        {
            dynamic user = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(_user));
            char usertype = Convert.ToString(user.user_id)[0];
            IdentityType user_type = (IdentityType)Convert.ToInt32(usertype - '0');
            string account_serial = user.user_id;
            string password = user.password;
            switch (user_type)
            {
                case IdentityType.车主:
                    var owner = _context.VehicleOwners.Where(x=>x.AccountSerial == account_serial).DefaultIfEmpty().FirstOrDefault();
                    if (owner == null)
                        return NewContent(new { }, 1, "User ID error.");
                    if (owner.Password != password)
                        return NewContent(new { }, 1, "Password error.");
                    return NewContent(
                        new
                        {
                            user_type = user_type.ToString(),
                            user_id = owner.OwnerId,
                            username = owner.Username,
                            phone_number = owner.PhoneNumber,
                            gender = owner.Gender,
                            email = owner.Email
                        });
                case IdentityType.员工:
                    var staff = _context.Employees
                        .Include(a=>a.switchStation)
                        .Where(x => x.AccountSerial == account_serial)
                        .DefaultIfEmpty().FirstOrDefault();
                    if (staff == null)
                        return NewContent(new { }, 1, "User ID error.");
                    if (staff.Password != password)
                        return NewContent(new { }, 1, "Password error.");
                    return NewContent(
                        new
                        {
                            user_type = user_type.ToString(),
                            user_id = staff.EmployeeId,
                            username = staff.UserName,
                            phone_number = staff.PhoneNumber,
                            gender = staff.Gender,
                            station_id = staff.switchStation.StationId,
                            position = (PositionEnum)staff.Position,
                            email = staff.Email
                        });
                case IdentityType.管理员:
                    var admin = _context.Administrators.Where(x => x.AccountSerial == account_serial).DefaultIfEmpty().FirstOrDefault();
                    if (admin == null)
                        return NewContent(new { }, 1, "User ID error.");
                    if (admin.Password != password)
                        return NewContent(new { }, 1, "Password error.");
                    return NewContent(
                        new
                        {
                            user_type = user_type.ToString(),
                            user_id = admin.AdminId,
                            email = admin.Email
                        });
                default:
                    return NotFound("身份无法识别");
            }
        }

        [HttpPost("sign-up")]
        public ActionResult SignupCheck([FromBody] dynamic _user)
        {
            using (TransactionScope tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                dynamic user = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(_user));
                string usertype = user.user_type ?? string.Empty;
                //可注册的用户类型：员工/车主
                if (usertype == string.Empty || usertype != "0" && usertype != "1")
                    return NoContent();
                IdentityType user_type = (IdentityType)Convert.ToInt32(usertype);
                //员工和车主共用的参数
                string username = user.username ?? "新用户";
                string password = user.password ?? "123456";
                string nickname = user.nickname ?? "蔚来";
                string gender = user.gender ?? "男";
                DateTime create_time = Convert.ToDateTime(user.create_time) ?? System.DateTime.Now.ToString();
                string phone_number = user.phone_number ?? string.Empty;
                //定义返回对象
                dynamic obj = new ExpandoObject();
                obj.data = new
                {
                    user_id = "-1"
                };
                obj.msg = "注册成功";
                if (user_type == IdentityType.车主) //注册车主
                {
                    //生成新id
                    long snake = Idcreator.EasyIDCreator.CreateId(_context);
                    long uid = Convert.ToInt64(((int)user_type).ToString() + snake.ToString());
                    obj.data = new
                    {
                        user_id = uid
                    };
                    //定义新tuple
                    VehicleOwner owner = new VehicleOwner
                    {
                        OwnerId = uid,
                        Username = username,
                        Password = password,
                        CreateTime = create_time,
                        PhoneNumber = phone_number,
                        Email = user.email ?? string.Empty,
                        Gender = gender,
                        Birthday = Convert.ToDateTime(user.birthday == null ? "2000-01-01" : user.birthday),   
                    };
                    OwnerPos OP = new OwnerPos
                    {
                        OwnerId = uid,
                        Address = user.address ?? string.Empty
                    };
                    _context.VehicleOwners.Add(owner);
                    _context.OwnerPos.Add(OP);
                    try
                    {
                        _context.SaveChanges();
                    }
                    catch (DbUpdateException)
                    {
                        return Conflict();
                    }
                }
                else if (user_type == IdentityType.员工) //注册员工
                {
                    string invite_code = user.invite_code ?? string.Empty;
                    if (invite_code != "123456")
                    {
                        obj.data = new
                        {
                            code =1 ,
                            msg = "注册码无效",
                            user_id = "-1"
                        };
                        return Content(JsonConvert.SerializeObject(obj), "application/json");
                    }
                    //生成新id
                    long snake = Idcreator.EasyIDCreator.CreateId(_context);
                    long uid = Convert.ToInt64(((int)user_type).ToString() + snake.ToString());
                    obj.data = new
                    {
                        user_id = uid
                    };
                    Employee employee = new Employee
                    {
                        EmployeeId = uid,
                        UserName = username,
                        Password = password,
                        Name = nickname,
                        CreateTime = create_time,
                        PhoneNumber = phone_number,
                        Gender = gender,
                        IdentityNumber = user.identity_number,
                        Position = user.position ?? (int)PositionEnum.其它,
                        Salary = 0
                    };
                    _context.Employees.Add(employee);
                    try
                    {
                        _context.SaveChanges();
                    }
                    catch (DbUpdateException)
                    {
                        return Conflict();
                    }
                }
                tx.Complete();
                return Content(JsonConvert.SerializeObject(obj), "application/json");
            }
        }

        private bool OwnerExists(long id)
        {
            return _context.VehicleOwners?.Any(e => e.OwnerId == id) ?? false;
        }
        private bool StaffExists(long id)
        {
            return _context.Employees?.Any(e => e.EmployeeId == id) ?? false;
        }

        ContentResult NewContent<T>(T data, int _code = 0, string _msg = "success")
        {
            var a = new
            {
                code = _code,
                msg = _msg,
                data
            };
            return Content(JsonConvert.SerializeObject(a), "application/json");
        }
    }
}
