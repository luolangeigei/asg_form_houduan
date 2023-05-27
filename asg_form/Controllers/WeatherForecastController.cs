using asg_form.Controllers;
using Manganese.Array;
using Manganese.Text;
using Masuit.Tools;
using Masuit.Tools.Files;
using Microsoft.AspNetCore.Mvc;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions;
using Mirai.Net.Sessions.Http.Managers;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Reactive.Linq;

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

        [Route("api/like/")]
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
                return BadRequest("验证码未通过" + ok);
            }

        }



        public class like
        {
            public int Number { get; set; }
        }

        [Route("api/updateform/")]
        [HttpPost]
        public async Task<ActionResult<string>> PostAsync([FromBody] form_get for1)
        {
            try
            {


                TestDbContext ctx = new TestDbContext();
                if (ctx.Forms.Any(e => e.team_name == for1.team_name))
                {
                    return BadRequest("有重名队伍");
                }
                else
                {
                    form form1 = new form();
                    form1.team_name = for1.team_name;
                    form1.team_password = for1.team_password;
                    form1.team_tel = for1.team_tel;
                    List<role> role = new List<role>();
                    foreach (role_get a in for1.role_get)
                    {
                        role.Add(new role { role_id = a.role_id, role_lin = a.role_lin, role_name = a.role_name });
                    }
                    form1.role = role;

                    ctx.Forms.Add(form1);
                    await ctx.SaveChangesAsync();


                }


                return "ok!";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Route("api/addbot/")]
        [HttpPost]
        public async Task<ActionResult<string>> AddbotAsync(string qq, string adress, string key)
        {
            bot.QQ = qq;
            bot.Address = adress;
            bot.VerifyKey = key;
            await bot.LaunchAsync();

            bot.MessageReceived
    .OfType<GroupMessageReceiver>()
    .Subscribe(async x =>
    {
        if (x.MessageChain.GetPlainMessage() == "所有队伍")
        {

            string[] names = null;
            names = System.IO.File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "/allteam.txt");

            string res = string.Join("\r\n", names);
            await MessageManager.SendGroupMessageAsync(x.Sender.Group, "以下队伍已报名：\r\n" + res);
        }
        if (x.MessageChain.GetPlainMessage().Contains("投票#"))
        {
            string msg = x.MessageChain.GetPlainMessage();
            INIFile ini = new INIFile(AppDomain.CurrentDomain.BaseDirectory + "team.ini");
            string name = msg.Split("#")[1];
            string timer = ini.IniReadValue(name, "timer");
            await MessageManager.SendGroupMessageAsync(x.Sender.Group, name);
            if (timer != "")
            {
                int number = ini.IniReadValue(name, "number").ToInt32();
                number++;
                ini.IniWriteValue(name, "number", number.ToString());
                await MessageManager.SendGroupMessageAsync(x.Sender.Group, "投票成功！\r\n票数：" + number);
            }
            else
            {
                await MessageManager.SendGroupMessageAsync(x.Sender.Group, "投票失败,无此队伍：" + name);

            }
        }

    });
            return "ok";
        }

        MiraiBot bot = new MiraiBot
        {
            Address = "localhost:8080",
            QQ = "1590454991",
            VerifyKey = "1145141919810"
        };








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
        public List<role> role { get; set; } = new List<role>();
    }

    public class form_get
    {

        public DateTime time { get; set; } = DateTime.Now;
        public string team_name { get; set; }
        public string team_password { get; set; }
        public string team_tel { get; set; }
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
    [Route("api/allteam/")]
    [HttpGet]
    public List<team> Get()
    {
        TestDbContext ctx = new TestDbContext();
        List<team> teams = new List<team>();
        int a = 0;
        foreach (form for1 in ctx.Forms)
        {
            teams.Add(new team { name = for1.team_name, timer = for1.time, piaoshu = for1.piaoshu });
            a++;
        }
        return teams;
    }

    public class team
    {
        public string name { get; set; }
        public DateTime timer { get; set; }
        public int piaoshu { get; set; }
    }








    [Route("api/lookateam")]
    [HttpPost]
    public async Task<ActionResult<form>> Post(string team_name, string password)
    {
        using TestDbContext ctx = new TestDbContext();
        var form = ctx.Forms.Single(b => b.team_name == team_name);
        if (form.team_password == password)
        {
            return form;

        }
        else
        {
            return BadRequest("密码错误");
        }

    }




    public class form_uri
    {
        public string uri { get; set; }
    }



}



















