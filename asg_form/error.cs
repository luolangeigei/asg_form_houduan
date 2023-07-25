using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

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
        string message = $"出现未处理异常:{exception.Message}";
    
        ObjectResult result = new ObjectResult(new { code = 500, message = message });
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
