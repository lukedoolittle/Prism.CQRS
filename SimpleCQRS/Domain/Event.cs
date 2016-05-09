using System;
using SimpleCQRS.Framework.Contracts;

namespace SimpleCQRS.Domain
{
    public class Event : IMessage, IUnique
    {
        public Guid Id { get; set; }
        public Guid AggregateId { get; set; }
        public int Version { get; set; }
    }
}
