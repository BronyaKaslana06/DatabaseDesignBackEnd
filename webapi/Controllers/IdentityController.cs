﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Dynamic;
using webapi.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            dynamic user = JsonConvert.DeserializeObject(Convert.ToString(_user));
            string user_id  = $"{user.user_id}";
            string password = $"{user.password}";
            string table_name = "vehicle_owner";
            string user_id_type = "owner_id";
            switch (user_id[0])
            {
                case '2':
                    table_name = "administrator";
                    user_id_type = "admin_id";
                    break;
                case '1':
                    table_name = "employee";
                    user_id_type = "employee_id";
                    break;
                case '0':
                    table_name = "vehicle_owner";
                    user_id_type = "owner_id";
                    break;
                default:
                    break;
            }

            string isExist = "SELECT * FROM " + table_name + " WHERE " + user_id_type + "='" + user_id + "'";
            DataTable exist = OracleHelper.SelectSql(isExist);

            dynamic obj = new ExpandoObject();
            obj.user_type = user_id[0];
            obj.code = 0;
            if (exist.Rows.Count == 0)
            {
                obj.code = 1;
                obj.msg = "用户id不存在";
                obj.user_info = new { };
            }
            else
            {
                string isRightPassword = "SELECT * FROM " + table_name +
                    " WHERE " + user_id_type + "='" + user_id +
                    "' AND password='" + password + "'";
                DataTable right = OracleHelper.SelectSql(isRightPassword);
                if (right.Rows.Count==0)
                {
                    obj.code = 2;
                    obj.msg = "密码错误";
                    obj.user_info = new { };
                }
                else
                {
                    obj.msg = "登陆成功";
                    obj.user_info = right;
                }
            }
            return Content(JsonConvert.SerializeObject(obj), "application/json");
        }

        [HttpPost("signin")]
        public ActionResult SigninCheck([FromBody] dynamic _user)
        {
            dynamic user = JsonConvert.DeserializeObject(Convert.ToString(_user));
            string user_type = $"{user.user_type}";
            //可注册的用户类型：员工/车主
            if (user_type == string.Empty || user_type != "0" && user_type != "1")
                return NoContent();
            //员工和车主共用的参数
            string username = $"{user.username}";
            string password = $"{user.password}";
            string nickname = $"{user.nickname}";
            DateTime create_time = Convert.ToDateTime($"{user.create_time}");
            string phone_number = $"{user.phone_number}";
            //定义返回对象
            dynamic obj = new ExpandoObject();
            obj.user_id = "-1";
            obj.msg = "注册成功";
            if (user_type == "0") //注册车主
            {
                //生成新id
                string sql = "SELECT MAX(owner_id) FROM VEHICLE_OWNER";
                DataTable df = OracleHelper.SelectSql(sql);
                int df_count = Convert.IsDBNull(df.Rows[0][0]) ? 0000000 : Convert.ToInt32(df.Rows[0][0]) + 1;
                string uid = df_count.ToString("D7");
                obj.user_id = uid;
                //定义新tuple
                VehicleOwner owner = new VehicleOwner
                {
                    OwnerId = uid,
                    Username = username,
                    Password = password,
                    Nickname = nickname,
                    CreateTime = create_time,
                    PhoneNumber = phone_number,
                    Email = $"{user.email}",
                    Gender = $"{user.gender}",
                    Birthday = Convert.ToDateTime($"{user.birthday}"),
                    Address = $"{user.address}"
                };
                _context.VehicleOwners.Add(owner);
                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    if (OwnerExists(owner.OwnerId))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else if(user_type == "1") //注册员工
            {
                string sql = "SELECT MAX(employee_id) FROM EMPLOYEE";
                DataTable df = OracleHelper.SelectSql(sql);
                int df_count = Convert.IsDBNull(df.Rows[0][0]) ? 1000000 : Convert.ToInt32(df.Rows[0][0]) + 1;
                string uid = df_count.ToString("D7");
                obj.user_id = uid;
                Employee employee = new Employee
                {
                    EmployeeId = uid,
                    Username = username,
                    Password = password,
                    Name = nickname,
                    CreateTime = create_time,
                    PhoneNumber = phone_number,
                    Gender = $"{user.gender}",
                    IdentityNumber = $"{user.identity_number}",
                    Positions = $"{user.position}"
                };
                _context.Employees.Add(employee);
                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    if (StaffExists(employee.EmployeeId))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return Content(JsonConvert.SerializeObject(obj), "application/json");
        }

        private bool OwnerExists(string id)
        {
            return _context.VehicleOwners?.Any(e => e.OwnerId == id) ?? false;
        }
        private bool StaffExists(string id)
        {
            return _context.Employees?.Any(e => e.EmployeeId == id) ?? false;
        }
    }
}