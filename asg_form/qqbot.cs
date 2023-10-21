using asg_form.Controllers;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions;
using Mirai.Net.Sessions.Http.Managers;
using System.Reactive.Linq;
using Microsoft.EntityFrameworkCore;
using Mirai.Net.Data.Events.Concretes.Bot;
using static asg_form.Controllers.schedule;
using NPOI.OpenXmlFormats.Spreadsheet;
using Manganese.Array;
using asg_form.Migrations;

namespace asg_form
{
    public class qqbot:BackgroundService
    {
        private void setTaskAtFixedTime()
        {
            DateTime now = DateTime.Now;
            DateTime oneOClock = DateTime.Today.AddHours(1.0); //凌晨1：00
            if (now > oneOClock)
            {
               // Console.WriteLine("=======================================>");
                oneOClock = oneOClock.AddDays(5.0);
            }
            int msUntilFour = (int)((oneOClock - now).TotalMilliseconds);

            var t = new System.Threading.Timer(doAt1AM);
            t.Change(msUntilFour, Timeout.Infinite);
        }
        public static bool isToday(DateTime dt)
        {
            DateTime today = DateTime.Today;
            DateTime tempToday = new DateTime(dt.Year, dt.Month, dt.Day);
            if (today == tempToday)
                return true;
            else
                return false;
        }

        private async void doAt1AM(object state)
        {
            //执行功能...
            try
            {
                TestDbContext db = new TestDbContext();
                var sh = db.team_Games.ToList();
               var sh1= sh.Where(a=>isToday(a.opentime)).ToList();
                string msg = "";
                if (sh1 == null)
                {
                    msg = "<今日无赛程！>";
                }
                else
                {
                    foreach (var a in sh1)
                    {
                        msg = $"{msg}\r\r{a.team1_name} VS {a.team2_name}";
                    }
                }
                await MessageManager.SendGroupMessageAsync("870248618", $"今日赛程：\r\n{msg}\r\n请有比赛的解说提前准备好。");
                await Task.Delay(3000);
                await MessageManager.SendGroupMessageAsync("456414070", $"今日赛程：\r\n{msg}\r\n。直播地址：\r\nhttps://live.bilibili.com/24208371");

            }
            catch
            {

            }
            //再次设定
            setTaskAtFixedTime();
        }



        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true) {
                Console.WriteLine("qqbot启动中");
                try
                {
                    //定时任务
                    setTaskAtFixedTime();
                    var bot = new MiraiBot
                    {
                        Address = "localhost:8080",
                        QQ = "197649191",
                        VerifyKey = "1234567890"
                    };

                await bot.LaunchAsync();

                   bot.EventReceived
                          .OfType<DroppedEvent>()
    .Subscribe(async receiver =>
    {
        Console.WriteLine("bot断开连接,十秒后重连");
        await Task.Delay(10000);
        await ExecuteAsync(stoppingToken);
    });

                    bot.MessageReceived
        .OfType<GroupMessageReceiver>()
        .Subscribe(async x =>
        {
            try
            {
                if (x.MessageChain.GetPlainMessage() == "近期赛程")
                {
                    TestDbContext testDb = new TestDbContext();
                    int q = testDb.team_Games.Count();
                    var a = testDb.team_Games.Where(a => a.opentime >= DateTime.Now).Take(7);
                    string msg = "";
                    foreach (var b in a)
                    {
                        msg = $"{msg}\r\n{b.team1_name} VS {b.team2_name}\r\n时间:{b.opentime.ToString("f")}";
                    }
                    await MessageManager.SendGroupMessageAsync(x.GroupId, msg);
                }
                if (x.MessageChain.GetPlainMessage() == "参赛队伍")
                {
                    TestDbContext testDb = new TestDbContext();
                    var team = testDb.Forms.Select(a => a.team_name);
                    string msg = "";
                    foreach (var t in team)
                    {
                        msg = $"{msg} {t}";
                    }
                    await MessageManager.SendGroupMessageAsync(x.GroupId, $"在所有比赛中有以下队伍参赛：\r\n{string.Join(" , ",team)}");

                }
                if (x.MessageChain.GetPlainMessage() == "查询冠军")
                {
                    TestDbContext ctx = new TestDbContext();

                   var teams = ctx.Champions.Include(a=>a.events).Include(a=>a.form.role).Select(a => new {a.form,a.events,a.msg}).ToList();
                    string msg = "";
                    foreach (var t in teams)
                    {
                        string role = "";
      
                        foreach (var t2 in t.form.role)
                        {
                            role = $"{role}  {t2.role_name}";
                        }
                        msg = $"{msg}\r\n队伍名称:{t.form.team_name}\r\n队员：{role}\r\n属于：{t.events.name}\r\n简介：{t.msg}\r\n";
                    }
                    await MessageManager.SendGroupMessageAsync(x.GroupId, $"拥有以下冠军:{msg}");

                }
            }
            catch(Exception ex) 
            {
                await MessageManager.SendGroupMessageAsync(x.GroupId, $"错误:{ex.Message}");

            }


        });



                    bot.MessageReceived
     .OfType<GroupMessageReceiver>()
     .Where(a=>a.MessageChain.GetPlainMessage().StartsWith("查询战队 "))
                    .Subscribe(async x =>
                    {

                        try {
                            TestDbContext ctx = new TestDbContext();

                            string result = x.MessageChain.GetPlainMessage().Substring(5); // 截取从'o'之后的字符串
                            Console.WriteLine(result);
                            List<form> teams = ctx.Forms.Include(a => a.role).Where(a => a.team_name.IndexOf(result) >= 0).ToList();
                            string msg = "";
                            foreach (var t in teams)
                            {
                                string role = "";
                                foreach (var t2 in t.role)
                                {
                                    role = $"{role}  {t2.role_name}";
                                }
                                msg = $"{msg}\r\n队伍名称:{t.team_name}\r\n队员：{role}\r\n";
                            }
                            await MessageManager.SendGroupMessageAsync(x.GroupId, msg);
                        }
                        catch
                        {

                        }
                     
                    });


                    bot.MessageReceived
   .OfType<GroupMessageReceiver>()
   .Where(a => a.MessageChain.GetPlainMessage().StartsWith("查询选手 "))
                  .Subscribe(async x =>
                  {

                      try
                      {
                          TestDbContext ctx = new TestDbContext();

                          string result = x.MessageChain.GetPlainMessage().Substring(5); // 截取从'o'之后的字符串
                          Console.WriteLine(result);
                          string msg = "";
                         var roles= ctx.Roles.Include(a => a.form).Where(a => a.role_name.IndexOf(result) >= 0).ToList();
                          foreach(var role in roles)
                          {
                              msg = $"{msg}\r\n姓名:{role.role_name}\r\n第五人格ID:{role.role_id}\r\n选手id:{role.Id}\r\n阵营:{role.role_lin}\r\n属于队伍:{role.form.team_name}\r\n";
                          }
                          await MessageManager.SendGroupMessageAsync(x.GroupId,$"搜索到以下结果:\r\n{msg}");
                      }
                      catch
                      {

                      }

                  });











                    Console.ReadLine();
                    bot.Dispose();
                }
                catch
                {
                    Console.WriteLine("bot启动失败,十秒后重连");
                    await Task.Delay(10000);
                }
            
        }
        }
    }
}
