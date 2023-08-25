using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using EntityFramework.Context;
using EntityFramework.Models;
using Idcreator;
using System.Transactions;
using System.Globalization;
using static System.Collections.Specialized.BitVector32;
using System.Drawing.Printing;

namespace webapi.Controllers.Administrator
{
    [Route("/owner/switch-reservation")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly ModelContext _context;

        public ReservationController(ModelContext context)
        {
            _context = context;
        }

       
    }
}
