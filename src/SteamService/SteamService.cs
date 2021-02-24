using System;
using DataServiceCore;
using Steamworks;

namespace SteamService
{
    public class SteamService : ISteamService
    {
        public static string AccountId;

        public SteamService()
        {
            SteamClient.Init(1518060);
            var steamId = SteamClient.SteamId;
            AccountId = steamId.AccountId.ToString();
        }


        public void ValidateToken()
        {
        }
    }
}
