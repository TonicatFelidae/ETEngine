using VContainer;

namespace ET.Monetization
{
    /// <summary>
    /// Type-safe generic monetization service binding a specific IAdProvider type.
    /// Useful for VContainer registration with concrete provider types, e.g.:
    /// <code>
    /// builder.Register&lt;ApplovinAdProvider&gt;(Lifetime.Singleton).As&lt;IAdProvider&gt;();
    /// builder.Register&lt;GenericMonetizationService&lt;ApplovinAdProvider&gt;&gt;(Lifetime.Singleton).As&lt;IMonetizationService&gt;();
    /// </code>
    /// </summary>
    /// <typeparam name="TProvider">The concrete IAdProvider implementation.</typeparam>
    public class GenericMonetizationService<TProvider> : MonetizationService where TProvider : class, IAdProvider
    {
        public TProvider TypedProvider => _provider as TProvider;

        [Inject]
        public GenericMonetizationService(TProvider provider) : base(provider)
        {
        }

        public GenericMonetizationService() : base()
        {
        }
    }
}
