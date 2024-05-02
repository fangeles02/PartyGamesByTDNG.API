using Microsoft.AspNetCore.SignalR;
using PartyGamesByTDNG.API.DbContexts;
using PartyGamesByTDNG.API.Models.SignalR;
using PartyGamesByTDNG.API.Helper;


namespace PartyGamesByTDNG.API.SignalRHubs
{

    public class HubOperationsHub : Hub
    {
        private readonly PartyGamesByTdngContext _partygamesbytdng;

        public HubOperationsHub(PartyGamesByTdngContext context)
        {
            this._partygamesbytdng = context;
        }



        //operations by game master

        public async Task CreateGroup(string Token, string Username, string GroupName, string Passcode, string GameCode)
        {
            string return_method = "CreateGroupResponse";

            //token check
            if (TokenHelper.IsTokenValid(Token))
            {
                string room_code = $"{Context.ConnectionId}-{GroupName.ToUpper().Replace(" ", "")}";
                //this.
                var search_res = _partygamesbytdng.HubGroups.Where(x => x.RoomName == GroupName && x.IsActive == 1).Count();

                if (search_res == 0)
                {

                    try
                    {
                        //add group
                        _partygamesbytdng.HubGroups.Add(new HubGroup
                        {
                            RoomCode = room_code,
                            RoomName = GroupName,
                            Passcode = Passcode,
                            GameCode = GameCode,
                            Owner = Username,
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

                        await Groups.AddToGroupAsync(Context.ConnectionId, room_code);

                        await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                        {
                            Result = ResponseCode.Success,
                            ResultMessage = $"The game \"{GroupName}\" has been created successfuly."
                        }));
                    }
                    catch (Exception ex)
                    {
                        await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                        {
                            Result = ResponseCode.Failed,
                            ResultMessage = $"Error: " + ex.Message
                        }));
                    }

                }
                else
                {
                    await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                    {
                        Result = ResponseCode.Failed,
                        ResultMessage = "Cannot create game. Game with the same name already exist."
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


        public async Task CloseGroup(string Token, string GroupName, string Passcode, string GameCode)
        {
            string return_method = "CloseGroupResponse";

            if (TokenHelper.IsTokenValid(Token))
            {
                string room_code = $"{Context.ConnectionId}-{GroupName.ToUpper().Replace(" ", "")}";
                //remove from members

                try
                {
                    foreach (var cur in _partygamesbytdng.HubMembers.Where(m => m.RoomCode == room_code))
                    {

                        //remove from hub group
                        if (cur.ConnectionId != Context.ConnectionId)
                        {
                            await Groups.RemoveFromGroupAsync(cur.ConnectionId, room_code);
                        }
                    }

                    //remove from db
                    _partygamesbytdng.HubMembers.Remove(_partygamesbytdng.HubMembers.Where(x => x.RoomCode == room_code).FirstOrDefault());

                    //remove from Hub groups

                    _partygamesbytdng.HubGroups.Remove(_partygamesbytdng.HubGroups.Where(x => x.RoomCode == room_code).FirstOrDefault());
                    await _partygamesbytdng.SaveChangesAsync();

                    //remove last member of hub group
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, room_code);

                    await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                    {
                        Result = ResponseCode.Success,
                        ResultMessage = $"The game \"{GroupName}\" has been ended."
                    }));
                }
                catch (Exception ex)
                {
                    await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                    {
                        Result = ResponseCode.Failed,
                        ResultTitle = "Error",
                        ResultMessage = ex.Message
                    }));
                }


            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                {
                    Result = ResponseCode.Failed,
                    ResultTitle = "Invalid token",
                    ResultMessage = "Invalid token"
                }));
            }
        }







        //operations by Game member
        public async Task JoinGame(string Token, string RoomName, string PassCode, string GameCode, string PlayerName)
        {
            string return_method = "JoinGameResponse";

            if (TokenHelper.IsTokenValid(Token))
            {
                var gamedata = _partygamesbytdng.HubGroups.Where(g => g.RoomName.ToUpper().Trim() == RoomName.ToUpper().Trim() && g.Passcode.Trim() == PassCode.Trim()).FirstOrDefault();

                if (gamedata != null)
                {
                    if (gamedata.IsOpen == 1)
                    {

                        //add to members
                        _partygamesbytdng.HubMembers.Add(new HubMember
                        {
                            ConnectionId = Context.ConnectionId,
                            RoomCode = gamedata.RoomCode,
                            LastActivity = DateTime.Now,
                            Username = PlayerName.Trim()
                        });

                        gamedata.LastActivity = DateTime.Now;
                        _partygamesbytdng.HubGroups.Update(gamedata);


                        try
                        {
                            await _partygamesbytdng.SaveChangesAsync();

                            await Groups.AddToGroupAsync(Context.ConnectionId, gamedata.RoomCode);

                            await Clients.GroupExcept(gamedata.RoomName, Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                            {
                                Result = ResponseCode.Success,
                                ResultTitle = "Success",
                                ResultMessage = $"{PlayerName.Trim()} has joined the game",
                                ResponseParams = $"{PlayerName.Trim()},{Context.ConnectionId}"
                            }));

                            await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                            {
                                Result = ResponseCode.Success,
                                ResultTitle = "Success",
                                ResultMessage = $"You have joined the game \"{gamedata.RoomName}\""
                            }));

                        }
                        catch (Exception ex)
                        {
                            await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                            {
                                Result = ResponseCode.Failed,
                                ResultTitle = "Error",
                                ResultMessage = ex.Message
                            }));
                        }


                    }
                    else
                    {
                        await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                        {
                            Result = ResponseCode.Failed,
                            ResultTitle = "Error",
                            ResultMessage = "Sorry, the current game does not accept new players as of now."
                        }));
                    }
                }
                else
                {
                    await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                    {
                        Result = ResponseCode.Failed,
                        ResultTitle = "Error",
                        ResultMessage = "Invalid game credentials"
                    }));
                }
            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                {
                    Result = ResponseCode.Failed,
                    ResultTitle = "Invalid token",
                    ResultMessage = "Invalid token"
                }));
            }
        }


    }
}
