

using Microsoft.AspNetCore.SignalR;
namespace asg_form.Controllers.Hubs;

public class room : Hub
{
    public async Task Send(string message)
    {
        await Clients.All.SendAsync("Send", message);
    }
   

}
