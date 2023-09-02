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

namespace webapi.Controllers.Owner
{
    [Route("")]
    [ApiController]
    public class OwnerBoardController : ControllerBase
    {
        private readonly ModelContext _context;

        public OwnerBoardController(ModelContext context)
        {
            _context = context;
        }

        
    }
}
