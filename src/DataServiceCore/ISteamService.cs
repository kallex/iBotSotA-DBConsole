using System.Threading.Tasks;

namespace DataServiceCore
{
    public interface ISteamService
    {
        void InitService(uint appId, string steamWebApiKey);
        Task<(ulong steamIdValue, byte[] authToken)> GetAuthTokenA();
        Task<bool> ValidateAuthToken(ulong steamIDValue, byte[] authToken);

        Task<bool> ValidateAuthTokenWeb(string authTokenHex);
        void InitSteamClient();
    }
}