﻿using Prism.Events;

namespace Common.EventAggregators
{
    public class SendMessageToUpcommingTasks<T> : PubSubEvent<T>
    {
    }
}
