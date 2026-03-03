using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
namespace ETEngine
{
    public interface ISceneEntryPoint
    {
        UniTask Init(IProgress<float> progress);
    }
}