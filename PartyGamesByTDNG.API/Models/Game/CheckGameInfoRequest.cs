namespace PartyGamesByTDNG.API;

public class CheckGameInfoRequest : BaseRequest
{
    public required string GameID { get; set; }
    public required string Passcode { get; set; }
}
