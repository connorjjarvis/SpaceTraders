using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpaceTraders.Models;

namespace SpaceTradersApp.Data
{
    public class LocationService
    {
        private readonly RateLimitedQueueManager _queueManager;
        private readonly SpaceTradersClient _spaceTradersClient;
        public LocationService(RateLimitedQueueManager queueManager, SpaceTradersClient spaceTradersClient)
        {
            _queueManager = queueManager;
            _spaceTradersClient = spaceTradersClient;
        }
        public async Task<Location> GetLocationAsync(string sector, string waypoint)
        {
            return await _spaceTradersClient.GetLocationAsync(sector, waypoint);
        }
        public async Task<LocalSystem> GetSystemAsync(string sector)
        {
            return await _spaceTradersClient.GetSystemAsync(sector);
        }
    }
}
