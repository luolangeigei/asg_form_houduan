using Manganese.Text;
using Masuit.Tools.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
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
            public string EMail { get; set; }

        }

        public class ok_email_get
        {
            public string email { get; set; }
            public string token { get; set; }

        }


        private readonly ILogger<login> logger;
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        public login(ILogger<login> logger,
            RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            this.logger = logger;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        [Route("api/enroll")]
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
                    user = new User { UserName = newuser.UserName, Email = newuser.EMail, EmailConfirmed = false };
                    var r = await userManager.CreateAsync(user, newuser.Password);
                    string email_token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    if (!r.Succeeded)
                    {
                        return BadRequest(r.Errors);
                    }
                    new Email()
                    {
                        SmtpServer = "smtp.office365.com",// SMTP服务器
                        SmtpPort = 587, // SMTP服务器端口
                        EnableSsl = true,//使用SSL
                        Username = "luolan233@outlook.com",// 邮箱用户名
                        Password = "luolan12323",// 邮箱密码
                        Tos = newuser.EMail,//收件人
                        Subject = "欢迎加入ASG赛事！",//邮件标题
                        Body = $"你的验证码为:{email_token}",//邮件内容
                    }.SendAsync(s =>
                    {

                    });// 异步发送邮件
                    return newuser;
                }
                return BadRequest("邮件已经被注册过！！");
            }
            else
            {
                return BadRequest("验证码未通过" + ok);
            }
        }


        [Route("api/okemail")]
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







        [Route("api/chongfa")]
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
                return NotFound("用户不存在！");
            }
            else
            {
                return BadRequest("验证码未通过" + ok);
            }
        }










        [Route("api/login")]
        [HttpPost]
        public async Task<ActionResult<newuser_get>> login1(LoginRequest req, [FromServices] IOptions<JWTOptions> jwtOptions)
        {


            string userName = req.UserName;
            string password = req.Password;
            var user = await userManager.FindByNameAsync(userName);

            if (await userManager.IsLockedOutAsync(user))
            {
                return BadRequest("密码输错太多，账号已被锁定");
            }
            if (user == null)
            {
                return NotFound($"用户名不存在{userName}");
            }
            var success = await userManager.CheckPasswordAsync(user, password);
            if (!success)
            {
                await userManager.AccessFailedAsync(user);
                return BadRequest("Failed");
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
