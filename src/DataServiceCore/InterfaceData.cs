using System;
using System.Collections.Generic;

namespace DataServiceCore
{
    public enum MethodName
    {
        SubmitMatchData,
    }

    public class MatchData
    {
        public string AccountID { get; set; }
        public string ItemID { get; set; }
        public ClientInfo ClientInfo { get; set; }
        public DateTime MatchStartTime { get; set; }
        public DateTime MatchEndTime { get; set; }
        public List<ChamberData> ChamberDatas { get; set; } = new List<ChamberData>();
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
        public List<PlayerChamberData> PlayerDatas { get; set; } = new List<PlayerChamberData>();
        public List<PositionalData> PositionalDatas { get; set; } = new List<PositionalData>();
    }

    public class PositionalData
    {
        public string Name { get; set; }
        public DateTime TimeStamp { get; set; }
        public int FrameNumber { get; set; }

        public Vec3 Position { get; set; }
        public Quart4 Rotation { get; set; }
    }

    public class Vec3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }

    public class Quart4
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }
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
        public int HeadShotHits { get; set; }

    }
}