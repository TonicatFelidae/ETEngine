using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Modal;
using UnityScreenNavigator.Runtime.Core.Page;
using VContainer;
using VContainer.Unity;
namespace ETEngine
{
    public class UIManagerInstaller
    {
        public static void Install(IContainerBuilder builder)
        {
            builder.Register<PageFactory>(Lifetime.Scoped);
            builder.Register<PopupFactory>(Lifetime.Scoped);
            builder.Register<IUIManager, UIManager>(Lifetime.Scoped);
            builder.RegisterEntryPoint<UIBootstrap>();
            Debug.Log("UIManagerInstaller Install completed.");
        }
    }
}
