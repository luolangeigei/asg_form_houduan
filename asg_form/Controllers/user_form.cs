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
        public async Task<ActionResult<string>> Addform(string formname, string formpassword)
        {
           
            

            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var ouser = userManager.FindByIdAsync(id).Result;
            if (ouser.haveform != null)
            {
                return BadRequest(new error_mb { code = 400, message = "无法多次绑定表单" });


            }
            else
            {
               TestDbContext db = new TestDbContext();
                ouser.haveform =await db.Forms.FirstOrDefaultAsync(a=>a.team_name==formname);
               await userManager.UpdateAsync(ouser);
                return "绑定成功";
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
            var ouser = userManager.FindByIdAsync(id).Result;
            if (ouser.haveform != null)
            {
                return ouser.haveform;
            }
            else
            {
               return BadRequest(new error_mb { code = 400, message = "你没有绑定表单" });

                
            }


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
            var ouser = userManager.FindByIdAsync(id).Result;
            if (ouser.haveform != null)
            {
                TestDbContext db = new TestDbContext();
                var forme = await db.Forms.FirstOrDefaultAsync(a => a.team_name == ouser.haveform.team_name);
                if (forme == null)
                {
                    return BadRequest(new error_mb { code = 400, message = "表单不存在" });
                }
                else
                {
                    List<role> role = new List<role>();
                    foreach (role_get a in form.role_get)
                    {
                        role.Add(new role { role_id = a.role_id, role_lin = a.role_lin, role_name = a.role_name });
                    }
                    forme.role = role;
                   await db.SaveChangesAsync();
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

