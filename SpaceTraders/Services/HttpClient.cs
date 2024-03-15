using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpaceTraders.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public class SpaceTradersClient
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _memoryCache;
    private readonly RateLimitedQueueManager _queueManager; // Add this

    public SpaceTradersClient(HttpClient httpClient, IMemoryCache memoryCache, RateLimitedQueueManager queueManager) // Update this
    {
        _httpClient = httpClient;
        _memoryCache = memoryCache;
        _queueManager = queueManager; // Update this
        _httpClient.BaseAddress = new Uri("https://api.spacetraders.io/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZGVudGlmaWVyIjoiS1VSS0lOIiwidmVyc2lvbiI6InYyLjIuMCIsInJlc2V0X2RhdGUiOiIyMDI0LTAzLTEwIiwiaWF0IjoxNzEwMTY2MzczLCJzdWIiOiJhZ2VudC10b2tlbiJ9.qf7yCWcotkqywuvwWsVAIzIS65_GltqVtIjeDj1lg8k1QU9tatai8GuSTmStlPjkE3BREnByiD7tzq40GWq-P_K5RgpvIkkrFpYlfaa6YeVLpq9iB_7icxg0T7jGX1zm2riRUf0cJ4Et4SWL5ciN35F5vyB5DSo3neYUfIf-GvemxqxUoEPYP_XzpbI7qPKD6b7A_uXngE1ggaxB5TnEsoD32ePT8pUotgmhTLyd9UYMq2-Mi9DAM3R5_bsuhmLYeRbNuNJiwXykivE2ggyYLYW6kigWRdGFq2kvtRPp_NdrRQemRM2Ce4TCo8IMKhSI3a9Jap-5Q_oVbfIJ2_VNhQ");
    }

    public Task<Agent> GetAgentAsync()
    {
        return _queueManager.EnqueueTask(async () => await FetchAgentData(), "Get Agent Data");
    }

    private async Task<Agent> FetchAgentData()
    {
        try
        {
            string cacheKey = "AgentData";
            if (!_memoryCache.TryGetValue(cacheKey, out Agent agent))
            {
                var response = await _httpClient.GetAsync("v2/my/agent");
                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();
                var parsedJson = JObject.Parse(jsonString);
                var data = parsedJson["data"].ToString();
                agent = JsonConvert.DeserializeObject<Agent>(data);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(1));
                _memoryCache.Set(cacheKey, agent, cacheEntryOptions);
            }
            return agent;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request exception: {e.Message}");
            return null;
        }
    }

    public async Task<Location> GetLocationAsync(string sector, string waypoint)
    {
        try
        {
            var response = await _httpClient.GetAsync($"v2/systems/{sector}/waypoints/{waypoint}");
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();

            var parsedJson = JObject.Parse(jsonString);

            var data = parsedJson["data"].ToString();

            return JsonConvert.DeserializeObject<Location>(data);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request exception: {e.Message}");
            return null;
        }
    }
    public async Task<LocalSystem> GetSystemAsync(string sector)
    {
        try
        {
            var response = await _httpClient.GetAsync($"v2/systems/{sector}");
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();

            var parsedJson = JObject.Parse(jsonString);

            var data = parsedJson["data"].ToString();

            return JsonConvert.DeserializeObject<LocalSystem>(data);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request exception: {e.Message}");
            return null;
        }
    }
}