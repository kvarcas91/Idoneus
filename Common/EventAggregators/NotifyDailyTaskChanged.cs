using Prism.Events;

namespace Common.EventAggregators
{
    public class NotifyDailyTaskChanged<T> : PubSubEvent<T>
    {
    }

    public class NotifyDailyTaskChanged : PubSubEvent
    {
    }
}
