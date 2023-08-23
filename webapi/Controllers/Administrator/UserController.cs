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
using webapi.Models;

namespace webapi.Controllers.Administrator
{
    [Route("administrator/owner")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ModelContext _context;

        public UserController(ModelContext context)
        {
            _context = context;
        }

        [HttpGet("{ownerId}/message")]
        public ActionResult<IEnumerable<Employee>> GetOwner(int ownerId)
        {
            var owner = _context.VehicleOwners.Find(ownerId.ToString());
            if (owner == null)
                return NewContent(1, "id不存在");
            else
            {
                var a = new
                {
                    code = 0,
                    msg = "success",
                    data = owner
                };
                return Content(JsonConvert.SerializeObject(owner), "application/json");
            } 
        }

        [HttpPatch("{ownerId}/information")]
        public IActionResult ChangeData([FromBody] dynamic param)
        {
            dynamic _owner = JsonConvert.DeserializeObject(Convert.ToString(param));
            string owner_id = $"{_owner.owner_id}";
            var owner = _context.VehicleOwners.Find(owner_id);
            if (owner == null)
            {
                return NewContent(1,"无该用户");
            }
            owner.Nickname = $"{_owner.user_nickname}";
            owner.Gender = $"{_owner.gender}";
            owner.PhoneNumber = $"{_owner.phone_number}";
            owner.Address = $"{_owner.address}";
            owner.Password = $"{_owner.password}";
            //owner.Username = $"{_owner.username}";
            DateTime.TryParse(_owner.birthday,out DateTime a);
            owner.Birthday=a;
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
