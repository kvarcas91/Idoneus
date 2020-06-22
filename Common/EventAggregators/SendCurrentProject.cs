using Prism.Events;

namespace Common.EventAggregators
{
    public class SendCurrentProject<T> : PubSubEvent<T>
    {
    }
}
