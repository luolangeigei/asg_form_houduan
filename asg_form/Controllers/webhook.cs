using Manganese.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NLog;
using RestSharp;
using System.Diagnostics;

namespace asg_form.Controllers
{
    public class webhook : ControllerBase
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 获取所有video
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/biliup/")]
        [HttpPost]
        public async Task<ActionResult<List<string>>> get_video([FromBody] Rootobject rootobject)
        {
            logger.Warn("bilibili webhook" + rootobject.EventType);
            if (rootobject.EventType == "FileClosed")
            {
                logger.Warn(rootobject.EventType);
                Random random = new Random();
                string[] strings = { "upload", "--copyright 1", "--dolby 0", "--hires 0", $"--title {rootobject.EventData.Title}-直播回放{random.Next(1,99)}", "--tag 第五人格,第五人格ASG赛事", @$"{AppDomain.CurrentDomain.BaseDirectory}video\{rootobject.EventData.RelativePath}" };
                StartProcess("biliup.exe", strings);


                return Ok("ok");
            }
            else
            {
                return Ok("ok");
            }


        }



        public class biliup
        {
            public int copyright { get; set; }
            public string source { get; set; }
            public int tid { get; set; }
            public string cover { get; set; }
            public string title { get; set; }
            public int desc_format_id { get; set; }
            public string desc { get; set; }
            public string dynamic { get; set; }
            public bool open_subtitle { get; set; }
            public string tag { get; set; }
            public int interactive { get; set; }
            public int dolby { get; set; }
            public bool up_selection_reply { get; set; }
            public bool up_close_reply { get; set; }
            public bool up_close_danmu { get; set; }
            public string video_path { get; set; }
            public string cover_path { get; set; }
        }

        public bool StartProcess(string runFilePath, params string[] args)
        {
            string s = "";
            foreach (string arg in args)
            {
                s = s + arg + " ";
            }
            s = s.Trim();
            Process process = new Process();//创建进程对象    
            ProcessStartInfo startInfo = new ProcessStartInfo(runFilePath, s); // 括号里是(程序名,参数)
            process.StartInfo = startInfo;
            process.Start();
            return true;
        }

        public class Rootobject
        {
            public string EventType { get; set; }
            public DateTime EventTimestamp { get; set; }
            public string EventId { get; set; }
            public Eventdata EventData { get; set; }
        }

        public class Eventdata
        {
            public string RelativePath { get; set; }
            public int FileSize { get; set; }
            public float Duration { get; set; }
            public DateTime FileOpenTime { get; set; }
            public DateTime FileCloseTime { get; set; }
            public string SessionId { get; set; }
            public int RoomId { get; set; }
            public int ShortId { get; set; }
            public string Name { get; set; }
            public string Title { get; set; }
            public string AreaNameParent { get; set; }
            public string AreaNameChild { get; set; }
            public bool Recording { get; set; }
            public bool Streaming { get; set; }
            public bool DanmakuConnected { get; set; }
        }



    }
}
