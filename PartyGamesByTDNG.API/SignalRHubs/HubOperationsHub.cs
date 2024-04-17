using Microsoft.AspNetCore.SignalR;
using PartyGamesByTDNG.API.DbContexts;
using PartyGamesByTDNG.API.Models.SignalR;

namespace PartyGamesByTDNG.API.SignalRHubs
{

    public class HubOperationsHub : Hub
    {
        private readonly PartyGamesByTdngContext _partygamesbytdng;

        public HubOperationsHub(PartyGamesByTdngContext context)
        {
            this._partygamesbytdng = context;
        }




        public async Task CreateGroup(string Token, string Username, string GroupName, string Passcode, string GameCode)
        {
            string return_method = "CreateGroupResponse";

            //token check
            if (1 == 1)
            {
                string room_code = $"{DateTime.Now.ToString("MM dd yyyy HH mm ss").Replace(" ", "")}-{GroupName.ToUpper().Replace(" ", "")}";
                //this.
                var search_res = _partygamesbytdng.HubGroups.Where(x => x.RoomName == GroupName && x.Passcode == Passcode).FirstOrDefault();

                if (search_res is null)
                {

                    //add group
                    _partygamesbytdng.HubGroups.Add(new HubGroup
                    {
                        RoomCode = room_code,
                        RoomName = GroupName,
                        Passcode = Passcode,
                        GameCode = GameCode,
                        DateCreated = DateTime.Now,
                        LastActivity = DateTime.Now,
                        IsActive = 1,
                        IsOpen = 1
                    });

                    await _partygamesbytdng.SaveChangesAsync();


                    //add user
                    _partygamesbytdng.HubMembers.Add(new HubMember
                    {
                        ConnectionId = Context.ConnectionId,
                        RoomCode = room_code,
                        Username = Username,
                        LastActivity = DateTime.Now
                    });

                    await _partygamesbytdng.SaveChangesAsync();

                    await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                    {
                        Result = ResponseCode.Success,
                        ResultMessage = $"The room {GroupName} has been created successfuly."
                    }));
                }
                else
                {
                    await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                    {
                        Result = ResponseCode.Failed,
                        ResultMessage = "Cannot create room. Already exist."
                    }));
                }
            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync("CreateGroupResponse", ResponseBuilder.Build(new Response
                {
                    Result = ResponseCode.Failed,
                    ResultTitle = "Invalid token",
                    ResultMessage = "Invalid token"
                }));
            }
        }


    }
}
