using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace UnityScreenNavigator.Runtime.Core.Page
{
    // WARNING USE PageFactory and PopupFactory for UnityScreenNavigator
    public class PageFactory 
    {
        private IObjectResolver _resolver;
        [Inject]
        public void Construct(IObjectResolver resolver)
        {
            _resolver = resolver;
        }
        public T Create<T>(GameObject prefab) where T : Component
        {
            var instance = _resolver.Instantiate(prefab);
            _resolver.InjectGameObject(instance); // auto inject dependencies
            return instance.GetComponent<T>();
        }
    }
    public class PopupFactory 
    {
        private IObjectResolver _resolver;
        [Inject]
        public void Construct(IObjectResolver resolver)
        {
            _resolver = resolver;
        }
        public T Create<T>(GameObject prefab) where T : Component
        {
            var instance = _resolver.Instantiate(prefab);
            _resolver.InjectGameObject(instance); // auto inject dependencies
            return instance.GetComponent<T>();
        }
    }
    //
}