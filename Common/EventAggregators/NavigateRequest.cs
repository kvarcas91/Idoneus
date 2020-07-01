using Prism.Events;

namespace Common.EventAggregators
{
    public class NavigateRequest<T> : PubSubEvent<(string, T, bool)>
    {
    }
}
