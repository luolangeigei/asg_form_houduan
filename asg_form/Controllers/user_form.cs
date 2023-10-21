using Manganese.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static asg_form.blog;

namespace asg_form.Controllers
{
    public class user_form : ControllerBase
    {
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;

        public user_form(
            RoleManager<Role> roleManager, UserManager<User> userManager)
        {

            this.roleManager = roleManager;
            this.userManager = userManager;
        }
    



    /// <summary>
    /// 绑定表单
    /// </summary>
    /// <param name="formname">表单名称</param>
    /// <param name="formpassword">表单密码</param>
    /// <returns></returns>
    [Authorize]
        [Route("api/v1/user/form")]
        [HttpPost]
        public async Task<ActionResult<string>> Addform(string formname,string eventname, string formpassword)
        {
           
            

            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var ouser = userManager.FindByIdAsync(id).Result;
            TestDbContext db = new TestDbContext();
            form formok;
            try {

               formok = await db.Forms.Include(a=>a.events).FirstAsync(a => a.team_name == formname&&a.events.name==eventname);

            }
            catch
            {
                return NotFound(new error_mb { code = 404, message = "表单不存在" });

            }


            if (ouser.haveform != null)
            {
                return BadRequest(new error_mb { code = 400, message = "无法多次绑定表单" });


            }
            if(formok.team_password==formpassword)
            {
                ouser.haveform = await db.Forms.FirstOrDefaultAsync(a => a.team_name == formname);
                await userManager.UpdateAsync(ouser);
                return "绑定成功";
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "表单密码错误" });

            }
        }

        /// <summary>
        /// 获取我的表单
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("api/v1/user/form")]
        [HttpGet]
        public async Task<ActionResult<form>> getmyform()
        {

            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var ouser =  userManager.Users.Include(a=>a.haveform.role).Include(a=>a.haveform.events).FirstOrDefault(a => a.Id == id.ToInt64());
            if (ouser.haveform == null)
            {
                return BadRequest(new error_mb { code = 400, message = "你没有绑定表单" });

            }
            ouser.haveform.events.forms = null;
            foreach (var role in ouser.haveform.role)
            {
                role.form = null;
            }
                return ouser.haveform;



        }


        /// <summary>
        /// 修改我的表单
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("api/v1/user/form")]
        [HttpPut]
        public async Task<ActionResult<form>> putmyform([FromBody]form_get form)
        { 
        
            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var ouser = userManager.Users.Include(a => a.haveform.role).FirstOrDefault(a => a.Id == id.ToInt64());
         
            if (ouser.haveform != null)
            {
                TestDbContext db = new TestDbContext();
             var forme = ouser.haveform;
                if (forme == null)
                {
                    return BadRequest(new error_mb { code = 400, message = "表单不存在" });
                }
                else
                {
                    //只能修改表单role
                  ouser.haveform.role.Clear();
                    foreach (role_get a in form.role_get)
                    {
                        ouser.haveform.role.Add(new role { role_id = a.role_id, role_lin = a.role_lin, role_name = a.role_name });
                    }
                   await userManager.UpdateAsync(ouser);

                    return Ok("修改完成");
                }
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "你没有绑定表单" });
            }



        
        }



        }


    }

