using System;
using Prism.Commands;

namespace Prism.CQRS.Test.SimpleCQRS.Eventing
{
    public class MyCommandPayload
    {
        public string MyPayloadData { get; set; }
    }

    public class MyCommand : SimpleCommand<MyCommandPayload>
    {
        public MyCommand(Action<MyCommandPayload> action) : base(action) { }
    }

    public class MySimpleCommand : SimpleCommand
    {
        public MySimpleCommand(Action action) : base(action)
        {
        }
    }
}
