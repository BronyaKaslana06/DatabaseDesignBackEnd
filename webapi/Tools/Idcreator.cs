using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Numerics;
using EntityFramework.Context;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
namespace Idcreator
{
    public class SnowflakeIDcreator
    {
        private static long workerId = 1; //机器ID
        private static long twepoch = 68020L; //唯一时间，这是一个避免重复的随机量，自行设定不要大于当前时间戳
        private static long sequence = 0L;
        private static int workerIdBits = 4; //机器码字节数。4个字节用来保存机器码(定义为Long类型会出现，最大偏移64位，所以左移64位没有意义)
        private static long maxWorkerId = -1L ^ -1L << workerIdBits; //最大机器ID
        private static int sequenceBits = 10; //计数器字节数，10个字节用来保存计数码
        private static int workerIdShift = sequenceBits; //机器码数据左移位数，就是后面计数器占用的位数
        private static int timestampLeftShift = sequenceBits + workerIdBits; //时间戳左移动位数就是机器码和计数器总字节数
        private static long sequenceMask = -1L ^ -1L << sequenceBits; //一微秒内可以产生计数，如果达到该值则等到下一微妙在进行生成
        private static long lastTimestamp = -1L;
        private static object lockObj = new object();

        /// <summary>
        /// 设置机器码
        /// </summary>
        /// <param name="id">机器码</param>
        public static void SetWorkerID(long id)
        {
            SnowflakeIDcreator.workerId = id;
        }

        public static long nextId()
        {
            lock (lockObj)
            {
                long timestamp = timeGen();
                if (lastTimestamp == timestamp)
                { //同一微妙中生成ID
                    SnowflakeIDcreator.sequence = (SnowflakeIDcreator.sequence + 1) & SnowflakeIDcreator.sequenceMask; //用&运算计算该微秒内产生的计数是否已经到达上限
                    if (SnowflakeIDcreator.sequence == 0)
                    {
                        //一微妙内产生的ID计数已达上限，等待下一微妙
                        timestamp = tillNextMillis(lastTimestamp);
                    }
                }
                else
                { //不同微秒生成ID
                    SnowflakeIDcreator.sequence = 0; //计数清0
                }
                if (timestamp < lastTimestamp)
                { //如果当前时间戳比上一次生成ID时时间戳还小，抛出异常，因为不能保证现在生成的ID之前没有生成过
                    throw new Exception(string.Format("Clock moved backwards.  Refusing to generate id for {0} milliseconds",
                        lastTimestamp - timestamp));
                }
                lastTimestamp = timestamp; //把当前时间戳保存为最后生成ID的时间戳
                long nextId = (timestamp - twepoch << timestampLeftShift) | SnowflakeIDcreator.workerId << SnowflakeIDcreator.workerIdShift | SnowflakeIDcreator.sequence;
                return nextId;
            }
        }

        /// <summary>
        /// 获取下一微秒时间戳
        /// </summary>
        /// <param name="lastTimestamp"></param>
        /// <returns></returns>
        private static long tillNextMillis(long lastTimestamp)
        {
            long timestamp = timeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = timeGen();
            }
            return timestamp;
        }

        /// <summary>
        /// 生成当前时间戳
        /// </summary>
        /// <returns></returns>
        private static long timeGen()
        {
            return (long)(DateTime.UtcNow -  DateTime.Today).TotalMilliseconds;
        }
    }


    public class EasyIDCreator: IPrincipalAccessor
    {
        static List<long>? allIds = null;
        static Random rand=null;
        private static ModelContext _context;
        public EasyIDCreator(IHttpContextAccessor httpContextAccessor)
        {
            var _httpContextAccessor = httpContextAccessor;
            _context = httpContextAccessor.HttpContext.RequestServices.GetService<ModelContext>();
        }
        static ModelContext GetContext([FromServices] ModelContext modelContext)
        {
            return modelContext;
        }
        public static long CreateId(ModelContext context)
        {
            if (_context == null)
            { 
                rand=new Random();
                _context = context;
                allIds = new List<long>();
                allIds=
                _context.Administrators.Select(a => a.AdminId).ToList().Union(
                _context.Employees.Select(a => a.EmployeeId).ToList()).ToList().Union(
                _context.VehicleOwners.Select(a=>a.OwnerId).ToList()).ToList();
            }

            if (allIds == null)
                return SnowflakeIDcreator.nextId();

            while (true)
            {
                long a = rand.NextInt64(1000000000, 9999999999);
                if (allIds.Any(b=>b==a)==false)
                {
                    allIds.Add(a);
                    return a;
                }
            }
        } 
    }
    public interface IPrincipalAccessor
    {
    }
}
