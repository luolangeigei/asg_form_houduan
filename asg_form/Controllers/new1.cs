using Microsoft.AspNetCore.Mvc;

namespace asg_form.Controllers
{
    public class 查询是否重名 : ControllerBase
    {
        [Route("api/chongname/")]
        [HttpPost]
        public async Task<ActionResult<string>> Post(string name)
        {






            TestDbContext ctx = new TestDbContext();
            bool b = ctx.Forms.Any(e => e.team_name == name);
            if (b)
            {
                return BadRequest("Have a CHONG NAME");
            }
            else
            {
                return "No have a chong name";
            }
        }


    }
}
