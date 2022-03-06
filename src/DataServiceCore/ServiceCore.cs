using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Utf8Json;

namespace Services
{
    public static class ServiceCore
    {

        public static async Task<T> FromJsonAsync<T>(Stream jsonStream)
        {
            T result = await JsonSerializer.DeserializeAsync<T>(jsonStream);
            return result;
        }

        public static T FromJson<T>(byte[] jsonData)
        {
            T result = JsonSerializer.Deserialize<T>(jsonData);
            return result;
        }

        public static T FromJson<T>(string jsonData)
        {
            T result = JsonSerializer.Deserialize<T>(jsonData);
            return result;
        }


        public static byte[] ToJsonBinary(object data)
        {
            var result = JsonSerializer.Serialize(data);
            return result;
        }

        public static string ToJsonString(object data)
        {
            var result = JsonSerializer.ToJsonString(data);
            return result;
        }

        public static async Task ToJsonStreamAsync(Stream stream, object data)
        {
            await JsonSerializer.SerializeAsync(stream, data);
        }

        public static byte[] GZipData(byte[] data)
        {
            using (var memStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(memStream, CompressionLevel.Optimal))
                {
                    gzipStream.Write(data, 0, data.Length);
                    gzipStream.Flush();
                    gzipStream.Close();
                }
                return memStream.ToArray();
            }
        }

        public static string GetRuntimeEnvironment()
        {
            var environment = Environment.GetEnvironmentVariable("Environment")?.ToLowerInvariant() ?? "dev";
            return environment;
        }
    }
}