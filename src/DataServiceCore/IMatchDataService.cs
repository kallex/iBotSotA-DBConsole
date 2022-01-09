using System;
using System.Threading.Tasks;
using DataServiceCore;

namespace Services
{
    public interface IMatchDataService
    {
        Task StoreMatchData(MatchData matchData);
    }
}