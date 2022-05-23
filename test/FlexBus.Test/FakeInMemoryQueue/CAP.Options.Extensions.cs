namespace FlexBus.Test.FakeInMemoryQueue
{
    public static class CapOptionsExtensions
    {
        public static FlexBusOptions UseFakeTransport(this FlexBusOptions options)
        {
            options.RegisterExtension(new FakeQueueOptionsExtension());
            return options;
        }
    }
}