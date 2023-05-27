using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace asg_form.Controllers
{
    public class news : ControllerBase
    {
        private readonly ILogger<news> logger;
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        public news(ILogger<news> logger,
            RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            this.logger = logger;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }



        [Authorize]
        [Route("api/getadmin/")]
        [HttpPost]
        public async Task<ActionResult<string>> Post(string password)
        {
            if (password == "luolanzuishuai")
            {
                string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                var user = await userManager.FindByIdAsync(id);
                try
                {
                    await roleManager.CreateAsync(new Role { Name = "admin", msg = "q" });

                }
                catch
                {

                }
                await userManager.AddToRoleAsync(user, "admin");
                return "ok";
            }
            else
            {
                return BadRequest("无权访问！");

            }
        }



        [Route("api/getallnews/")]
        [HttpPost]
        public async Task<ActionResult<List<T_news>>> getnews()
        {
            TestDbContext ctx = new TestDbContext();
            return ctx.news.Where(a => a.Id >= 0).ToList();

        }



        [Authorize]
        [Route("api/updatenews/")]
        [HttpPost]
        public async Task<ActionResult<string>> Post(string title, string msg)
        {
            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);



            bool a = await userManager.IsInRoleAsync(user, "admin");
            if (a)
            {
                TestDbContext ctx = new TestDbContext();
                await ctx.news.AddAsync(new T_news { Title = title, msg = msg, FormName = user.UserName });
                await ctx.SaveChangesAsync();
                return "ok!";
            }
            else
            {
                return "无权访问";
            }

        }

    }

    public class T_news
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string FormName { get; set; }

        public string msg { get; set; }
    }
}