using Prism.Events;

namespace Common.EventAggregators
{
    class SendProjectToProjectView<T> : PubSubEvent<T>
    {
    }
}
