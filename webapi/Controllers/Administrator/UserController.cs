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
    [Route("owner")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ModelContext _context;

        public UserController(ModelContext context)
        {
            _context = context;
        }

        [HttpGet("{ownerId}/message")]
        public ActionResult<IEnumerable<Employee>> GetOwner(long ownerId)
        {
            var owner = _context.VehicleOwners.Find((ownerId));
            if (owner == null)
                return NewContent(1, "id不存在");
            else
            {
                var a = new
                {
                    code = 0,
                    msg = "success",
                    data = new
                    {
                        user_name = owner.Username,
                        user_nickname = "数据库更新，无此项",
                        gender = owner.Gender,
                        birthday = owner.Birthday,
                        address = owner.ownerpos.ToArray(),
                        phone_number = owner.PhoneNumber,
                        password=owner.Password,
                        email = owner.Email
                    }
                };
                return Content(JsonConvert.SerializeObject(a), "application/json");
            } 
        }

        [HttpPatch("{ownerId}/information")]
        public IActionResult ChangeData(long ownerId,[FromBody] dynamic param)
        {
            dynamic _owner = JsonConvert.DeserializeObject(Convert.ToString(param));
            var owner = _context.VehicleOwners.Find(ownerId);
            if (owner == null)
            {
                return NewContent(1,"无该用户");
            }
            owner.Gender = _owner.gender?? owner.Gender;
            owner.PhoneNumber = _owner.phone_number ?? owner.PhoneNumber;
            owner.Password = _owner.password ?? owner.Password;
            owner.Username = _owner.user_name?? owner.Username;
            owner.Email = _owner.email ?? owner.Email;
            if(DateTime.TryParse (_owner.birethday,out DateTime b))
                owner.Birthday=b;
            Console.Write(owner);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                return NewContent(2, e.InnerException?.Message+"");
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
