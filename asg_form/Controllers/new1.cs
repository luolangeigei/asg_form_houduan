
using Manganese.Array;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using System.Collections.Generic;

namespace asg_form.Controllers
{
    public class 查询是否重名 : ControllerBase
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 获取所有video
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/video/")]
        [HttpGet]
        public async Task<ActionResult<List<string>>> get_video()
        {
            DirectoryInfo di = new DirectoryInfo($@"{AppDomain.CurrentDomain.BaseDirectory}video");
            FileInfo[] files = di.GetFiles();
            
        List<string> result = new List<string>();
            foreach (FileInfo file in files)
            {
               result.Add(file.Name);

            }
            return result;

        }



            [Route("api/v1/Duplicate_name/")]
        [HttpGet]
        public async Task<ActionResult<string>> Post(string name,string event_name)
        {




            TestDbContext ctx = new TestDbContext();
            bool b = ctx.Forms.Include(a=>a.events).Any(a=>a.team_name==name&&a.events.name==event_name);
            if (b)
            {
                return BadRequest("Have a CHONG NAME");
            }
            else
            {
                return "No have a chong name";
            }
        }

           }
}