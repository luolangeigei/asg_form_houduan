using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using asg_form.Migrations;
using Mirai.Net.Sessions.Http.Managers;

public class MyExceptionFilter : IAsyncExceptionFilter
{
    private readonly ILogger<MyExceptionFilter> logger;
    private readonly IHostEnvironment env;
    public MyExceptionFilter(ILogger<MyExceptionFilter> logger, IHostEnvironment env)
    {
        this.logger = logger;
        this.env = env;
    }
    public Task OnExceptionAsync(ExceptionContext context)
    {
        Exception exception = context.Exception;
        logger.LogError(exception, "UnhandledException occured");
      
        ObjectResult result = new ObjectResult(new { code = 500, message = exception.Message });
        try
        {
            MessageManager.SendGroupMessageAsync("860395385", $"<拉花实验室-错误>\r\n警告等级：不严重\r\n内容：{exception.Message}");

        }
        catch
        {

        }


        result.StatusCode = 500;
        context.Result = result;
        context.ExceptionHandled = true;
        return Task.CompletedTask;
    }

}






public class error_mb
{
    public int code { get; set; }
    public string message { get; set; }
}
