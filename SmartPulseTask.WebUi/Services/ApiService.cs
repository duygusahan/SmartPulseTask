using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartPulseTask.WebUi.Dtos;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static SmartPulseTask.WebUi.Models.ApiModel;

namespace SmartPulseTask.WebUi.Services
{


    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<TransactionHistoryGipDataDto>> GetDataAsync(string startDate, string endDate)
        {
            var username = "duygusahan0724@outlook.com";
            var password = "361544Dy*";

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var postData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password),
            };

            var content = new FormUrlEncodedContent(postData);

            var response = await _httpClient.PostAsync("https://giris.epias.com.tr/cas/v1/tickets", content);

            var responseContent = await response.Content.ReadAsStringAsync();

            var tgt = "";
            using (JsonDocument document = JsonDocument.Parse(responseContent))
            {
                tgt = document.RootElement.GetProperty("tgt").GetString();

                Console.WriteLine("TGT : " + tgt);
            }

            var json = "{\r\n    \"startDate\":\"2024-11-12T00:00:00+03:00\",\r\n    \"endDate\":\"2024-11-12T00:00:00+03:00\",\r\n    \"page\":{\r\n        \"size\":20,\r\n        \"number\":1,\r\n        \"sort\":{\r\n            \"field\":\"date\",\r\n            \"direction\":\"ASC\"\r\n        }\r\n    }\r\n\r\n}";
            
            var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");

            // Form verilerini içeren içerik oluşturuluyor
            _httpClient.DefaultRequestHeaders.Add("TGT", tgt);

            //// GET yerine POST isteği gönderiyoruz
            //var epiasResponse = await _httpClient.PostAsync(
            //    "https://seffaflik.epias.com.tr/electricity-service/v1/markets/dam/data/mcp",
            //    jsonContent);

            var epiasResponse = await _httpClient.PostAsync(
                "https://seffaflik.epias.com.tr/electricity-service/v1/markets/idm/data/transaction-history?endDate=2022-09-11&startDate=2024-09-10",
                jsonContent);

            //var epiasResponse = await _httpClient.PostAsync(
            //    $"https://seffaflik.epias.com.tr/electricity-service/v1/markets/idm/data/transaction-history?endDate={endDate}&startDate={startDate}",
            //    jsonContent);

            

            // İstek başarılı ise gelen veriyi deserialize ediyoruz
            if (epiasResponse.IsSuccessStatusCode)
            {
                string responseData = await epiasResponse.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<TransactionHistoryGipDataDto>>(JObject.Parse(responseData)["items"].ToString());

                return data;
            }

            // Başarısız ise null dönüyoruz
            return null;
        }
    }

}
