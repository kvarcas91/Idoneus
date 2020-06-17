using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Common.EventAggregators
{
    public class SendMessageEvent<T> : PubSubEvent<T>
    {
    }
}
