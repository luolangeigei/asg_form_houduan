using Manganese.Array;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static asg_form.Controllers.提交表单;

namespace asg_form.Controllers
{
    public class Champion : ControllerBase
    {
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        public Champion(
            RoleManager<Role> roleManager, UserManager<User> userManager)
        {

            this.roleManager = roleManager;
            this.userManager = userManager;
        }



        /// <summary>
        /// 修改冠军
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("api/v1/admin/Champion/")]
        [HttpPut]
        public async Task<ActionResult<string>> Putchampion([FromBody] req_Champion req)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                TestDbContext testDb = new TestDbContext();
                var form = testDb.Forms.First(x => x.Id == req.formId);
                var events = testDb.events.First(x => x.name == req.eventname);
                var chap = testDb.Champions.First(x => x.form == form);
                chap.events = events;
                chap.msg = req.msg;
                await testDb.SaveChangesAsync();
                return Ok();
            }
            return BadRequest(new error_mb { code = 400, message = "你没有管理员呢~" });
        
        }




            [Route("api/v1/admin/Champion/")]
        [HttpPost]
        public async Task<ActionResult<string>> Postchampion([FromBody]req_Champion req)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                TestDbContext testDb = new TestDbContext();
                var form = testDb.Forms.First(x => x.Id == req.formId);
                var events = testDb.events.First(x => x.name == req.eventname);
                testDb.Champions.Add(new T_Champion { events = events, form = form ,msg=req.msg});
                await testDb.SaveChangesAsync();
                return Ok();
            }
            return BadRequest(new error_mb { code = 400, message = "你没有管理员呢~" });
        }

        /// <summary>
        /// 删除冠军
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/admin/Champion/")]
        [HttpDelete]
        public async Task<ActionResult<List<Champion.T_Champion>>> delchampion()
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                TestDbContext testDb = new TestDbContext();
                var chaps = testDb.Champions.ToList();
                
                testDb.Champions.RemoveRange(chaps);
                await testDb.SaveChangesAsync();
                return Ok();
            }
            return BadRequest(new error_mb { code = 400, message = "你没有管理员呢~" });
        }


            [Route("api/v1/Champion/")]
        [HttpGet]
        public async Task<ActionResult<List<Champion.T_Champion>>> getchampion()
        {

            TestDbContext testDbContext = new TestDbContext();
            var chaps = testDbContext.Champions.Include(a => a.form.role).Include(a => a.events).ToList();
            foreach (var chap in chaps)
            {
                foreach (var item in chap.form.role)
                {
                    item.form = null;
                }
                chap.events.forms = null;
            }
            return chaps;
        }




        public class T_Champion
        {
            public long Id { get; set; }
            public form form { get; set; }
            public Events.T_events events { get; set; }
            public string msg { get; set; }

        }

        public class req_Champion
        {
            public long formId { get; set; }
            public string eventname { get; set; }
            public string msg { get; set; }
        }
    }
}
