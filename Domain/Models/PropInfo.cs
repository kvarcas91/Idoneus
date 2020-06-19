using System;

namespace Domain.Models
{
    public struct PropInfo
    {
        public Type Type { get; set; }
        public string Name { get; set; }

#nullable enable
        public dynamic? Value { get; set; }
    }
}
