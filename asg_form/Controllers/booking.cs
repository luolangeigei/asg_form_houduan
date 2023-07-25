
﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace asg_form.Controllers
{


    [Authorize]
    public class booking : ControllerBase
    {


        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        public booking(
            RoleManager<Role> roleManager, UserManager<User> userManager)
        {

            this.roleManager = roleManager;
            this.userManager = userManager;
        }



        [Route("api/newbooking")]
        [HttpPost]
        public async Task<ActionResult<string>> newbooking()
        {
            string username = this.User.FindFirst(ClaimTypes.Name)!.Value;
            User user = await userManager.FindByNameAsync(username);
            if (user.isbooking==null)
            {
                user.isbooking = true;
                await userManager.UpdateAsync(user);
                return "OK";
                
            }
            return BadRequest(new error_mb { code = 400, message = "已经预约过了" }); ;
        }










    }




}