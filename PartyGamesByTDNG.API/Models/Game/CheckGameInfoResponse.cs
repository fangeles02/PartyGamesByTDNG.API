namespace PartyGamesByTDNG.API;

public class CheckGameInfoResponse : BaseResponse
{
    public string? RoomCode { get; set; }
    public string? RoomName { get; set; }
    public string? Owner { get; set; }
    public string? GameID { get; set; }

}
