using System;
using System.Collections.Generic;

namespace DataServiceCore
{
    public class MatchData
    {
        public string AccountID { get; set; }
        public string ItemID { get; set; }
        public ClientInfo ClientInfo { get; set; }
        public DateTime MatchStartTime { get; set; }
        public DateTime MatchEndTime { get; set; }
        public List<ChamberData> ChamberDatas { get; set; }
    }

    public class ClientInfo
    {
        public ulong SteamId { get; set; }
        public string ClientName { get; set; }
    }

    public class ChamberData
    {
        public TimeSpan Duration { get; set; }
        public int ChamberNo { get; set; }
        public List<PlayerChamberData> PlayerDatas { get; set;}
    }

    public class PlayerChamberData
    {
        public string Name { get; set; }
        public PlayerPerformance Performance { get; set; }

    }

    public class PlayerPerformance
    {
        public int Shots { get; set; }
        public int Hits { get; set; }
        public int HeadshotHits { get; set; }

    }
}