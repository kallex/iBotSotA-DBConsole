using System.Threading.Tasks;

namespace DataServiceCore
{
    public interface ISteamService
    {
        void InitService(uint appId);
        Task<(ulong steamIdValue, byte[] authToken)> GetAuthTokenA();
        Task<bool> ValidateAuthToken(ulong steamIDValue, byte[] authToken);
    }
}