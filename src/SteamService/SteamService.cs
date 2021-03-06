using System;
using System.Net.Http;
using System.Threading.Tasks;
using DataServiceCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SteamWebAPI2.Utilities;
using Steamworks;

namespace SteamService
{
    public class SteamService : ISteamService
    {
        public uint AppId;
        private string SteamWebApiKey;
        private HttpClient HttpClient;

        public SteamService()
        {
            this.HttpClient = new HttpClient();
        }



        void ISteamService.InitService(uint appId, string steamWebApiKey)
        {
            AppId = appId;
            SteamWebApiKey = steamWebApiKey;
        }

        void ISteamService.InitSteamClient()
        {
            SteamClient.Init(AppId);
        }

        async Task<(ulong steamIdValue, byte[] authToken)> ISteamService.GetAuthTokenA()
        {
            var authToken = await SteamUser.GetAuthSessionTicketAsync();
            var steamId = SteamClient.SteamId;
            return (steamId.Value, authToken.Data);
        }


        async Task<bool> ISteamService.ValidateAuthToken(ulong steamIDValue, byte[] authToken)
        {
            var tcs = new TaskCompletionSource<bool>();


            SteamUser.OnValidateAuthTicketResponse += OnSteamUserOnOnValidateAuthTicketResponse;
            try
            {
                SteamUser.BeginAuthSession(authToken, steamIDValue);
            }
            catch
            {
                SteamUser.OnValidateAuthTicketResponse -= OnSteamUserOnOnValidateAuthTicketResponse;
                throw;
            }

            var result = await tcs.Task;
            return result;

            void OnSteamUserOnOnValidateAuthTicketResponse(SteamId userId, SteamId appId, AuthResponse authResponse)
            {
                SteamUser.EndAuthSession(steamIDValue);
                SteamUser.OnValidateAuthTicketResponse -= OnSteamUserOnOnValidateAuthTicketResponse;
                bool result = authResponse == AuthResponse.OK;
                tcs.SetResult(result);
            }
        }

        async Task<(bool authenticated, ulong steamid, ulong ownersteamid, bool vacbanned, bool publishedbanned)> ISteamService.ValidateAuthTokenWeb(string authTokenHex)
        {
            /*
            var webInterfaceFactory = new SteamWebInterfaceFactory(SteamWebApiKey);
            var userInterface = webInterfaceFactory.CreateSteamWebInterface<SteamWebAPI2.Interfaces.SteamUserAuth>(HttpClient);
            var authResult = await userInterface.AuthenticateUserTicket(AppId, authTokenHex);
            return authResult.Data.Response.Success;
            */
            using var httpClient = new HttpClient();

            var url = $"https://partner.steam-api.com/ISteamUserAuth/AuthenticateUserTicket/v1/?key={SteamWebApiKey}&appid={AppId}&ticket={authTokenHex}";
            var response = await httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var json = (JObject) JsonConvert.DeserializeObject(content);
            var responseData = json["response"]["params"];

            var authenticated = responseData["result"]?.ToString() == "OK";
            var steamIdStr = responseData["steamid"]?.ToString() ?? "0";
            if (!ulong.TryParse(steamIdStr, out var steamid))
                steamid = 0;
            var ownerSteamIdStr = responseData["ownersteamid"]?.ToString() ?? "0";
            if (!ulong.TryParse(ownerSteamIdStr, out var ownersteamid))
                ownersteamid = 0;

            var vacbanned = responseData["vacbanned"]?.ToString() == "true";
            var publishedBanned = responseData["publisherbanned"]?.ToString() == "true";

            var result = (authenticated, steamid, ownersteamid, vacbanned, publishedBanned);
            return result;
        }
    }
}
