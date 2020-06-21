using Prism.Events;

namespace Common.EventAggregators
{
    public class SendSnackBarMessage : PubSubEvent<string>
    {
    }
}
