
﻿using asg_form;
using Masuit.Tools.Files;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Packaging.Ionic.Zlib;
using System.IO.Compression;
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

        public static void ExportToExcel<T>(List<T> data, string fileName)
        {
            using (var package = new ExcelPackage())
            {
                int worksheetIndex = 1;
                foreach (var item in data)
                {
                    var properties = typeof(T).GetProperties();
                    var worksheet = package.Workbook.Worksheets.Add($"队伍 :{worksheetIndex}");

                 
                    int columnIndex = 1;

                    // 写入 Person 实体类的属性名作为表头  
                    foreach (var property in properties)
                    {
                        worksheet.Cells[1, columnIndex].Value = property.Name;
                        columnIndex++;
                    }

                    int rowIndex = 2;

                    // 写入 Person 实体类的属性值  
                    foreach (var property in properties)
                    {
                        worksheet.Cells[rowIndex, columnIndex].Value = property.GetValue(item)?.ToString();
                        columnIndex++;
                    }

                    // 写入 Address 实体类的属性值  
                    if (properties.Any(p => p.PropertyType == typeof(List<role>)))
                    {
                        var addresses = (List<role>)properties.First(p => p.PropertyType == typeof(List<role>)).GetValue(item);
                        int addressIndex = 0;
                        foreach (var address in addresses)
                        {
                            worksheet.Cells[++rowIndex, columnIndex].Value = "Role";
                            columnIndex++;

                            var addressProperties = typeof(role).GetProperties();
                            foreach (var addressProperty in addressProperties)
                            {
                                worksheet.Cells[rowIndex, columnIndex].Value = addressProperty.Name;
                                columnIndex++;
                            }
                            rowIndex++;

                            foreach (var addressProperty in addressProperties)
                            {
                                worksheet.Cells[rowIndex, columnIndex].Value = addressProperty.GetValue(address)?.ToString();
                                columnIndex++;
                            }
                            rowIndex++;
                            addressIndex++;
                        }
                    }
                }
                FileInfo excelFile = new FileInfo(fileName);
                package.SaveAs(excelFile);
            }
        }


        [Authorize]
        [Route("api/v1/admin/form/{search}")]
        [HttpGet]
        public async Task<ActionResult<List<allteam>>> Get(string search)
        {

            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
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


        [Authorize]
        [Route("api/v1/admin/user/{search}")]
        [HttpGet]
        public async Task<ActionResult<List<post_user>>> searchuser(string search)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                TestDbContext ctx = new TestDbContext();
                List<post_user> data = new List<post_user>();
                List<User> users = userManager.Users.Where(a => a.UserName.IndexOf(search) >= 0).ToList();
                foreach (var user1 in users)
                {
                    post_user post_User = new post_user();
                    post_User.id = user1.Id;
                    post_User.name = user1.UserName;
                    post_User.email = user1.Email;
                    post_User.officium = user1.officium;
                    post_User.chinaname = user1.chinaname;

                post_User.Base64=user1.UserBase64;
                    post_User.Roles = (List<string>?)await userManager.GetRolesAsync(user1);
                    data.Add(post_User);
                }
                return data;
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });
            }
        }

        [Authorize]
        [Route("api/v1/admin/user/email/{search}")]
        [HttpGet]
        public async Task<ActionResult<List<post_user>>> searchuser_byemail(string search)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                TestDbContext ctx = new TestDbContext();
                List<post_user> data = new List<post_user>();
                List<User> users = userManager.Users.Where(a => a.Email.IndexOf(search) >= 0).ToList();
                foreach (var user1 in users)
                {
                    post_user post_User = new post_user();
                    post_User.id = user1.Id;
                    post_User.name = user1.UserName;
                    post_User.email = user1.Email;
                    post_User.officium = user1.officium;
                    post_User.chinaname = user1.chinaname;

                    post_User.Base64 = user1.UserBase64;
                    post_User.Roles = (List<string>?)await userManager.GetRolesAsync(user1);
                    data.Add(post_User);
                }
                return data;
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });
            }
        }

        [Authorize]
        [Route("api/v1/admin/user/chinaname/{search}")]
        [HttpGet]
        public async Task<ActionResult<List<post_user>>> searchuser_bychinaname(string search)
        {
           
            if (this.User.FindAll(ClaimTypes.Role).Any(a=>a.Value=="admin"))
            {
                TestDbContext ctx = new TestDbContext();
                List<post_user> data = new List<post_user>();
                List<User> users = userManager.Users.Where(a => a.chinaname.IndexOf(search) >= 0).ToList();
                foreach (var user1 in users)
                {
                    post_user post_User = new post_user();
                    post_User.id = user1.Id;
                    post_User.name = user1.UserName;
                    post_User.email = user1.Email;
                    post_User.officium = user1.officium;
                    post_User.chinaname = user1.chinaname;

                    post_User.Base64 = user1.UserBase64;
                    post_User.Roles = (List<string>?)await userManager.GetRolesAsync(user1);
                    data.Add(post_User);
                }
                return data;
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });
            }
        }


        [Route("api/v1/admin/excel")]
        [HttpGet]
        public async Task<ActionResult<List<post_user>>> excel1(string event_name)
        {
            string guid = Guid.NewGuid().ToString();
            TestDbContext testDb = new TestDbContext();
            var result = testDb.Forms
      .Include(a => a.role)
      .Include(a => a.events)
      .Where(e => e.events.name == event_name)
      .Select(a => new form
      {
          team_name = a.team_name,
          piaoshu = a.piaoshu,
          team_tel = a.team_tel,
          role = a.role.Select(r => new role
          {
              role_name = r.role_name,
              role_id = r.role_id,
              role_lin = r.role_lin
          }).ToList()
      }).ToList();
            ExportToExcel(result,$"{AppDomain.CurrentDomain.BaseDirectory}excel/{guid}.xlsx");
            return Ok(guid);
        }
        /// <summary>
        /// 将指定目录压缩为Zip文件
        /// </summary>
        /// <param name="folderPath">文件夹地址 D:/1/ </param>
        /// <param name="zipPath">zip地址 D:/1.zip </param>
        public static void CompressDirectoryZip(string folderPath, string zipPath)
        {
            DirectoryInfo directoryInfo = new(zipPath);

            if (directoryInfo.Parent != null)
            {
                directoryInfo = directoryInfo.Parent;
            }

            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            ZipFile.CreateFromDirectory(folderPath, zipPath, System.IO.Compression.CompressionLevel.Optimal, false);
        }
        [Route("api/v1/admin/img_zip")]
        [HttpGet]
        [ResponseCache(Duration = 1720)]
        public async Task<ActionResult<string>> img_zip()
        {
            string guid = Guid.NewGuid().ToString();
            CompressDirectoryZip($"{AppDomain.CurrentDomain.BaseDirectory}loge/", $"{AppDomain.CurrentDomain.BaseDirectory}doc/{guid}.zip");
            return $"/doc/{guid}.zip";
        }
         
          


        public class team_count
        {
            public string eventname { get; set; }
            public int formnumber { get; set; }

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
        public async Task<ActionResult<List<form>>> Post(string events)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))

            {

                TestDbContext ctx = new TestDbContext();

                var teams = ctx.Forms.Include(a => a.role).Include(a=>a.events).Where(a=>a.events.name==events).ToList();
               foreach( var team in teams)
                {
                    team.events.forms = null;
                   foreach(var role in team.role)
                    {
                        role.form = null;
                    }
                }
            
                return teams;









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



      
    }
}