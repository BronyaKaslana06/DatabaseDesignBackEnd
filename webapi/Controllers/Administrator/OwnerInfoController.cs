using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using NuGet.ContentModel;
using NuGet.Packaging.Signing;
using NuGet.Protocol;
using System.Data;
using System.Net;
using System.Reflection.Metadata;
using System.Xml.Linq;
using EntityFramework.Context;
using EntityFramework.Models;
using Idcreator;
using System.Transactions;

namespace webapi.Controllers.Administrator
{
    [Route("administrator/owner-info")]
    [ApiController]
    public class VehicleOwnerController : ControllerBase
    {
        private readonly ModelContext _context;

        public VehicleOwnerController(ModelContext context)
        {
            _context = context;
        }
        [HttpGet("message")]
        public ActionResult<IEnumerable<VehicleOwner>> GetPage(int pageIndex, int pageSize)
        {
            int offset = (pageIndex - 1) * pageSize;
            int limit = pageSize;
            if (offset < 0 || limit <= 0)
            {
                var errorResponse = new
                {
                    code = 1,
                    msg = "页码或页大小非正",
                    totalData = 0,
                    data = "",
                };
                return Content(JsonConvert.SerializeObject(errorResponse), "application/json");
            }
            var query = _context.VehicleOwners
                    .OrderBy(vo => vo.OwnerId)
                    .Select(vo => new
                    {                         
                        owner_id = vo.OwnerId.ToString(),
                        username = vo.Username,
                        gender = vo.Gender,
                        phone_number = vo.PhoneNumber,
                        address = string.Join(", ", vo.ownerpos.Select(pos => pos.Address)),
                        password = vo.Password
                    })
                    .Skip(offset)
                    .Take(limit)
                    .ToList();

            var totalNum = _context.VehicleOwners.Count();
            var responseObj = new
            {
                code = 0,
                msg = "success",
                totalData = totalNum,
                data = query,
            };
            return Content(JsonConvert.SerializeObject(responseObj), "application/json");

        }
        [HttpGet("query")]
        public ActionResult<IEnumerable<VehicleOwner>> GetPage_(int pageIndex, int pageSize, string owner_id = "", string username = "", string gender = "", string phone_number = "", string address = "", string password = "")
        {
            int offset = (pageIndex - 1) * pageSize;
            int limit = pageSize;

            if (offset < 0 || limit <= 0)
            {
                var errorResponse = new
                {
                    code = 1,
                    msg = "页码或页大小非正",
                    totalData = 0,
                    data = "",
                };
                return Content(JsonConvert.SerializeObject(errorResponse), "application/json");
            }

            var pattern1 = "%" + (string.IsNullOrEmpty(owner_id) ? "" : owner_id) + "%";
            var pattern2 = "%" + (string.IsNullOrEmpty(username) ? "" : username) + "%";
            var pattern3 = "%" + (string.IsNullOrEmpty(gender) ? "" : gender) + "%";
            var pattern4 = "%" + (string.IsNullOrEmpty(phone_number) ? "" : phone_number) + "%";
            var pattern5 = "%" + (string.IsNullOrEmpty(address) ? "" : address) + "%";
            var pattern6 = "%" + (string.IsNullOrEmpty(password) ? "" : password) + "%";

            var query = _context.VehicleOwners
                .Join(_context.OwnerPos, vo => vo.OwnerId, op => op.OwnerId, (vo, op) => new { vo, op })
                .Where(j =>
                    EF.Functions.Like(j.vo.OwnerId.ToString(), pattern1) &&
                    EF.Functions.Like(j.vo.Username, pattern2) &&
                    EF.Functions.Like(j.vo.Gender, pattern3) &&
                    EF.Functions.Like(j.vo.PhoneNumber, pattern4) &&
                    EF.Functions.Like(j.op.Address, pattern5) &&
                    EF.Functions.Like(j.vo.Password, pattern6))
                .OrderBy(j => j.vo.OwnerId)
                .Select(j => new
                {
                    owner_id = j.vo.OwnerId.ToString(),
                    address = string.Join(", ", j.vo.ownerpos.Select(pos => pos.Address)),
                    username = j.vo.Username,
                    password = j.vo.Password,
                    gender = j.vo.Gender,
                    email = j.vo.Email,
                    phone_number = j.vo.PhoneNumber
                })
                .Skip(offset)
                .Take(limit)
                .ToList();

            var totalNum = _context.VehicleOwners.Count();

            var responseObj = new
            {
                totalData = totalNum,
                data = query,
            };

            return Content(JsonConvert.SerializeObject(responseObj), "application/json");
        }

