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
          


            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var ouser = await userManager.FindByIdAsync(id);

            bool sa = await userManager.IsInRoleAsync(ouser, "admin");
            if (sa)
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
                    user.Add(new post_user { id = auser.Id, chinaname = auser.chinaname, name = auser.UserName, isadmin = isadmin, email = auser.Email });
                }
                return user;

                return BadRequest(new error_mb { code = 400, message = "此邮件已被使用" });


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
            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);

            bool a = await userManager.IsInRoleAsync(user, "nbadmin");
            if (a)
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
            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var ouser = await userManager.FindByIdAsync(id);

            bool a = await userManager.IsInRoleAsync(ouser, "admin");
            if (a)
            {

                User user = await this.userManager.FindByEmailAsync(newuser.EMail);
                if (user == null)
                {
                    user = new User { UserName = newuser.UserName, Email = newuser.EMail, chinaname = newuser.chinaname, EmailConfirmed = false };
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

            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);

            bool a = await userManager.IsInRoleAsync(user, "nbadmin");
            if (a)
            {
                var setuser = await userManager.FindByIdAsync(userid);

                await userManager.DeleteAsync(setuser);
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
            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);

            bool a = await userManager.IsInRoleAsync(user, "nbadmin");
            if (a)
            {
                var ouser = await userManager.FindByIdAsync(userid);

                ouser.officium = officium;
                userManager.UpdateAsync(ouser);
                  
                return "成功！";
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }

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

            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);

            bool a = await userManager.IsInRoleAsync(user, "admin");
            if (a)
            {
                TestDbContext ctx = new TestDbContext();
                var form = await ctx.Forms.FirstOrDefaultAsync(a => a.team_name == formname);
                ctx.Forms.Remove(form); ;
                await ctx.SaveChangesAsync();
                return Ok("删除成功！");
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }






        }











    }
}
