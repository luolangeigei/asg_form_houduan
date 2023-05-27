using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace asg_form.Controllers
{


    [Authorize]
    public class booking : ControllerBase
    {

        private readonly ILogger<booking> logger;
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        public booking(ILogger<booking> logger,
            RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            this.logger = logger;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }



        [Route("api/newbooking")]
        [HttpPost]
        public async Task<ActionResult<string>> newbooking()
        {
            string username = this.User.FindFirst(ClaimTypes.Name)!.Value;
            User user = await userManager.FindByNameAsync(username);
            if ((bool)user.isbooking)
            {
                BadRequest("已经预约过了");
            }
            user.isbooking = true;
            userManager.UpdateAsync(user);
            return "OK";
        }










    }




}
