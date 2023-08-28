using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using EntityFramework.Context;
using EntityFramework.Models;
using Idcreator;

namespace webapi.Controllers.Administrator
{
    [Route("administrator/announcement")]
    [ApiController]
    public class AnnouncementController : ControllerBase
    {
        private readonly ModelContext _context;

        public AnnouncementController(ModelContext context)
        {
            _context = context;
        }

        [HttpGet("message")]
        public ActionResult<IEnumerable<Employee>> GetPage_()
        {
            var a = new
            {
                announcementArray = OracleHelper.SelectSql("select * from News"),
            };
            return Content(JsonConvert.SerializeObject(a), "application/json");
        }
        [HttpGet("query")]
        public ActionResult Query(string title="",string publisher="",string publish_time="",string publish_pos="")
        {
            DateTime? date=null;
            long? id = null;
            if(DateTime.TryParse(publish_time, out DateTime b))
                date=b.Date;
            if (long.TryParse(publisher, out var _id))
                id = _id;
            var c=_context.News.Where(a =>
            (a.Title==null? title=="" : a.Title.Contains(title))  &&
            (id==null? publisher=="": a.administrator.AdminId==id)&&
            (a.PublishPos==null? publish_pos == "" : a.PublishPos.Contains(publish_pos)) &&
            (date==null? publish_time=="":a.PublishTime.Date == date)
            );
            var a = new
            {
                code = 0,
                msg = "success",
                announcementArray = c
            };
            return Content(JsonConvert.SerializeObject(a), "application/json");
        }
        [HttpPost]
        public IActionResult PotAnnouncement([FromBody] dynamic _acm)
        {
            _acm = JsonConvert.DeserializeObject(Convert.ToString(_acm));

            long id = SnowflakeIDcreator.nextId();
            var acm = new News()
            {
                AnnouncementId = id,
                Contents = _acm.contents,
                PublishTime = DateTime.Now,
                Title = _acm.title,
                PublishPos = _acm.publish_Pos
            };
            _context.Add(acm);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                return NewContent(1, e.InnerException?.Message + "");
            }

            return NewContent(0, "success");
        }
        [HttpPatch]
        public IActionResult PatAnnouncement([FromBody] dynamic _acm)
        {
            _acm = JsonConvert.DeserializeObject(Convert.ToString(_acm));
            
            bool flag = long.TryParse($"{_acm.announcement_id}", out long id);
            if(!flag)
                return NewContent(1, "id无效");
            
            var acm = _context.News.Find(id);

            if(acm==null)
                return NewContent(1,"无该id的公告");
            else
            {
                acm.Contents = _acm.contents;
                //acm.PublishTime = Convert.ToDateTime(_acm.publish_time);
                acm.Title = _acm.title;
                acm.PublishPos = _acm.publish_Pos;
            }
            try{
                _context.SaveChanges();
            }
            catch(DbUpdateException e)
            {
                return NewContent(1, e.InnerException?.Message+"");
            }
            
            return NewContent(0,"success");
        }
        [HttpDelete]
        public IActionResult DelAnnouncement([FromBody] dynamic _acm)
        {
            _acm = JsonConvert.DeserializeObject(Convert.ToString(_acm));
            string id = $"{_acm.announcement_id}";
            if (id == null)
                return NewContent(1, "请输入id");
            var acm = _context.News.Find(id);
            if(acm==null)
            {
                return NewContent(1, "无此id的公告");
            }
            else
            {
                _context.News.Remove(acm);
                try{
                    _context.SaveChanges();
                }
                catch(DbUpdateException e)
                {
                    return NewContent(1, e.InnerException?.Message + "");
                }
                return NewContent(0,"success");
            }
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