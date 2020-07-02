using Prism.Events;

namespace Common.EventAggregators
{
    public class NotifyProjectChanged<T> : PubSubEvent<T>
    {
    }
}
