using VContainer;

namespace ETEngine.SignalSystem
{
    public static class SignalBusInstaller
    {
        public static void Install(IContainerBuilder builder)
        {
            builder.Register<ISignalBus, SignalBus>(Lifetime.Singleton);
        }
    }
}
