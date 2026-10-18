using VContainer;

namespace ET.Monetization
{
    /// <summary>
    /// Type-safe generic IAP service binding a specific IIAPProvider type.
    /// Useful for VContainer registration with concrete provider types, e.g.:
    /// <code>
    /// builder.Register&lt;UnityIAPProvider&gt;(Lifetime.Singleton).As&lt;IIAPProvider&gt;();
    /// builder.Register&lt;GenericIAPService&lt;UnityIAPProvider&gt;&gt;(Lifetime.Singleton).As&lt;IIAPService&gt;();
    /// </code>
    /// </summary>
    /// <typeparam name="TProvider">The concrete IIAPProvider implementation.</typeparam>
    public class GenericIAPService<TProvider> : IAPService where TProvider : class, IIAPProvider
    {
        public TProvider TypedProvider => _provider as TProvider;

        [Inject]
        public GenericIAPService(TProvider provider) : base(provider)
        {
        }

        public GenericIAPService() : base()
        {
        }
    }
}
