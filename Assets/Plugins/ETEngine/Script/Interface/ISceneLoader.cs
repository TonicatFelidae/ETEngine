using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace ETEngine
{
    public interface ISceneLoader
    {
        Progress<float> Progress { get; set; }
        void StartInitialization();
        UniTask InitializeAll(List<Func<IProgress<float>, UniTask>> initMethods, IProgress<float> progress);
        UniTask InititilizeSceneEntry(IProgress<float> progress);
    }
}