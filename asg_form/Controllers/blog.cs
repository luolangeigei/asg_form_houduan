using asg_form.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static 所有队伍;

namespace asg_form;
public class blog : ControllerBase
{

    private readonly RoleManager<Role> roleManager;
    private readonly UserManager<User> userManager;
    public blog(
        RoleManager<Role> roleManager, UserManager<User> userManager)
    {

        this.roleManager = roleManager;
        this.userManager = userManager;
    }



    [Authorize]
    [Route("api/v1/blog")]
    [HttpPost]
    public async Task<ActionResult<string>> blogpost([FromBody] req_blogpost req, [FromForm]IFormCollection file)
    {
        string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var user = await userManager.FindByIdAsync(id);
        //新建一个数据库查询示例
        TestDbContext testDb = new TestDbContext();
        testDb.blogs.Add(new blog_db { pushtime = DateTime.Now, title = req.title, msg = req.msg, formuser = "luolan" });
        await testDb.SaveChangesAsync();
        return "ok";
    }

    [Route("api/v1/blog/all")]
    [HttpGet]
    public async Task<ActionResult<List<get_blogall>>> allblog(short page)
    {



        TestDbContext test = new TestDbContext();
        int a = test.blogs.Count();
        int b = 10 * page;
        if (10 * page > a)
        {
            b = a;
        }
        var blog = test.blogs.Skip(10 * page - 10).Take(b).ToList();
        List<get_blogall> get_s = new List<get_blogall>();
        foreach (var item in blog)
        {
            get_s.Add(new get_blogall { title = item.title, id = item.ID });
        }
        return get_s;
    }

    [Route("api/v1/blog/{id}")]
    [HttpPost]
    public async Task<ActionResult<blog_db>> allblog(long id)
    {
        TestDbContext test = new TestDbContext();
        blog_db blog_Db = await test.blogs.FirstOrDefaultAsync(allblog => allblog.ID == id);
        return blog_Db;
    }


    public class req_blogpost
    {
        public string title { get; set; }
        public string msg { get; set; }
        public string formuser { get; set; }
    }

    public class get_blogall
    {
        public long id { get; set; }
        public string title { get; set; }
    }

    public class blog_db
    {
        public long ID { get; set; }
        public string title { get; set; }
        public string msg { get; set; }
        public string formuser { get; set; }
        public DateTime pushtime { get; set; }

    }


}

