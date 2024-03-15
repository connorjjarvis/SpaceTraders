using SpaceTraders.Models;

public class AgentService
{
    private readonly RateLimitedQueueManager _queueManager;
    private readonly SpaceTradersClient _spaceTradersClient;

    public AgentService(RateLimitedQueueManager queueManager, SpaceTradersClient spaceTradersClient)
    {
        _queueManager = queueManager;
        _spaceTradersClient = spaceTradersClient;

        InitializeRecurringTasks();
    }

    private void InitializeRecurringTasks()
    {
        _queueManager.ScheduleRecurringTask(() => _spaceTradersClient.GetAgentAsync(), TimeSpan.FromSeconds(30));
    }
    public async Task<Agent> GetAgentAsync()
    {
        return await _spaceTradersClient.GetAgentAsync();
    }
}
