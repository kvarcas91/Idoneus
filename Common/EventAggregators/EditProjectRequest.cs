using Prism.Events;

namespace Common.EventAggregators
{
    public class EditProjectRequest<T> : PubSubEvent<T>
    {
    }
}
