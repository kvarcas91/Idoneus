﻿using Prism.Events;

namespace Common.EventAggregators
{
    public class SendMessageEvent<T> : PubSubEvent<T>
    {
    }
}
