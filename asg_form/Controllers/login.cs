
using Manganese.Text;
using Masuit.Tools.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using RestSharp;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

namespace asg_form.Controllers
{
    public class login : ControllerBase
    {

        public class newuser_get
        {

            public string UserName { get; set; }
            public string Password { get; set; }
            public string chinaname { get; set; }
            public string EMail { get; set; }

        }

        public class ok_email_get
        {
            public string email { get; set; }
            public string token { get; set; }

        }



        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        public login(
            RoleManager<Role> roleManager, UserManager<User> userManager)
        {

            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="newuser">用户信息</param>
        /// <param name="captoken">谷歌人机验证token</param>
        /// <returns></returns>
        [Route("api/v1/enroll")]
        [HttpPost]
        public async Task<ActionResult<newuser_get>> Post([FromBody] newuser_get newuser, string captoken)
        {

            var client = new RestClient($"https://www.recaptcha.net/recaptcha/api/siteverify?secret=6LcdXUEmAAAAAJLICuxBgtMsDiMSCm5XpB0z-fzK&response={captoken}");

            var request = new RestRequest(Method.POST);
            IRestResponse response = client.Execute(request);
            string a = response.Content;
            JObject b = a.ToJObject();
            string ok = b["success"].ToString();


            if (ok == "True")
            {
                User user = await this.userManager.FindByEmailAsync(newuser.EMail);
                if (user == null)
                {
                    user = new User { UserName = newuser.UserName, Email = newuser.EMail,chinaname=newuser.chinaname, EmailConfirmed = false };
                    var r = await userManager.CreateAsync(user, newuser.Password);
                   
                    if (!r.Succeeded)
                    {
                        return BadRequest(r.Errors);
                    }
                string email_token = await userManager.GenerateEmailConfirmationTokenAsync(user);


                SendEmail(newuser.EMail, "欢迎注册ASG官网账号！", $@"<div>
    <includetail>
        <table style=""font-family: Segoe UI, SegoeUIWF, Arial, sans-serif; font-size: 12px; color: #333333; border-spacing: 0px; border-collapse: collapse; padding: 0px; width: 580px; direction: ltr"">
            <tbody>
            <tr>
                <td style=""font-size: 10px; padding: 0px 0px 7px 0px; text-align: right"">
                    {newuser.chinaname} 正在注册一个新的ASG官网账号。
                </td>
            </tr>
            <tr style=""background-color: #0078D4"">
                <td style=""padding: 0px"">
                    <table style=""font-family: Segoe UI, SegoeUIWF, Arial, sans-serif; border-spacing: 0px; border-collapse: collapse; width: 100%"">
                        <tbody>
                        <tr>
                            <td style=""font-size: 38px; color: #FFFFFF; padding: 12px 22px 4px 22px"" colspan=""3"">
                                注册
                            </td>
                        </tr>
                        <tr>
                            <td style=""font-size: 20px; color: #FFFFFF; padding: 0px 22px 18px 22px"" colspan=""3"">
                                 {newuser.chinaname} 正在注册一个新的ASG官网账号。
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
                               你的验证码是：{email_token}
                                <ul style=""font-size: 14px"">
                                    <li style=""padding-top: 10px"">
                                        如果你没有注册ASG官网账号，请忽略此邮件。
                                    </li>
                                    <li>
                                        请不要回复此邮件。如果你需要帮助，请联系我们。
                                    </li>
                                    <li>
                                        请不要与他人分享此验证码。
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

                    
                    return Ok("注册成功");
                }
                return BadRequest(new error_mb { code = 400, message = "邮件已经被注册过！！" });
           }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "未通过人机验证" });

            }
         
        }



