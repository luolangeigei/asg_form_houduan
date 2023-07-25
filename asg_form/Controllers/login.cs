
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
/*
            var client = new RestClient($"https://www.recaptcha.net/recaptcha/api/siteverify?secret=6LcdXUEmAAAAAJLICuxBgtMsDiMSCm5XpB0z-fzK&response={captoken}");

            var request = new RestRequest(Method.POST);
            IRestResponse response = client.Execute(request);
            string a = response.Content;
            JObject b = a.ToJObject();
            string ok = b["success"].ToString();


            if (ok == "True")
            {*/
                User user = await this.userManager.FindByEmailAsync(newuser.EMail);
                if (user == null)
                {
                    user = new User { UserName = newuser.UserName, Email = newuser.EMail,chinaname=newuser.chinaname, EmailConfirmed = false };
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
                    return Ok("注册成功");
                }
                return BadRequest(new error_mb { code = 400, message = "邮件已经被注册过！！" });
         /*   }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "未通过人机验证" });

            }
         */
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
            return new post_user { id = id.ToInt64(), name = user.UserName, chinaname = user.chinaname, email = user.Email, isadmin = isadmin ,Roles=roles};

        }

        public class post_user
        {
            public long id { get; set; }
            public string name { get; set; }
            public string chinaname { get; set; }
            public string ?email { get; set; }
            public bool isadmin { get; set; }

            public List<string> Roles { get;set; }

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
            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);
            user.chinaname = newchinaname;
            var r = await userManager.UpdateAsync(user);
            return user;

        }



        [Route("api/v1/chongfa")]
        [HttpPost]
        public async Task<ActionResult<string>> chongfa(string email, string captoken)
        {

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
                    new Email()
                    {
                        SmtpServer = "smtp.office365.com",// SMTP服务器
                        SmtpPort = 587, // SMTP服务器端口
                        EnableSsl = true,//使用SSL
                        Username = "luolan233@outlook.com",// 邮箱用户名
                        Password = "luolan12323",// 邮箱密码
                        Tos = email,//收件人
                        Subject = "欢迎加入ASG赛事！",//邮件标题
                        Body = $"你的验证码为:{email_token}",//邮件内容
                    }.SendAsync(s =>
                    {

                    });// 异步发送邮件
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

