using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartyGamesByTDNG.API.DbContexts;

namespace PartyGamesByTDNG.API;

[ApiController]
[Route("[controller]")]
[Authorize]
public class GameController : ControllerBase
{
    private readonly PartyGamesByTdngContext _partygamesbytdng;

    public GameController(PartyGamesByTdngContext context)
    {
        this._partygamesbytdng = context;
    }



    [HttpPost("CheckGameInfo")]
    public CheckGameInfoResponse CheckGameInfo(CheckGameInfoRequest request)
    {
        var searchres = _partygamesbytdng.HubGroups.Where(x => x.RoomName.ToUpper().Trim() == request.GameID.Trim().ToUpper() && x.Passcode.Trim() == request.Passcode.Trim() && x.IsActive == 1).FirstOrDefault();

        if (searchres is not null)
        {
            if (searchres.IsOpen == 1)
            {
                return new CheckGameInfoResponse
                {
                    ResultCode = "OK",
                    ResultMessage = "game found and currently accepting members.",
                    Owner = searchres.Owner,
                    RoomCode = searchres.RoomCode,
                    RoomName = searchres.RoomName,
                    GameID = searchres.GameCode,
                };
            }
            else
            {
                return new CheckGameInfoResponse
                {
                    ResultCode = "ERR",
                    ResultMessage = "The game details you entered does not currently accept new members"
                };
            }
        }
        else
        {
            return new CheckGameInfoResponse
            {
                ResultCode = "ERR",
                ResultMessage = "The game details you entered does not exist"
            };

        }


    }

}
