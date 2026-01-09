using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ETEngine.SignalSystem
{
    public interface ISignalBus
    {
        void Fire<T>(T signal);
        void Subscribe<T>(UnityAction<T> callback);
        void UnSubscribe<T>(UnityAction<T> callback);
    }
}