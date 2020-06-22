using Prism.Events;

namespace Common.EventAggregators
{
    class SendCurrentProject<T> : PubSubEvent<T>
    {
    }
}
