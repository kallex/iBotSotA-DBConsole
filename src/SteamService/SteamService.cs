using System;
using DataServiceCore;
using Steamworks;

namespace SteamService
{
    public class SteamService : ISteamService
    {
        public SteamService()
        {
            SteamClient.Init(1518060);
            var steamId = SteamClient.SteamId;
        }


        public void ValidateToken()
        {
        }
    }
}
