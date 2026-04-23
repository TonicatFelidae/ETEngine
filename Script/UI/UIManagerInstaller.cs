using System;
using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Modal;
using UnityScreenNavigator.Runtime.Core.Page;
using VContainer;
using VContainer.Unity;
namespace ETEngine
{
    public class UIManagerInstaller
    {
        public static bool mainCasvasForAllInstalled = false;
        public static void Install(IContainerBuilder builder, ProjectInstallerSetup projectInstallerSetup)
        {
            if (projectInstallerSetup.useMainCanvasForAll)
            {
                if (!mainCasvasForAllInstalled)
                {
                    builder.RegisterComponentInNewPrefab(projectInstallerSetup.mainCanvas, Lifetime.Singleton).DontDestroyOnLoad();
                    mainCasvasForAllInstalled = true;
                }
            }
            else
            {
                builder.RegisterComponentInNewPrefab(projectInstallerSetup.mainCanvas, Lifetime.Scoped);

            }
            builder.Register<PageFactory>(Lifetime.Scoped);
            builder.Register<PopupFactory>(Lifetime.Scoped);
            builder.Register<IUIManager, UIManager>(Lifetime.Scoped);
            builder.RegisterEntryPoint<UIBootstrap>();
        }
    }
}
