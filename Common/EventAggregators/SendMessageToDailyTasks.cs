using Prism.Events;

namespace Common.EventAggregators
{
    public class SendMessageToDailyTasks : PubSubEvent<(double, int)>
    {
    }
}
