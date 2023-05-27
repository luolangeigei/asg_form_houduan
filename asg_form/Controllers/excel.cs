using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Security.Claims;

namespace asg_form.Controllers
{
    public class excel : ControllerBase
    {
        private readonly ILogger<excel> logger;
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        public excel(ILogger<excel> logger,
            RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            this.logger = logger;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }




        [Authorize]
        [Route("api/allteam/")]
        [HttpPost]
        public async Task<ActionResult<List<allteam>>> Post()
        {


            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);

            bool a = await userManager.IsInRoleAsync(user, "admin");
            if (a)
            {
                TestDbContext ctx = new TestDbContext();
                List<allteam> data = new List<allteam>();
                List<form> teams = ctx.Forms.Include(a => a.role).Where(a => a.Id >= 0).ToList();
                foreach (var team in teams)
                {
                    var roles = team.role;
                    allteam allteam = new allteam();
                    allteam.Id = team.Id;
                    allteam.Name = team.team_name;
                    foreach (var role in roles)
                    {
                        role.form = null;
                        allteam.role.Add(role);
                    }
                    data.Add(allteam);
                }
                return data;
            }
            else
            {
                return BadRequest("无权访问此信息");
            }


        }

        public class allteam
        {
            public long Id { get; set; }

            public string Name { get; set; }

            public List<role> role { get; set; } = new List<role>();
        }



        public static void ExportToExcel<T>(List<T> data, string fileName)
        {
            using (ExcelPackage package = new ExcelPackage(new FileInfo(fileName)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells["A1"].LoadFromCollection(data, true);
                package.Save();
            }
        }
    }
}
