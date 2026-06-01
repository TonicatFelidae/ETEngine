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
            Install(builder, (IObjectResolver)null, projectInstallerSetup, false);
        }

        public static void Install(IContainerBuilder builder, LifetimeScope currentScope, ProjectInstallerSetup projectInstallerSetup)
        {
            var objectResolver = currentScope?.Container != null ? currentScope?.Container : currentScope?.Parent?.Container;
            Install(builder, objectResolver, projectInstallerSetup, currentScope.IsRoot);
        }

        public static void Install(IContainerBuilder builder, IObjectResolver objectResolver, ProjectInstallerSetup projectInstallerSetup, bool isRoot)
        {
            builder.Register<PageFactory>(Lifetime.Scoped);
            builder.Register<PopupFactory>(Lifetime.Scoped);
            if (projectInstallerSetup.useMainCanvasForAll)
            {
                if (!mainCasvasForAllInstalled)
                {
                    builder.RegisterComponentInNewPrefab(projectInstallerSetup.mainCanvas, Lifetime.Singleton).DontDestroyOnLoad();
                    mainCasvasForAllInstalled = true;
                    Debug.Log("UIManagerInstaller: Registered maincanvas for all");
                }
            }
            else
            {
                if ((objectResolver != null && objectResolver.TryResolve<MainCanvas>(out _))
                || isRoot)
                {

                }
                else
                {

                    builder.RegisterComponentInNewPrefab(projectInstallerSetup.mainCanvas, Lifetime.Scoped);
                    Debug.Log("UIManagerInstaller: Registered a local maincanvas");
                }
            }
            builder.Register<IUIManager, UIManager>(Lifetime.Scoped);
            builder.RegisterBuildCallback(container =>
            {
                var serviceA = container.Resolve<PageFactory>();
                var serviceB = container.Resolve<PopupFactory>();
                var mainCanvas = container.Resolve<MainCanvas>();
                mainCanvas.ReInitiation(serviceA, serviceB);
                // ...
            });
        }
    }
}
