using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.EventAggregators
{
    public class UserLoginMessage<T> : PubSubEvent<T>
    {
    }
}
