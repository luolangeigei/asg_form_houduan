using AngleSharp.Css.Parser;
using ChatGPT.Net;
using ChatGPT.Net.DTO.ChatGPT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using static asg_form.Controllers.login;

namespace asg_form.Controllers
{
    public class chatgpt : ControllerBase
    {
        List<chatgptbot> allchatgpt=new List<chatgptbot>();

        [Route("api/v1/Addchat")]
        [HttpPost]
        //[Authorize]
        public async Task<ActionResult<string>> newgptbot()
        {
            ChatGpt bot = new ChatGpt("ak-xyk1gy2eWM8k7PTumTKXqU47mmkCXvNsrqIxRwWcDRqoviSN", new ChatGptOptions
            {
                BaseUrl = "https://api.nextweb.fun/openai"
            });
            var newbot = new chatgptbot { id = Guid.NewGuid(), bot = bot };
            allchatgpt.Add(newbot);
           
            return newbot.id.ToString();
        }
        [Route("api/v1/reqchat")]
        [HttpPost]
        //[Authorize]
        public async Task<ActionResult<string>> reqchatgpt([FromBody]req req)
        {
            try
            {

                ChatGpt bot = new ChatGpt("ak-xyk1gy2eWM8k7PTumTKXqU47mmkCXvNsrqIxRwWcDRqoviSN", new ChatGptOptions
                {
                    BaseUrl = "https://api.nextweb.fun/openai"
                });
                var msg = await bot.Ask(req.chatreq);
                return msg;
            }
            catch
            {
                return Ok(allchatgpt);
            }

        }

       public class chatgptbot
        {
            public Guid id { get; set; }
            public ChatGpt bot { get; set; }

        }
       public class req
        {
            public string botid { get; set; } 
            public string chatreq { get; set;}
        }
    }
}

 