        /// <summary>
        /// 确认邮件验证码
        /// </summary>
        /// <param name="EMail">邮箱</param>
        /// <returns></returns>
        [Route("api/v1/okemail")]
        [HttpPost]
        public async Task<ActionResult<newuser_get>> okemail([FromBody] ok_email_get EMail)
        {
            try
            {
                User user = await userManager.FindByEmailAsync(EMail.email);
                userManager.ConfirmEmailAsync(user, EMail.token);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok("成功！");

        }


        /// <summary>
        /// 获取我自己的信息
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("api/v1/user/")]
        [HttpGet]
        public async Task<ActionResult<post_user>> getuser()
        {
            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);
            var isadmin = await userManager.IsInRoleAsync(user, "admin");
            List<string> roles= (List<string>)await userManager.GetRolesAsync(user);
            return new post_user { id = id.ToInt64(),Base64=user.UserBase64, name = user.UserName, chinaname = user.chinaname, email = user.Email, isadmin = isadmin ,Roles=roles};

        }

        public class post_user
        {

            public long id { get; set; }
            public string? Base64 { get; set; }
            public string name { get; set; }
            public string? chinaname { get; set; }
            public string? email { get; set; }
            public bool isadmin { get; set; }

            public List<string>? Roles { get;set; }

        }
        /// <summary>
        /// 修改中文名称
        /// </summary>
        /// <param name="newchinaname">新的中文名称</param>
        /// <returns></returns>
        [Route("api/v1/user/name")]
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<User>> setusername(string newchinaname)
        {
/*
            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);
            user.chinaname = newchinaname;
            var r = await userManager.UpdateAsync(user);
            return user;
*/
            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);
            user.chinaname = newchinaname;
            var r = await userManager.UpdateAsync(user);
            return Ok("修改成功");
        }


        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="email">收件人邮箱</param>
        /// <param name="title">标题</param>
        /// <param name="content">发送内容</param>
        /// <returns></returns>
        public static bool SendEmail(string email, string title, string content)
        {
            string _smtpServer = "smtp.office365.com";   //SMTP服务器
            string _userName = "luolan233@outlook.com";   //邮箱
            string _pwd = "luolan12323";   //密码或授权码

            if (_smtpServer == "" || _userName == "" || _pwd == "")
            {
                return false;
            }

            using (System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage())
            {
                /*   
               * msg.To.Add("b@b.com");可以发送给多人   
               */
                msg.To.Add(email);  //设置收件人

                /*  
                * msg.CC.Add("c@c.com");   
                * msg.CC.Add("c@c.com");可以抄送给多人   
                */

                /* 3个参数分别是 发件人地址（可以随便写），发件人姓名，编码*/
                msg.From = new MailAddress(_userName, _userName, System.Text.Encoding.UTF8);


                msg.Subject = title; //邮件标题   
                msg.SubjectEncoding = System.Text.Encoding.UTF8; //邮件标题编码   
                msg.Body = content;  //邮件内容   
                msg.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码   
                msg.IsBodyHtml = true; //是否是HTML邮件   
                msg.Priority = MailPriority.Normal; //邮件优先级

                SmtpClient client = new SmtpClient(_smtpServer, 587); //邮件服务器地址及端口号
                client.EnableSsl = true; //ssl加密发送
                client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                client.Credentials = new System.Net.NetworkCredential(_userName, _pwd); //邮箱账号  密码
                client.Timeout = 40000;  //6秒超时

                client.Send(msg);  //发送邮件

                client.Dispose();  //释放资源
                return true;

            }
        }



        [Route("api/v1/sendemail")]
        [HttpPost]
        public async Task<ActionResult<string>> chongfa(string email, string captoken)
        {
            //验证谷歌人机验证
           
            var client = new RestClient($"https://www.recaptcha.net/recaptcha/api/siteverify?secret=6LcdXUEmAAAAAJLICuxBgtMsDiMSCm5XpB0z-fzK&response={captoken}");

            var request = new RestRequest(Method.POST);
            IRestResponse response = client.Execute(request);
            string a = response.Content;
            JObject b = a.ToJObject();
            string ok = b["success"].ToString();


            if (ok == "True")
            {

                User user = await this.userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    string email_token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    SendEmail(user.Email, "欢迎注册ASG官网账号！", $@"<div>
    <includetail>
        <table style=""font-family: Segoe UI, SegoeUIWF, Arial, sans-serif; font-size: 12px; color: #333333; border-spacing: 0px; border-collapse: collapse; padding: 0px; width: 580px; direction: ltr"">
            <tbody>
            <tr>
                <td style=""font-size: 10px; padding: 0px 0px 7px 0px; text-align: right"">
                    {user.chinaname} 正在注册一个新的ASG官网账号。
                </td>
            </tr>
            <tr style=""background-color: #0078D4"">
                <td style=""padding: 0px"">
                    <table style=""font-family: Segoe UI, SegoeUIWF, Arial, sans-serif; border-spacing: 0px; border-collapse: collapse; width: 100%"">
                        <tbody>
                        <tr>
                            <td style=""font-size: 38px; color: #FFFFFF; padding: 12px 22px 4px 22px"" colspan=""3"">
                                注册
                            </td>
                        </tr>
                        <tr>
                            <td style=""font-size: 20px; color: #FFFFFF; padding: 0px 22px 18px 22px"" colspan=""3"">
                                 {user.chinaname} 正在注册一个新的ASG官网账号。
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
                               你的验证码是：{email_token}
                                <ul style=""font-size: 14px"">
                                    <li style=""padding-top: 10px"">
                                        如果你没有注册ASG官网账号，请忽略此邮件。
                                    </li>
                                    <li>
                                        请不要回复此邮件。如果你需要帮助，请联系我们。
                                    </li>
                                    <li>
                                        请不要与他人分享此验证码。
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
                    return "ok!";
                }
                return NotFound(new error_mb { code = 404, message = "未找到用户" });
 
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "人机验证未通过" });

            }
            

        }






        /// <summary>
        /// 根据职位获取用户
        /// </summary>
        /// <param name="req">用户信息</param>
        /// <param name="jwtOptions"></param>
        /// <returns></returns>
        [Route("api/v1/getuserbyop")]
        [HttpGet]
        public async Task<List<post_user>> getuserbyop(string opname)
        {
        var opuser= userManager.Users.Where(x => x.officium == opname).ToList();
            List<post_user> user = new List<post_user>();
            foreach (var auser in opuser)
            {
                bool isadmin = await userManager.IsInRoleAsync(auser, "admin");
                var roles = await userManager.GetRolesAsync(auser);
                user.Add(new post_user { id = auser.Id, chinaname = auser.chinaname, name = auser.UserName, isadmin = isadmin, email = auser.Email, Roles = (List<string>)roles });

            }
            return user;


        }






            /// <summary>
            /// 登陆
            /// </summary>
            /// <param name="req">用户信息</param>
            /// <param name="jwtOptions"></param>
            /// <returns></returns>
            [Route("api/v1/login")]
        [HttpPost]
        public async Task<ActionResult<newuser_get>> login1([FromBody]LoginRequest req, [FromServices] IOptions<JWTOptions> jwtOptions)
        {


            string userName = req.UserName;
            string password = req.Password;
            var user = await userManager.FindByNameAsync(userName);

            if (await userManager.IsLockedOutAsync(user))
            {
                return BadRequest(new error_mb { code = 400, message = "账号被锁定" });

            }
            if (user == null)
            {
                return NotFound(new error_mb { code = 404, message = "用户未找到" });

            }
            var success = await userManager.CheckPasswordAsync(user, password);
            if (!success)
            {
                await userManager.AccessFailedAsync(user);
                return BadRequest(new error_mb { code = 400, message = "人机验证未通过" });

            }
            
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            var roles = await userManager.GetRolesAsync(user);
            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            string jwtToken = BuildToken(claims, jwtOptions.Value);
            return Ok(jwtToken);


        }
        private static string BuildToken(IEnumerable<Claim> claims, JWTOptions options)
        {
            DateTime expires = DateTime.Now.AddSeconds(options.ExpireSeconds);
            byte[] keyBytes = Encoding.UTF8.GetBytes(options.SigningKey);
            var secKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(secKey,
                SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(expires: expires,
                signingCredentials: credentials, claims: claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}

