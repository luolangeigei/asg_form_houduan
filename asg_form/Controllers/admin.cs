using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static asg_form.Controllers.excel;
using System.Security.Claims;
using static asg_form.Controllers.login;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using RestSharp;
using static asg_form.blog;
using static asg_form.Controllers.schedule;
using static NPOI.HSSF.Util.HSSFColor;
using NLog;
using MimeKit;
using MailKit.Net.Smtp;
using System.Security.Authentication;
using System.Text.Json;

namespace asg_form.Controllers
{

    public class admin : ControllerBase
    {
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        public admin(
            RoleManager<Role> roleManager, UserManager<User> userManager)
        {

            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        [Route("api/v1/admin/allperson_c")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<int>> getalladmin_c()
        {
           int a=await userManager.Users.CountAsync();
            return Ok(a);
        }
        [Route("api/v1/admin/allteam_c")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<int>> getteam_c()
        {
             TestDbContext testDb=new TestDbContext();
              int a = testDb.Forms.Count();
              return Ok(a);
        }


        [Route("api/v1/admin/allschedle_c")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<int>> getschedle_c()
        {
            TestDbContext testDb = new TestDbContext();
            int a = testDb.team_Games.Count();
            return Ok(a);
        }

        /// <summary>
        /// 获取所有用户-支持分页
        /// </summary>
        /// <param name="page"></param>
        /// <param name="page_long"></param>
        /// <returns></returns>
        [Route("api/v1/admin/allperson")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<post_user>>> getalladmin( short page,short page_long=10)
        {



            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {

                int a = userManager.Users.Count();
                int b = page_long * page;
                if (page_long * page > a)
                {
                    b = a;
                }
                var users = userManager.Users.Skip(page_long * page - page_long).Take(page_long).ToList();
                List<post_user> user = new List<post_user>();
                foreach (var auser in users)
                {
                    bool isadmin = await userManager.IsInRoleAsync(auser, "admin");
                    var roles = await userManager.GetRolesAsync(auser);
 user.Add(new post_user { id = auser.Id, chinaname = auser.chinaname, name = auser.UserName, isadmin = isadmin, email = auser.Email ,Roles= (List<string>)roles ,officium=auser.officium});

                }
                return user;

              
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }




        }

        /// <summary>
        /// 设置管理员,需要superadmin
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [Route("api/v1/admin/setadmin")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<string>> setadmin(string userid)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                var ouser = await userManager.FindByIdAsync(userid);

                await userManager.AddToRoleAsync(ouser, "admin");
              return "成功！";
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }

        }
       

        //管理员设置用户的职位
        [Route("api/v1/admin/setop")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<string>> setrole(string userid,string opname)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                var ouser = await userManager.FindByIdAsync(userid);

              ouser.officium = opname;
                await userManager.UpdateAsync(ouser);

        
               return "成功！";
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }

        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="email">收件人邮箱</param>
        /// <param name="title">标题</param>
        /// <param name="content">发送内容</param>
        /// <returns></returns>
        public static bool SendEmail(string email1, string title, string content)
        {
            var smtpServer = new SmtpClient();

            smtpServer.ServerCertificateValidationCallback = (s, c, h, e) => true;
            smtpServer.SslProtocols = SslProtocols.Tls12;
            smtpServer.Connect("mail.idvasg.cn", 465, true); // 连接到SMTP服务器  
            smtpServer.Authenticate("admin@idvasg.cn", "Luolan12323"); // 进行身份验证  

            // 创建邮件对象并设置属性  
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("ASG赛事组", "admin@idvasg.cn"));
            message.To.Add(new MailboxAddress("用户", email1));
            message.Subject = title;
            message.Body = new TextPart("html")
            {
                Text = content
            };

            // 发送邮件  
            try
            {
               
                smtpServer.Send(message);
                smtpServer.Disconnect(true); // 断开与SMTP服务器的连接  
                Console.WriteLine("邮件发送成功！");
            }
            catch (Exception ex)
            {
                Console.WriteLine("邮件发送失败：" + ex.Message);
            }
            return true;


            
        }




        /// <summary>
        /// 管理员直接添加一个用户
        /// </summary>
        /// <param name="newuser"></param>
        /// <param name="captoken"></param>
        /// <returns></returns>
        [Route("api/v1/admin/enroll")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<newuser_get>> Post([FromBody] newuser_get newuser, string captoken)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {

                User user = await this.userManager.FindByEmailAsync(newuser.EMail);
                if (user == null)
                {
                    user = new User { UserName = newuser.UserName, Email = newuser.EMail, chinaname = newuser.chinaname, EmailConfirmed = true };
                    var r = await userManager.CreateAsync(user, newuser.Password);

                    if (!r.Succeeded)
                    {
                        return BadRequest(r.Errors);
                    }
                    /*   new Email()
                       {
                           SmtpServer = "smtphz.qiye.163.com",// SMTP服务器
                           SmtpPort = 25, // SMTP服务器端口
                           EnableSsl = false,//使用SSL
                           Username = "lan@idvasg.cn",// 邮箱用户名
                           Password = "aNcdGsEYVghrNsE7",// 邮箱密码
                           Tos = newuser.EMail,//收件人
                           Subject = "欢迎加入ASG赛事！",//邮件标题
                           Body = $"欢迎加入ASG赛事，当你看到这封邮件时说明你已经注册成功，感谢你支持ASG赛事！",//邮件内容
                       }.SendAsync(s =>
                       {

                       });// 异步发送邮件
                    */
                    return newuser;
                }
                return BadRequest(new error_mb { code = 400, message = "此邮件已被使用" });


            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }




        }




        /// <summary>
        /// 删除用户,需要superadmin
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [Route("api/v1/admin/deluser")]
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult<string>> deluser(string userid)
        {

            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                var setuser = await userManager.FindByIdAsync(userid);

                await userManager.DeleteAsync(setuser);
                logger.Warn($"管理员删除了用户{setuser.UserName}！");
               return "成功！";
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }

        }



        /// <summary>
        /// 设置职位,需要superadmin
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <param name="officium">职位名称</param>
        /// <returns></returns>
        [Route("api/v1/admin/officium")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<string>> setofficium(string userid,string officium)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                var ouser = await userManager.FindByIdAsync(userid);

                ouser.officium = officium;
               await userManager.UpdateAsync(ouser);
                  logger.Warn($"设置了{ouser.UserName}的职位为{officium}");
                SendEmail(ouser.Email, "ASG赛事组", $@"<div>
    <includetail>
        <table style=""font-family: Segoe UI, SegoeUIWF, Arial, sans-serif; font-size: 12px; color: #333333; border-spacing: 0px; border-collapse: collapse; padding: 0px; width: 580px; direction: ltr"">
            <tbody>
            <tr>
                <td style=""font-size: 10px; padding: 0px 0px 7px 0px; text-align: right"">
                    {ouser.chinaname} ，欢迎加入ASG赛事组。
                </td>
            </tr>
            <tr style=""background-color: #0078D4"">
                <td style=""padding: 0px"">
                    <table style=""font-family: Segoe UI, SegoeUIWF, Arial, sans-serif; border-spacing: 0px; border-collapse: collapse; width: 100%"">
                        <tbody>
                        <tr>
                            <td style=""font-size: 38px; color: #FFFFFF; padding: 12px 22px 4px 22px"" colspan=""3"">
                                欢迎
                            </td>
                        </tr>
                        <tr>
                            <td style=""font-size: 20px; color: #FFFFFF; padding: 0px 22px 18px 22px"" colspan=""3"">
                                 欢迎{ouser.chinaname}加入ASG赛事组。
                            </td>
                        </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
            <tr>
                <td style=""padding: 30px 20px; border-bottom-style: solid; border-bottom-color: #0078D4; border-bottom-width: 4px"">
                    <table style=""font-family: Segoe UI, SegoeUIWF, Arial, sans-serif; font-size: 12px; color: #333333; border-spacing: 0px; border-collapse: collapse; width: 100%"">
                        <tbody>
                        <tr>
                            <td style=""font-size: 12px; padding: 0px 0px 5px 0px"">
                               你的职位已经被设置为{officium}。
                                <ul style=""font-size: 14px"">
                                    <li style=""padding-top: 10px"">
                                        对此次执行有疑问请联系我们的QQ：2667210109。
                                    </li>
                                    <li>
                                        请不要回复此邮件。如果你需要帮助，请联系我们。
                                    </li>
                                    <li>
                                        请加入对应职位的群聊。
                                    </li>
                                </ul>
                            </td>
                        </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
            <tr>
                <td style=""padding: 0px 0px 10px 0px; color: #B2B2B2; font-size: 12px"">
                    版权所有 ASG赛事官网
                </td>
            </tr>
            </tbody>
        </table>
    </includetail>
</div>
");
                return "成功！";
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }

        }

        /// <summary>
        /// 给所有form两两随机组队
        /// </summary>
        /// <param name="formname"></param>
        /// <returns></returns>
        [Route("api/v1/admin/team/")]
        [HttpPost]
        public async Task<ActionResult<string>> team(string events_name)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            { 
            TestDbContext ctx = new TestDbContext();
                var eve=ctx.events.First(a=>a.name==events_name);
                var form =  ctx.Forms.Where(a=>a.events==eve).OrderBy(a => Guid.NewGuid()).ToList();
             
                string teamname1 = "";
                string teamname2 = "";
                for (int i = 0; i < form.Count; i++)
                {
                    if (i % 2 == 0)
                    {
                        teamname1 = form[i].team_name;
                        
                    }
                    else
                    {
                       teamname2 = form[i].team_name;
                        ctx.team_Games.Add(new team_game
                        {
                            team1_name =teamname1,
                            team2_name = teamname2,
                            opentime = DateTime.Now,
                            team1_piaoshu = 0,
                            team2_piaoshu = 0,
                            commentary = "待公布",
                            referee = "待公布",
                            belong=events_name
                        });
                    }
                }
                
               await ctx.SaveChangesAsync();
                logger.Info($"管理员已经随机分组");
                return "OK";
            
            }
            return BadRequest(new error_mb { code = 400, message = "无权访问" });


        }

        [Route("api/v1/admin/SendEmail/")]
        [HttpPost]
        public async Task<ActionResult<string>> Sendemail(string To,string Title,string msg)
        { 
        SendEmail(To,Title, msg);
            return Ok();
        }

            /// <summary>
            /// 删除表单
            /// </summary>
            /// <param name="formid">表单id</param>
            /// <param name="password">表单密码</param>
            /// <returns></returns>
            [Route("api/v1/admin/form/")]
        [HttpDelete]
        public async Task<ActionResult<string>> delform(string formname)
        {

            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                TestDbContext ctx = new TestDbContext();
                var form = await ctx.Forms.Include(a=>a.role).FirstOrDefaultAsync(a => a.team_name == formname);
                ctx.Forms.Remove(form); ;
                await ctx.SaveChangesAsync();
                logger.Warn($"管理员删除了表单{formname},参赛选手：{string.Join(',',form.role.Select(a=>a.role_name))}");
                return Ok("删除成功！");
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }






        }



        


        private readonly Logger logger = LogManager.GetCurrentClassLogger();






    }
}
