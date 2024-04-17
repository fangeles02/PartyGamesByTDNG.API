using Microsoft.AspNetCore.SignalR;
using PartyGamesByTDNG.API.DbContexts;
using PartyGamesByTDNG.API.Models.SignalR;

namespace PartyGamesByTDNG.API.SignalRHubs;

public class SystemHub : Hub
{
    private readonly PartyGamesByTdngContext _partygamesbytdng;

    public SystemHub(PartyGamesByTdngContext context)
    {
        this._partygamesbytdng = context;
    }

    public async Task SendSelfMessage(string Message)
    {
        Console.WriteLine($"Send self message to ID: {Context.ConnectionId}\nMessage: {Message}");
        await Clients.Client(Context.ConnectionId).SendAsync("SelfMessageReceived", "You have sent this message: " + Message);
    }




    public async Task Sample(string Message)
    {
        Console.WriteLine($"Send self message to ID: {Context.ConnectionId}\nMessage: {Message}");
        await Clients.Client(Context.ConnectionId).SendAsync("SelfMessageReceived", "You have sent this message: " + Message);
    }


}
