using System.Xml.Linq;

public class RateLimitedQueueManager
{
    private readonly Queue<Func<Task>> _tasks = new Queue<Func<Task>>();
    private readonly Dictionary<Func<Task>, string> _taskNames = new Dictionary<Func<Task>, string>();
    private readonly SemaphoreSlim _rateLimiter = new SemaphoreSlim(2, 2);
    private readonly Timer _processingTimer;
    private bool _isProcessing = false;
    private readonly List<(Func<Task> task, DateTime runAt, string name)> _delayedTasks = new List<(Func<Task> task, DateTime runAt, string name)>();

    public void ScheduleDelayedTask(Func<Task> task, TimeSpan delay, string name = "Unnamed Task")
    {
        var runAt = DateTime.UtcNow.Add(delay);
        lock (_delayedTasks)
        {
            _delayedTasks.Add((task, runAt, name));
        }
    }

    private void ProcessDelayedTasks()
    {
        List<(Func<Task> task, string name)> tasksToEnqueue = new List<(Func<Task> task, string name)>();
        lock (_delayedTasks)
        {
            var now = DateTime.UtcNow;
            var dueTasks = _delayedTasks.Where(t => t.runAt <= now).ToList();
            foreach (var (task, _, name) in dueTasks)
            {
                tasksToEnqueue.Add((task, name));
            }
            _delayedTasks.RemoveAll(t => t.runAt <= now);
        }

        foreach (var (task, name) in tasksToEnqueue)
        {
            Enqueue(task, name);
        }
    }

    public RateLimitedQueueManager()
    {
        _processingTimer = new Timer(ProcessQueue, null, Timeout.Infinite, Timeout.Infinite);
    }
    public void StartProcessing()
    {
        _processingTimer.Change(0, 1000);
    }
    public void ScheduleRecurringTask(Func<Task> task, TimeSpan interval, string name = "Unamed Task")
    {
        ScheduleDelayedTask(task, interval, name);
    }
    public void Enqueue(Func<Task> task, string name = "Unnamed Task")
    {
        lock (_tasks)
        {
            _tasks.Enqueue(task);
            _taskNames[task] = name;
        }
        StartProcessing();
    }

    public Task<T> EnqueueTask<T>(Func<Task<T>> taskGenerator, string name = "Unnamed Task")
    {
        var taskCompletionSource = new TaskCompletionSource<T>();
        Func<Task> wrappedTask = async () =>
        {
            try
            {
                var result = await taskGenerator();
                taskCompletionSource.SetResult(result);
            }
            catch (Exception ex)
            {
                taskCompletionSource.SetException(ex);
            }
            finally
            {
                _rateLimiter.Release();
            }
        };

        Enqueue(wrappedTask, name); // Use the new Enqueue signature
        return taskCompletionSource.Task;
    }
    public string GetQueue()
    {
        lock (_tasks)
        {
            return string.Join(", ", _tasks.Select(t => _taskNames.TryGetValue(t, out var name) ? name : "Unnamed Task"));
        }
    }

    public string GetQueueStatus()
    {
        return $"Queue length: {_tasks.Count}, Rate limiter count: {_rateLimiter.CurrentCount}";
    }

    // Ensure to clear the name from _taskNames when a task is dequeued
    private async void ProcessQueue(object state)
    {
        if (_isProcessing)
            return;

        _isProcessing = true;

        try
        {
            ProcessDelayedTasks(); // Check and enqueue any due delayed tasks

            while (_tasks.Any())
            {
                await _rateLimiter.WaitAsync();

                Func<Task> task;
                lock (_tasks)
                {
                    if (!_tasks.Any())
                        return;

                    task = _tasks.Dequeue();
                    // It's crucial to remove the task name right after dequeueing to keep _taskNames clean.
                    _taskNames.Remove(task);
                }

                var ignore = Task.Run(async () =>
                {
                    try
                    {
                        await task();
                    }
                    finally
                    {
                        _rateLimiter.Release();
                    }
                });
            }
        }
        finally
        {
            _isProcessing = false;
        }
    }

}
