using System;
using System.Net.Http;
using System.Threading.Tasks;
using DryIoc;
using Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SteamWebAPI2.Utilities;
using Steamworks;

namespace SteamServices
{
    public class SteamService : ISteamService
    {
        public uint AppId;
        private string SteamWebApiKey;
        private HttpClient HttpClient;
        public IDiagnosticService DiagnosticService { get; }

        public SteamService(IDiagnosticService diagnosticService)
        {
            this.HttpClient = new HttpClient();
            this.DiagnosticService = diagnosticService;
        }



        void ISteamService.InitService(uint appId, string steamWebApiKey)
        {
            AppId = appId;
            SteamWebApiKey = steamWebApiKey;
        }

        void ISteamService.InitSteamClient()
        {
            if(!SteamClient.IsValid)
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

        async Task<(bool isAuthenticated, ulong steamId, ulong ownerSteamId, bool vacBanned, bool publisherBanned)> ISteamService.ValidateAuthTokenWeb(string authTokenHex)
        {
            return await DiagnosticService.ExecAsync(nameof(SteamService), async diagnosticService =>
            {
                var webInterfaceFactory = new SteamWebInterfaceFactory(SteamWebApiKey);
                var userInterface = webInterfaceFactory.CreateSteamWebInterface<SteamWebAPI2.Interfaces.SteamUserAuth>(HttpClient);
                var authRequestResponse = await userInterface.AuthenticateUserTicket(AppId, authTokenHex);
                var authResponse = authRequestResponse.Data.Response;
                var authResult = authResponse.Params;

                var isAuthenticated = authResponse.Success;
                if (!ulong.TryParse(authResult.SteamId, out var steamId))
                    steamId = default;
                if (!ulong.TryParse(authResult.OwnerSteamId, out var ownerSteamId))
                    ownerSteamId = default;
                var vacBanned = authResult.VacBanned;
                var publisherBanned = authResult.PublisherBanned;

                var result = (isAuthenticated, steamId, ownerSteamId,
                    vacBanned, publisherBanned);
                return result;
            });
        }
    }
}
