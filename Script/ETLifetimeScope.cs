using System;
using ET.Installer;
using ETEngine.SignalSystem;
using UnityEngine;
using VContainer;
using VContainer.Unity;
namespace ETEngine
{
    /// <summary>
    /// Versatile LifetimeScope that can be used for various purposes (e.g. project-wide installer, scene-specific installer, etc.) 
    /// by inheriting and overriding the 
    /// Register() and 
    /// RegisterBuildCallback() methods.
    /// </summary>
    public class ETLifetimeScope : LifetimeScope
    {
        [SerializeField] private ProjectInstallerSetup _projectInstallerSetup;
        [Tooltip("Objects in this array will be instantiated on Configure(), prefer static objects.")]
        [SerializeField] private GameObject[] instalizeOnLoadObjects;
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"[ETLifetimeScope] [{gameObject.name}] Configure started");
            if (_projectInstallerSetup.useMainCanvasForAll) builder.RegisterComponentInNewPrefab(_projectInstallerSetup.mainCanvas, Lifetime.Singleton).DontDestroyOnLoad();
            Debug.Log($"[ETLifetimeScope] [{gameObject.name}] Is using main canvas for all: {_projectInstallerSetup.useMainCanvasForAll}");
            Register(builder);
            Debug.Log($"[ETLifetimeScope] [{gameObject.name}] Registered");
            builder.RegisterBuildCallback(RegisterBuildCallback);
            Debug.Log($"[ETLifetimeScope] [{gameObject.name}] Registered Build Callback");
            foreach (var obj in instalizeOnLoadObjects)
            {
                GameObject.Instantiate(obj);
            }
            Debug.Log($"[ETLifetimeScope] [{gameObject.name}] Instalized On Load Objects count: {instalizeOnLoadObjects.Length} ");
            Debug.Log($"[ETLifetimeScope] [{gameObject.name}] Configure finished");
        }
        protected virtual void Register(IContainerBuilder builder)
        {

        }
        protected virtual void RegisterBuildCallback(IObjectResolver resolver)
        {
            //resolver to get the registered objects and do some initialization that can't be done in Register() (e.g. because of RegisterInstance objects that don't support [Inject])
        }


    }
}