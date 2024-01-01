using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchYourBalance.Models.Market.Servers.Binance.Futures.Entity
{
    [Serializable]
    public class ApiKeysEntity
    {
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }

    }

    public class ApiJson
    {
        public static void ApiSerializeJson(string apiKey, string apiSecret)
        {
            ApiKeysEntity apiKeys = new ApiKeysEntity()
            {
                ApiKey = apiKey,
                ApiSecret = apiSecret
            };

            string apiJson = JsonConvert.SerializeObject(apiKeys);


            string folderPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\JsonFiles";

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, "ApiJson.json");

            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(apiJson);

                }
            }
        }

        public static ApiKeysEntity ApiKeys()
        {
            string pathFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\JsonFiles" + "\\ApiJson.json";

            if (File.Exists(pathFile))
            {

                try
                {
                    using (StreamReader reader = new StreamReader(pathFile))
                    {
                        string apiKeysStr = reader.ReadToEnd();

                        ApiKeysEntity apiKeys = JsonConvert.DeserializeObject<ApiKeysEntity>(apiKeysStr);
                        return apiKeys;
                    }
                }
                catch (Exception ex)
                {
                    //Вывести ошибку
                    return null;
                }
            }
            return null;
        }
    }
}

