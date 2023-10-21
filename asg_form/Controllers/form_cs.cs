
using asg_form.Controllers.Hubs;
using Manganese.Text;
using Masuit.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NLog;
using NPOI.OpenXmlFormats.Spreadsheet;
using RestSharp;
using SixLabors.ImageSharp;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using static asg_form.Controllers.excel;
using static 所有队伍;

namespace asg_form.Controllers
{




    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };


        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

    }

    // [Route("api/updateform/")]
    public class 提交表单 : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        public static void WriteFile(String str)
        {
            StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "/allteam.txt", true, System.Text.Encoding.Default);
            sw.WriteLine(str);
            sw.Close();
        }
        /// <summary>
        /// 点赞表单
        /// </summary>
        /// <param name="name">队伍名称</param>
        /// <param name="captoken">谷歌验证码token</param>
        /// <returns></returns>
        [Route("api/v1/form/like/")]
        [HttpPost]
        public async Task<ActionResult<like>> Post(string name, string captoken)
        {
            var client = new RestClient($"https://www.recaptcha.net/recaptcha/api/siteverify?secret=6LcdXUEmAAAAAJLICuxBgtMsDiMSCm5XpB0z-fzK&response={captoken}");

            var request = new RestRequest(Method.POST);
            IRestResponse response = client.Execute(request);
            string a = response.Content;
            JObject d = a.ToJObject();
            string ok = d["success"].ToString();


            if (ok == "True")
            {
                try
                {
                    using TestDbContext ctx = new TestDbContext();
                    var b = ctx.Forms.Single(b => b.team_name == name);
                    b.piaoshu = b.piaoshu + 1;
                    await ctx.SaveChangesAsync();
                    return new like { Number = b.piaoshu };
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "人机验证未通过" });
               
            }

        }



        public class like
        {
            public int Number { get; set; }
        }

      
        /// <summary>
        /// 修改问卷
        /// </summary>
        /// <param name="password">密码</param>
        /// <param name="formid">队伍名称</param>
        /// <param name="for1">队伍信息</param>
        /// <returns></returns>
        [Route("api/v1/form/")]
        [HttpPut]
        public async Task<ActionResult<string>> updateform(string password,string formname,[FromBody] form_get for1)
        {
            TestDbContext ctx = new TestDbContext();
            var form = ctx.Forms.Include(a => a.role).FirstOrDefault(a => a.team_name==formname);
            if (form.team_password == password)
            {
              
            List<role> role = new List<role>();
            foreach (role_get a in for1.role_get)
            {
                role.Add(new role { role_id = a.role_id, role_lin = a.role_lin, role_name = a.role_name });
            }
            form.role = role;

            await ctx.SaveChangesAsync();
               
            return Ok("成功！");
        }
            else
            {
                return BadRequest(new error_mb { code=400,message="密码错误"});
    }
}
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IHubContext<room>? hubContext;
        


        [Route("api/v1/websocket/")]
        [HttpGet]
        public async  Task  GetAsync()
        {
         await hubContext.Clients.All.SendAsync("formok", $"测试，ok");


        }


        /// <summary>
        /// 提交表单
        /// </summary>
        /// <param name="for1">表单信息</param>
        /// <param name="captoken">谷歌人机验证验证码</param>
        /// <returns></returns>
        [Route("api/v1/form/")]
        [HttpPost]
        public async Task<ActionResult<string>> PostAsync([FromBody] form_get for1,string captoken)
        {

                var client = new RestClient($"https://www.recaptcha.net/recaptcha/api/siteverify?secret=6LcdXUEmAAAAAJLICuxBgtMsDiMSCm5XpB0z-fzK&response={captoken}");

                var request = new RestRequest(Method.POST);
                IRestResponse response = client.Execute(request);
                string tokenjson = response.Content;
                JObject d = tokenjson.ToJObject();
                string ok = d["success"].ToString();


                if (ok == "True")
                {
         
                    TestDbContext ctx = new TestDbContext();
                    if (ctx.Forms.Include(a=>a.events).Where(a=>a.events.name==for1.events_name).Any(e => e.team_name == for1.team_name))
                    {
                    return BadRequest(new error_mb { code = 400, message = "有重名队伍" });
                }
                else
                    {
                        base64toimg(for1.logo_base64, $@"{AppDomain.CurrentDomain.BaseDirectory}loge\{for1.events_name}\{for1.team_name}.png");
                  var events= await ctx.events.FirstAsync(ctx => ctx.name == for1.events_name);


                        form form1 = new form();
                        form1.logo_uri = $"https://124.223.35.239/loge/{for1.events_name}/{for1.team_name}.png";
                        form1.team_name = for1.team_name;
                        form1.team_password = for1.team_password;
                        form1.team_tel = for1.team_tel;
                        form1.events = events;
                        List<role> role = new List<role>();
                        foreach (role_get a in for1.role_get)
                        {
                            role.Add(new role { role_id = a.role_id, role_lin = a.role_lin, role_name = a.role_name });
                        }
                        form1.role = role;

                        ctx.Forms.Add(form1);
                        await ctx.SaveChangesAsync();
                    int nownumber = ctx.Forms.Count();
                    //ChatRoomHub chat = new ChatRoomHub();
                    // await chat.formok(nownumber, for1.team_name);
                    try
                    {
                        await hubContext.Clients.All.SendAsync("formok", $"队伍{for1.team_name}已经成功报名，剩余队伍名额：{ctx.Forms.Count()}/32");

                    }
                    catch
                    {


                    }
                    logger.Info($"有新队伍报名！队伍名称：{for1.team_name} ");
                    }


                    return "ok!";

                }
                else
                {

                return BadRequest(new error_mb { code = 400, message = "人机验证未通过" });

            }


        }




     public void base64toimg(string base64,string path)
        {

            // 移除base64字符串开头的data URI标识部分（例如：data:image/png;base64）
            if (base64.Contains(","))
                base64 = base64.Split(',')[1];

            byte[] imageBytes = Convert.FromBase64String(base64);

            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms);

                // 设置保存路径和文件名
                string savePath = path;

                // 根据文件扩展名决定图像格式
                image.Save(savePath);
            }


        }



        /// <summary>
        /// 获得所有表单信息
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="page_long">每页长度</param>
        /// <returns></returns>
     
        [Route("api/v1/form/all")]
        [HttpGet]
        [Authorize]
        public List<team> Getform(short page,short page_long,string sort,string eventsname)
        {
            TestDbContext ctx = new TestDbContext();


            int c = ctx.Forms.Count();
            int b = page_long * page;
            if (page_long * page > c)
            {
                b = c;
            }
          var events=  ctx.events.First(ctx => ctx.name == eventsname);
           List<form> forms;
            if(sort=="vote")
            {
                forms = ctx.Forms.Include(a => a.role).Include(a=>a.events).OrderByDescending(a => a.piaoshu).Where(a=>a.events==events).Skip(page_long * page - page_long).Take(page_long).ToList();
            }
            else
            {
                //改为按照id倒序排序
              //forms = ctx.Forms.Include(a => a.role).Skip(page_long * page - page_long).Take(page_long).ToList();
              forms = ctx.Forms.Include(a => a.role).Include(a => a.events).OrderByDescending(a => a.Id).Where(a => a.events == events).Skip(page_long * page - page_long).Take(page_long).ToList();

            }
            List<team> teams = new List<team>();


            foreach (form for1 in forms)
            {
                var team = new team { name = for1.team_name, timer = for1.time, piaoshu = for1.piaoshu ,logo_uri=for1.logo_uri};
                foreach (var role in for1.role)
                {
                    team.rolename.Add(new roletwo { name = role.role_name, lin = role.role_lin });
                }
                teams.Add(team);
               // a++;
            }
            return teams;
        }
















            /// <summary>
            /// 搜索表单
            /// </summary>
            /// <param name="team_name">表单名称</param>
            /// <returns></returns>
            [Route("api/v1/form/{team_name}")]
        [HttpGet]
        public async Task<ActionResult<List<allteam>>> formsearch(string team_name)
        {
            TestDbContext ctx = new TestDbContext();
            List<allteam> data = new List<allteam>();
            List<form> teams = ctx.Forms.Include(a => a.role).Where(a => a.team_name.IndexOf(team_name) >= 0).ToList();
            foreach (var team in teams)
            {
                var roles = team.role;
                allteam allteam = new allteam();
                allteam.Id = team.Id;
                allteam.Name = team.team_name;
                foreach (var role in roles)
                {
                    role.form = null;
                    allteam.role.Add(role);
                }
                data.Add(allteam);
            }
            return data;


        }
        /// <summary>
        /// 模糊搜索表单名称
        /// </summary>
        /// <param name="team_name">表单名称</param>
        /// <returns></returns>
        [Route("api/v1/form/name/{team_name}")]
        [HttpGet]
        public async Task<ActionResult<List<string>>> search_name(string team_name,string events_name)
        {
            var ctx = new TestDbContext();
            var data = ctx.Forms.Where(a => a.team_name.IndexOf(team_name) >= 0&&a.events.name==events_name).Select(a => a.team_name).ToList();
            return data;
        }

    }
  


    public class role
    {
        public long Id { get; set; }

        public form form { get; set; }//属于哪个队伍

        public string role_id { get; set; } = "无";
        public string role_name { get; set; } = "无";//阵容
        public string role_lin { get; set; }
    }

    public class role_get
    {
        public string role_id { get; set; } = "无";
        public string role_name { get; set; } = "无";//阵容
        public string role_lin { get; set; } = "无";
    }
    public class form
    {
        public long Id { get; set; }
        public int piaoshu { get; set; }
        public DateTime time { get; set; } = DateTime.Now;
        public string team_name { get; set; }
        public string team_password { get; set; }
        public string team_tel { get; set; }
        public string logo_uri { get; set; }
        public Events.T_events events { get; set; }
      //  public string? belong { get; set; }
        public List<role> role { get; set; } = new List<role>();
    }

    public class form_get
    {

        public DateTime time { get; set; } = DateTime.Now;
        public string team_name { get; set; }
        public string team_password { get; set; }
        public string team_tel { get; set; }
        public string logo_base64 { get; set; }
        public string events_name { get; set; }
      //  public string? belong { get; set; }
        public List<role_get> role_get { get; set; }
    }




    public class form1
    {

        public long Id { get; set; }
        public string team_name { get; set; }
        public string team_password { get; set; }
        public string team_tel { get; set; }


        public role[] role1 { get; set; }

        public string loge_base64 { get; set; } = "null";
    }

}












public class 所有队伍 : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
   




    public class team
    {
        public string name { get; set; }
        public DateTime timer { get; set; }
        public int piaoshu { get; set; }
        public string logo_uri { get; set; }
        public List<roletwo> rolename { get; set; }=new List<roletwo>();
    }

    public class roletwo
    {
        public string name { get; set; }
        public string lin { get; set; }
    }












    public class form_uri
    {
        public string uri { get; set; }
    }



}



















