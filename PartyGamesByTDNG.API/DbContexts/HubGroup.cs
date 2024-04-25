using System;
using System.Collections.Generic;

namespace PartyGamesByTDNG.API.DbContexts;

public partial class HubGroup
{
    public int Id { get; set; }

    public string RoomCode { get; set; } = null!;

    public string RoomName { get; set; } = null!;

    public string Passcode { get; set; } = null!;

    public string GameCode { get; set; } = null!;

    public string Owner { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public DateTime LastActivity { get; set; }

    public int IsActive { get; set; }

    public int IsOpen { get; set; }
}
