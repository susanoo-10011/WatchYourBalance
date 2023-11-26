using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WatchYourBalance.Models.SaveSettings;

namespace WatchYourBalance.Models
{
    class ApiBinance
    {
        public async Task GetAccountData(string apiKey, string apiSecret)
        {
            string endpoint = "https://fapi.binance.com/fapi/v2/balance"; // это URL конечной точки API бинанса для получения баланса фьючерсного аккаунта.

            using (HttpClient client = new HttpClient()) //создается объект HTTPClient, который используется для отправки HTTP-запросов.
            {
                string queryString = $"timestamp={GetTimestamp()}"; //создается строка запроса с параметром времени timestamp, который используется для предотвращения повторных запросов
                string signature = Sign(queryString, apiSecret); //подпись запроса вычисляется подпись запроса HMAC-SHA256 для строки запроса с использованием секретного ключа

                string url = $"{endpoint}?{queryString}&signature={signature}"; //Составляется полный URL запрос, включая строку запроса и подпись

                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey); //Добавляется заголовок с API ключом для аунтификации

                HttpResponseMessage response = await client.GetAsync(url); //Выполняетя Get запрос к бинанс апи

                if (response.IsSuccessStatusCode) //обработка ответа. Проверяется успешность ответа от сервера
                {
                    string content = await response.Content.ReadAsStringAsync(); // Если ответ успешен, читается содержимое ответа в виде строки

                    AccountData.ReadingResponse(content);
                }
                else
                {
                    //вывести через месседжБокс
                    // Console.WriteLine($"Ошибка: {response.StatusCode}");

                }
            }
        }

        static string Sign(string data, string apiSecret)
        {
            using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(apiSecret)))
            {
                byte[] hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        static long GetTimestamp()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
    }
}
