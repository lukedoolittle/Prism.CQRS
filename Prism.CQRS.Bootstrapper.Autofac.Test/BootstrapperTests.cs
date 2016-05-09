using Autofac;
using SimpleCQRS.Framework;
using Xunit;

namespace Prism.CQRS.Bootstrapper.Autofac.Test
{
    public class MockBootstrapper : Bootstrapper
    {
        public IContainer Run()
        {
            return base.Run();
        }

        protected override AssemblyCatalog ProvideCatalog()
        {
            return new AssemblyCatalog
            {
                this.GetType().Assembly
            };
        }
    }

    public class BootstrapperTests
    {
        [Fact]
        public async void RegisterAllOpenTypesInAssembly()
        {
            var resolver = new MockBootstrapper().Run();
            var messageHub = new DynamicGenerationMessageHub(resolver);
            var payloadOne = new SomeEventPayload<RequestOne>();
            var payloadTwo = new AThirdEventPayload();

            await messageHub.Publish<SomeEvent<RequestOne>, SomeEventPayload<RequestOne>>(payloadOne);
            await messageHub.Publish<AThirdEvent, AThirdEventPayload>(payloadTwo);

            Assert.Equal(1, SomeEventHandler<RequestOne>.HandledEvents.Count);
            Assert.Equal(1, AnotherEventHandler<RequestOne>.HandledEvents.Count);
            Assert.Equal(1, AThirdEventHandler.HandledEvents.Count);

            await messageHub.Publish<SomeEvent<RequestOne>, SomeEventPayload<RequestOne>>(payloadOne);
            await messageHub.Publish<AThirdEvent, AThirdEventPayload>(payloadTwo);

            Assert.Equal(2, SomeEventHandler<RequestOne>.HandledEvents.Count);
            Assert.Equal(2, AnotherEventHandler<RequestOne>.HandledEvents.Count);
            Assert.Equal(2, AThirdEventHandler.HandledEvents.Count);
        }
    }
}
