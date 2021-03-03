using System;
using System.Threading.Tasks;
using DataServiceCore;
using Steamworks;

namespace SteamService
{
    public class SteamService : ISteamService
    {
        public uint AppId;

        public SteamService()
        {
        }



        void ISteamService.InitService(uint appId)
        {
            AppId = appId;
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
    }
}
