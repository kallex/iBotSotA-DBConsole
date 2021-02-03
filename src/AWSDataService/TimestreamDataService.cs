using System;
using System.IO;
using DataServiceCore;

namespace AWSDataService
{
    public class TimestreamDataService : ITimeseriesDataService
    {
        public string GetAccountData(string accountID, string dataType, DateTime startTimeUtc, DateTime endTimeUtc,
            (string dimensionName, string dimensionValue)[] dimensionFilters)
        {
            throw new NotImplementedException();
        }

        public int InsertAccountData(string accountID, string dataType, (string dimensionName, string dimensionValue)[] dimensions,
            (string name, string value, string type, DateTime dateTimeUtc)[] records)
        {
            throw new NotImplementedException();
        }

        public int DeleteAccountData(string accountID, string dataType, DateTime startTimeUtc, DateTime endTimeUtc,
            (string dimensionName, string dimensionValue)[] dimensionFilters)
        {
            throw new NotImplementedException();
        }

        public void QueryAccountData(string accountID, string dataType, DateTime startTimeUtc, DateTime endTimeUtc, string queryString,
            Stream resultStream)
        {
            throw new NotImplementedException();
        }
    }
}