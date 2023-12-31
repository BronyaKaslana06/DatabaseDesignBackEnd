﻿using Microsoft.AspNetCore.Http;
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
using Microsoft.AspNetCore.Authorization;

namespace webapi.Controllers.Admin
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
                .OrderBy(s => s.vo.OwnerId)
                .Select(j => new
                {
                    owner_id = j.vo.OwnerId,
                    address = string.Join(", ", j.vo.ownerpos.Select(pos => pos.Address)),
                    username = j.vo.Username,
                    gender = j.vo.Gender,
                    email = j.vo.Email,
                    phone_number = j.vo.PhoneNumber,
                    create_time = j.vo.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
                }).ToList();
                

            var totalNum = query.Count();
            var data = query.Skip(offset)
                .Take(limit)
                .ToList();
            var responseObj = new
            {
                totalData = totalNum,
                data,
            };

            return Content(JsonConvert.SerializeObject(responseObj), "application/json");
        }

        [Authorize]
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
            var pos = _context.OwnerPos.DefaultIfEmpty().FirstOrDefault();
            if (owner == null || pos == null)
            {
                return NotFound();
            }
            owner.OwnerId = num;
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

        [Authorize]
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
                long maxOwnerId = _context.VehicleOwners.Max(o => (long?)o.OwnerId) ?? 0;
                long newOwnerId = maxOwnerId + 1;
                VehicleOwner new_owner = new VehicleOwner()
                {
                    OwnerId = newOwnerId,
                    AccountSerial = '0'+EasyIDCreator.CreateId_New(_context, Idcreator.EntityType.VehicleOwner),
                    Username = "蔚来车主",
                    Password = "123456",
                    ProfilePhoto = null,
                    CreateTime = System.DateTime.Now,
                    PhoneNumber = owner.phone_number,
                    Email = "暂无",
                    Gender = owner.gender,
                    Birthday = System.DateTime.Now,
                };
                _context.VehicleOwners.Add(new_owner);
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
                Console.WriteLine("\n\n\n\n" + new_owner.OwnerId + "\n\n\n\n");
                OwnerPos new_pos = new OwnerPos()
                {
                    OwnerId = new_owner.OwnerId,
                    Address = owner.address,
                };
                _context.OwnerPos.Add(new_pos);
                Console.WriteLine("\n\n\n\n" + new_owner.OwnerId + "\n\n\n\n");
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
                tx.Complete();
                return Content(JsonConvert.SerializeObject(returnMessage), "application/json");
            }
        }

        [Authorize]
        [HttpDelete]
        public IActionResult DeleteOwner(string owner_id)
        {
            using (TransactionScope tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (_context.VehicleOwners == null)
                {
                    return NotFound();
                }
                if (!long.TryParse(owner_id, out long num))
                {
                    return NewContent(1, "id格式错误");
                }
                var owner = _context.VehicleOwners.Find(num);
                if (owner == null)
                {
                    return NewContent(1, "找不到该车主");
                }
                
                _context.VehicleOwners.Remove(owner);
                try
                {
                    _context.SaveChanges();
                    tx.Complete();
                }
                catch (DbUpdateException e)
                {
                    return NewContent(1, e.InnerException?.Message);
                }
                return NewContent();
            }
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
