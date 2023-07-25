
﻿using asg_form;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Packaging.Ionic.Zlib;
using System.Security.Claims;
using static asg_form.Controllers.login;

namespace asg_form.Controllers
{
    public class excel : ControllerBase
    {

        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        public excel(
            RoleManager<Role> roleManager, UserManager<User> userManager)
        {

            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        /// <summary>
        /// 通过战队名搜索一个战队的详细信息
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
       [Authorize]
        [Route("api/v1/admin/form/{search}")]
        [HttpGet]
        public async Task<ActionResult<List<allteam>>> Get(string search)
        {
     
            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);

            bool a = await userManager.IsInRoleAsync(user, "admin");
            if (a)
            {
                TestDbContext ctx = new TestDbContext();
                List<allteam> data = new List<allteam>();
                List<form> teams = ctx.Forms.Include(a => a.role).Where(a => a.team_name.IndexOf(search)>=0).ToList();
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
                return BadRequest(new error_mb { code = 400, message = "无权访问" });
            }
    

        }



        /// <summary>
        /// 获得所有战队信息
        /// </summary>
        /// <param name="page"></param>
        /// <param name="page_long"></param>
        /// <returns></returns>
            [Authorize]
        [Route("api/v1/admin/form/all")]
        [HttpGet]
        public async Task<ActionResult<List<allteam>>> Post(short page,short page_long)
        {





            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);

            bool a = await userManager.IsInRoleAsync(user, "admin");
            if (a)
            {

                TestDbContext ctx = new TestDbContext();


                int c = ctx.Forms.Count();
                int b = page_long * page;
                if (page_long * page > c)
                {
                    b = c;
                }
                var teams = ctx.Forms.Include(a => a.role).Skip(page_long * page - page_long).Take(page_long).ToList();
                List<allteam> data = new List<allteam>();
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
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }


        }







public class allteam
        {
            /// <summary>
            /// 战队id
            /// </summary>
            public long Id { get; set; }
            /// <summary>
            /// 战队名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 战队的角色
            /// </summary>
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