        [HttpPatch]
        public IActionResult PutOwner([FromBody] dynamic _param)
        {
            dynamic param = JsonConvert.DeserializeObject(Convert.ToString(_param));
            string owner_id = $"{param.owner_id}";
            if (!long.TryParse(owner_id, out long num))
            {
                return NewContent(1, "id格式错误");
            }
            var owner = _context.VehicleOwners.Find(num);
            var pos = _context.OwnerPos.Find(num);
            if (owner == null || pos == null)
            {
                return NotFound();
            }
            owner.OwnerId = long.Parse($"{param.owner_id}");
            owner.Gender = $"{param.gender}";
            owner.PhoneNumber = $"{param.phone_number}";
            owner.Password = $"{param.password}";
            owner.Username = $"{param.username}";
            pos.Address = $"{param.address}";
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                return NewContent(1, e.InnerException.Message);
            }

            return NewContent();
        }

        [HttpPost]
        public ActionResult<string> PostOwner([FromBody] dynamic _owner)
        {
            using (TransactionScope tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (_context.VehicleOwners == null)
                {
                    return Problem("Entity set 'ModelContext.VehicleOwner' is null.");
                }
                dynamic owner = JsonConvert.DeserializeObject(Convert.ToString(_owner));
                string sql = "SELECT MAX(owner_id) FROM EMPLOYEE";
                DataTable df = OracleHelper.SelectSql(sql);
                string sql_total = "SELECT COUNT(*) " +
                    "FROM vehicle_owner ";
                DataTable df_count = OracleHelper.SelectSql(sql_total);
                int totalNum = df_count != null ? Convert.ToInt32(df_count.Rows[0][0]) : 0;
                long uid = SnowflakeIDcreator.nextId();

                VehicleOwner new_owner = new VehicleOwner()
                {
                    OwnerId = uid,
                    Username = $"{owner.username}",
                    Password = "123456",
                    ProfilePhoto = null,
                    CreateTime = System.DateTime.Now,
                    PhoneNumber = $"{owner.phone_number}",
                    Email = "wl@car.com",
                    Gender = $"{owner.gender}",
                    Birthday = null,

                };
                OwnerPos new_pos = new OwnerPos()
                {
                    OwnerId = uid,
                    Address = $"{owner.address}"
                };


                _context.VehicleOwners.Add(new_owner);
                _context.OwnerPos.Add(new_pos);
                try
                {
                    _context.SaveChanges();
                }
                catch (DbUpdateException e)
                {
                    var a = new
                    {
                        code = 1,
                        msg = e.InnerException?.Message
                    };

                    return Conflict(a);
                }

                var returnMessage = new
                {
                    code = 0,
                    owner_id = new_owner.OwnerId.ToString(),
                    msg = "success"
                };
                return Content(JsonConvert.SerializeObject(returnMessage), "application/json");
            }
        }

        [HttpDelete]
        public IActionResult DeleteOwner(string switch_log_id)
        {
            if (_context.VehicleOwners == null)
            {
                return NotFound();
            }
            if (!long.TryParse(owner_id, out long num))
            {
                return NewContent(1, "id格式错误");
            }
            var ownerpos = _context.OwnerPos.Find(num);
            var owner = _context.VehicleOwners.Find(num);
            if (owner == null|| ownerpos == null)
            {
                return NewContent(1, "找不到该车主");
            }
            _context.OwnerPos.Remove(ownerpos);
            _context.VehicleOwners.Remove(owner);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                return NewContent(1, e.InnerException?.Message);
            }
            return NewContent();
        }

        private bool OwnerExists(string id)
        {
            return _context.VehicleOwners?.Any(e => e.OwnerId.ToString() == id) ?? false;
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
