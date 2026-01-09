using System.Collections;
using System.Collections.Generic;
using ET.ResourceManagement;
using UnityEngine;
using VContainer;

namespace ET.Installer
{
    public class ETResourceLoaderInstaller : MonoBehaviour
    {
        public static class ResourceLoaderInstaller
        {
            public static void Install(IContainerBuilder builder)
            {
                builder.Register<IAssetLoader, AddressableAssetLoader>(Lifetime.Singleton)
                    .Keyed(ResourceLoaderType.AddressableAsset);

                builder.Register<IAssetLoader, ResourcesAssetLoader>(Lifetime.Singleton)
                    .Keyed(ResourceLoaderType.ResourceAsset);
            }
        }

        public enum ResourceLoaderType
        {
            AddressableAsset,
            ResourceAsset,
        }
    } 
}