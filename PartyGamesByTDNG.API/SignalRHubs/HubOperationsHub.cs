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
                            ResultMessage = $"The game \"{GroupName}\" has been created successfuly.",
                            Recipient = Recipient.Self
                        }));
                    }
                    catch (Exception ex)
                    {
                        await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                        {
                            Result = ResponseCode.Failed,
                            ResultMessage = $"Error: " + ex.Message,
                            Recipient = Recipient.Self
                        }));
                    }

                }
                else
                {
                    await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                    {
                        Result = ResponseCode.Failed,
                        ResultMessage = "Cannot create game. Game with the same name already exist.",
                        Recipient = Recipient.Self
                    }));
                }
            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync("CreateGroupResponse", ResponseBuilder.Build(new Response
                {
                    Result = ResponseCode.Failed,
                    ResultTitle = "Invalid token",
                    ResultMessage = "Invalid token",
                    Recipient = Recipient.Self
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
                            //send message to each member of group
                            await Clients.Client(cur.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                            {
                                Result = ResponseCode.Success,
                                ResultTitle = "Success",
                                ResultMessage = $"The game \"{GroupName}\" has been ended by the gamemaster.",
                                Recipient = Recipient.Group
                            }));

                            await Groups.RemoveFromGroupAsync(cur.ConnectionId, room_code);
                        }
                    }

                    //remove from db
                    _partygamesbytdng.HubMembers.RemoveRange(_partygamesbytdng.HubMembers.Where(x => x.RoomCode == room_code).ToList());

                    //remove from Hub groups

                    _partygamesbytdng.HubGroups.Remove(_partygamesbytdng.HubGroups.Where(x => x.RoomCode == room_code).FirstOrDefault());
                    await _partygamesbytdng.SaveChangesAsync();

                    //remove last member of hub group
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, room_code);

                    await _partygamesbytdng.SaveChangesAsync();

                    await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                    {
                        Result = ResponseCode.Success,
                        ResultMessage = $"The game \"{GroupName}\" has been ended.",
                        Recipient = Recipient.Self
                    }));
                }
                catch (Exception ex)
                {
                    await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                    {
                        Result = ResponseCode.Failed,
                        ResultTitle = "Error",
                        ResultMessage = ex.Message,
                        Recipient = Recipient.Self
                    }));
                }


            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                {
                    Result = ResponseCode.Failed,
                    ResultTitle = "Invalid token",
                    ResultMessage = "Invalid token",
                    Recipient = Recipient.Self
                }));
            }
        }



        public async Task KickMemberOut(string Token, string GroupName, string Passcode, string GameCode, string ConnectionId)
        {
            string return_method = "KickMemberOutResponse";

            if (TokenHelper.IsTokenValid(Token))
            {
                string room_code = $"{Context.ConnectionId}-{GroupName.ToUpper().Replace(" ", "")}";
                //remove from members

                try
                {
                    //get player name 
                    string s_player_name = _partygamesbytdng.HubMembers.Where(x => x.ConnectionId == ConnectionId).FirstOrDefault().Username;

                    //send message to each member of group
                    await Clients.Client(ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                    {
                        Result = ResponseCode.Success,
                        ResultTitle = "Success",
                        ResultMessage = $"You have been removed from the game by the gamemaster.",
                        Recipient = Recipient.Self
                    }));

                    //remove from hub group
                    await Groups.RemoveFromGroupAsync(ConnectionId, room_code);

                    //remove from db
                    _partygamesbytdng.HubMembers.Remove(_partygamesbytdng.HubMembers.Where(x => x.ConnectionId == ConnectionId).FirstOrDefault());

                    await _partygamesbytdng.SaveChangesAsync();

                    await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                    {
                        Result = ResponseCode.Success,
                        ResultMessage = $"{s_player_name} has been removed.",
                        ResponseParams = ConnectionId,
                        Recipient = Recipient.Self
                    }));
                }
                catch (Exception ex)
                {
                    await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                    {
                        Result = ResponseCode.Failed,
                        ResultTitle = "Error",
                        ResultMessage = ex.Message,
                        Recipient = Recipient.Self
                    }));
                }


            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                {
                    Result = ResponseCode.Failed,
                    ResultTitle = "Invalid token",
                    ResultMessage = "Invalid token",
                    Recipient = Recipient.Self
                }));
            }
        }









        //operations by Game member
        public async Task JoinGroup(string Token, string RoomName, string PassCode, string GameCode, string PlayerName)
        {
            string return_method = "JoinGroupResponse";

            if (TokenHelper.IsTokenValid(Token))
            {
                var gamedata = _partygamesbytdng.HubGroups.Where(g => g.RoomName.ToUpper().Trim() == RoomName.ToUpper().Trim() && g.Passcode.Trim() == PassCode.Trim()).FirstOrDefault();

                if (gamedata != null)
                {
                    if (gamedata.IsOpen == 1)
                    {


                        //check if already joined
                        if (_partygamesbytdng.HubMembers.Where(x => x.ConnectionId == Context.ConnectionId).FirstOrDefault() is null)
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
                            // _partygamesbytdng.HubGroups.Update(gamedata);


                            try
                            {
                                await _partygamesbytdng.SaveChangesAsync();

                                await Groups.AddToGroupAsync(Context.ConnectionId, gamedata.RoomCode);


                                var resss = _partygamesbytdng.HubMembers.Where(x => x.RoomCode == gamedata.RoomCode).Select(x => x.ConnectionId).ToList();

                                foreach (string cur in resss)
                                {
                                    if (cur != Context.ConnectionId)
                                    {
                                        await Clients.Client(cur).SendAsync(return_method, ResponseBuilder.Build(new Response
                                        {
                                            Result = ResponseCode.Success,
                                            ResultTitle = "Success",
                                            ResultMessage = $"{PlayerName.Trim()} has joined the game",
                                            ResponseParams = $"{PlayerName.Trim()},{Context.ConnectionId}",
                                            Recipient = Recipient.Group
                                        }));

                                    }
                                }

                                // await Clients.GroupExcept(gamedata.RoomName, Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                                // {
                                //     Result = ResponseCode.Success,
                                //     ResultTitle = "Success",
                                //     ResultMessage = $"{PlayerName.Trim()} has joined the game",
                                //     ResponseParams = $"{PlayerName.Trim()},{Context.ConnectionId}"
                                // }));

                                await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                                {
                                    Result = ResponseCode.Success,
                                    ResultTitle = "Success",
                                    ResultMessage = $"You have joined the game \"{gamedata.RoomName}\"",
                                    Recipient = Recipient.Self
                                }));

                            }
                            catch (Exception ex)
                            {
                                await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                                {
                                    Result = ResponseCode.Failed,
                                    ResultTitle = "Error",
                                    ResultMessage = ex.Message,
                                    Recipient = Recipient.Self
                                }));
                            }

                        }
                        else
                        {
                            await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                            {
                                Result = ResponseCode.Failed,
                                ResultTitle = "Error",
                                ResultMessage = "Sorry, you already joined this game.",
                                Recipient = Recipient.Self
                            }));
                        }


                    }
                    else
                    {
                        await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                        {
                            Result = ResponseCode.Failed,
                            ResultTitle = "Error",
                            ResultMessage = "Sorry, the current game does not accept new players as of now.",
                            Recipient = Recipient.Self
                        }));
                    }
                }
                else
                {
                    await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                    {
                        Result = ResponseCode.Failed,
                        ResultTitle = "Error",
                        ResultMessage = "Invalid game credentials",
                        Recipient = Recipient.Self
                    }));
                }
            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                {
                    Result = ResponseCode.Failed,
                    ResultTitle = "Invalid token",
                    ResultMessage = "Invalid token",
                    Recipient = Recipient.Self
                }));
            }
        }



        public async Task MemberLeavesGroup(string Token, string RoomName, string GameCode, string PlayerName)
        {
            string return_method = "MemberLeavesGroupResponse";

            if (TokenHelper.IsTokenValid(Token))
            {
                try
                {
                    foreach (var cur in _partygamesbytdng.HubMembers.Where(m => m.RoomCode == GameCode))
                    {
                        if (cur.ConnectionId != Context.ConnectionId)
                        {
                            //send message to each member of group
                            await Clients.Client(cur.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                            {
                                Result = ResponseCode.Success,
                                ResultTitle = "Success",
                                ResultMessage = $"{PlayerName} has left the game",
                                ResponseParams = Context.ConnectionId,
                                Recipient = Recipient.Group
                            }));
                        }
                    }



                    //remove from hub group
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, GameCode);

                    //remove from db
                    _partygamesbytdng.HubMembers.Remove(_partygamesbytdng.HubMembers.Where(x => x.ConnectionId == Context.ConnectionId).FirstOrDefault());

                    await _partygamesbytdng.SaveChangesAsync();

                    await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                    {
                        Result = ResponseCode.Success,
                        ResultMessage = $"You left the game",
                        Recipient = Recipient.Self
                    }));
                }
                catch (Exception ex)
                {
                    await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                    {
                        Result = ResponseCode.Failed,
                        ResultTitle = "Error",
                        ResultMessage = ex.Message,
                        Recipient = Recipient.Self
                    }));
                }


            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                {
                    Result = ResponseCode.Failed,
                    ResultTitle = "Invalid token",
                    ResultMessage = "Invalid token",
                    Recipient = Recipient.Self
                }));
            }
        }



        //Stop accepting member
        public async Task StartGroupGame(string Token, string RoomName, string PassCode)
        {
            string return_method = "StartGroupGameResponse";

            if (TokenHelper.IsTokenValid(Token))
            {
                var gamedata = _partygamesbytdng.HubGroups.Where(g => g.RoomName.ToUpper().Trim() == RoomName.ToUpper().Trim() && g.Passcode.Trim() == PassCode.Trim()).FirstOrDefault();

                gamedata.IsOpen = 0;
                await _partygamesbytdng.SaveChangesAsync();

                await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                {
                    Result = ResponseCode.Success,
                    ResultTitle = "",
                    ResultMessage = "You have stopped accepting players and started the game.",
                    Recipient = Recipient.Self
                }));

                //send signal to each players
                foreach (var cur in _partygamesbytdng.HubMembers.Where(x => x.RoomCode == gamedata.RoomCode))
                {
                    if (cur.ConnectionId != Context.ConnectionId)
                    {
                        await Clients.Client(cur.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                        {
                            Result = ResponseCode.Success,
                            ResultTitle = "",
                            ResultMessage = "Gamemaster has initiated to start the game",
                            Recipient = Recipient.Group
                        }));
                    }

                }

            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync(return_method, ResponseBuilder.Build(new Response
                {
                    Result = ResponseCode.Failed,
                    ResultTitle = "Invalid token",
                    ResultMessage = "Invalid token",
                    Recipient = Recipient.Self
                }));
            }
        }


    }
}
