using Prism.Events;

namespace Common.EventAggregators
{
    public class SendMessageToDailyTasks<T> : PubSubEvent<T>
    {
    }
}
