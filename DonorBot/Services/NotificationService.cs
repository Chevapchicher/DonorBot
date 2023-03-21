using DonorBot.Configs;
using Microsoft.Extensions.Options;

namespace DonorBot.Services;

public class NotificationService
{
    private readonly Dictionary<long, bool> _notifications = new();

    public NotificationService(IOptions<AppConfig> config)
    {
        foreach (var subscriber in config.Value.Subscribers)
        {
            _notifications.Add(subscriber, true);
        }
    }

    public IEnumerable<long> GetIdsForNotifications()
    {
        return _notifications
            .Where(x => x.Value)
            .Select(x => x.Key);
    }

    public void SetNotification(long id, bool isEnabled)
    {
        _notifications[id] = isEnabled;
    }
}