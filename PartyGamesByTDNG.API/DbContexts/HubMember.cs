using System;
using System.Collections.Generic;

namespace PartyGamesByTDNG.API.DbContexts;

public partial class HubMember
{
    public int Id { get; set; }

    public string RoomCode { get; set; } = null!;

    public string ConnectionId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public DateTime LastActivity { get; set; }
}
