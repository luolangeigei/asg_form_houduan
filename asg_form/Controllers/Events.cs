using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static asg_form.Controllers.excel;

namespace asg_form.Controllers
{
    public class Events:ControllerBase
    {
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        public Events(
            RoleManager<Role> roleManager, UserManager<User> userManager)
        {

            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        /// <summary>
        /// 获取所有赛事
        /// </summary>
        /// <returns></returns>
        //  [Authorize]
        
        [Route("api/v1/Events")]
        [HttpGet]
        [ResponseCache(Duration = 260)]
        public async Task<ActionResult<List<T_events>>> Getallevent()
        {
            TestDbContext testDbContext = new TestDbContext();
          var Event=  testDbContext.events.ToList();
        return Event;
        
        }
        /// <summary>
        /// 发布新赛事
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("api/v1/admin/Events")]
        [HttpPost]
        public async Task<ActionResult<List<T_events>>> Postevent(T_events events, [FromBody]string rule_markdown)
        {
            TestDbContext testDbContext = new TestDbContext();
            await testDbContext.events.AddAsync(new T_events { name = events.name, is_over = events.is_over, opentime = events.opentime ,events_rule_uri= new Uri($"https://124.223.35.239/doc/rule/{events.name}.md") });
           System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory+ $"doc/rule/{events.name}.md", rule_markdown);
            await testDbContext.SaveChangesAsync();
         
            return Ok("添加成功！");
            

        }
        /// <summary>
        /// 修改赛事
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("api/v1/admin/Events")]
        [HttpPut]
        public async Task<ActionResult<List<T_events>>> putevent(string event_name, [FromBody]T_events events)
        {

            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            { 
            
            TestDbContext testDb=new TestDbContext();
              var eve=  testDb.events.FirstOrDefault(a => a.name == event_name);
                eve.name=events.name;
                eve.opentime=events.opentime;
                eve.is_over=events.is_over;


                await testDb.SaveChangesAsync();
                return Ok(eve);
            }
            return BadRequest(new error_mb { code = 400, message = "没管理员改个P！" });
            }

            /// <summary>
            /// 删除新赛事
            /// </summary>
            /// <returns></returns>
            [Authorize]
        [Route("api/v1/admin/Events")]
        [HttpDelete]
        public async Task<ActionResult<List<T_events>>> Delevent(string event_name)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                TestDbContext test=new TestDbContext();
               var evernt= test.events.FirstOrDefault(a => a.name == event_name);
                test.Remove(evernt);
               await test.SaveChangesAsync();
                return Ok("删掉了呢");
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "没P管理员隔着装尼玛呢" });
            }

            }


        public  class T_events
        {
            public int Id { get; set; }
            public string? name { get; set; }
            public bool? is_over { get; set; }
            public DateTime? opentime { get; set; }
            public List<form>? forms { get; set; }
            public Uri? events_rule_uri { get; set; }

        }
      }
}
