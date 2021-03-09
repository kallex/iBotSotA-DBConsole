using System;
using System.IO;

namespace Services
{
    public interface ITimeseriesDataService
    {
        string GetAccountData(string accountID, string dataType, DateTime startTimeUtc, DateTime endTimeUtc, (string dimensionName, string dimensionValue)[] dimensionFilters);

        int InsertAccountData(string accountID, string dataType,
            (string dimensionName, string dimensionValue)[] dimensions,
            (string name, string value, string type, DateTime dateTimeUtc)[] records);
        int DeleteAccountData(string accountID, string dataType, DateTime startTimeUtc, DateTime endTimeUtc, (string dimensionName, string dimensionValue)[] dimensionFilters);

        void QueryAccountData(string accountID, string dataType, DateTime startTimeUtc, DateTime endTimeUtc,
            string queryString, Stream resultStream);

    }
}
