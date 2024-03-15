using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SpaceTradersApp.Data
{
    public class AgentService
    {
        private readonly HttpClient _httpClient;

        public AgentService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<Agent> GetAgentAsync()
        {
            _httpClient.BaseAddress = new Uri("https://api.spacetraders.io/");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZGVudGlmaWVyIjoiS1VSS0lOIiwidmVyc2lvbiI6InYyLjEuNSIsInJlc2V0X2RhdGUiOiIyMDI0LTAyLTI1IiwiaWF0IjoxNzA5OTAzNjMwLCJzdWIiOiJhZ2VudC10b2tlbiJ9.kNYIbrtfRya5rzZKV80lGAwLZoLrRGRzSLobTdh2_xcA_Uwxunm8rBcGM3a66gIzhII284Z-bkcUejR_FRkApiZ31ojhuxdEIw5xYDE_j1VHLRasx26_pzufZDCLzlHeRReQAjdgCIGN3se9AEZ3fGqBaYoCFt-X9-EwIJyewey0Q9_SkQSFmW4DPRwvoTgHDH5EBfxHV3aWzsgx9WF6bdaccj5Pk5t8MI_MMw4gwkS8A605P8LcSDBsQq873bvuPaUgZu5jY1S_mZH7z1Yqeutm6r3yfPeN33pXZdNjdmnpRr5-1CfpycLZvGLGdN7CRABZQFlX2q6YlG9S5Rd35A");

            try
            {
                var response = await _httpClient.GetAsync("v2/my/agent");
                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();

                var parsedJson = JObject.Parse(jsonString);

                var data = parsedJson["data"].ToString();

                var agent = JsonConvert.DeserializeObject<Agent>(data);
                return agent;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request exception: {e.Message}");
                return null;
            }
        }
    }
}
