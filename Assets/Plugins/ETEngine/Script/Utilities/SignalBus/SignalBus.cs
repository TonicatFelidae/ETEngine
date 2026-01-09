using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace ETEngine.SignalSystem
{
    public class SignalBus : ISignalBus
    {
        private readonly Dictionary<Type, List<Delegate>> _listeners
            = new Dictionary<Type, List<Delegate>>();

        public void Fire<T>(T signal)
        {
            var type = typeof(T);
            if (!_listeners.TryGetValue(type, out var list))
                return;

            // snapshot to avoid modification during iteration
            var copy = list.ToArray();
            foreach (Delegate d in copy)
            {
                ((UnityAction<T>)d)?.Invoke(signal);
            }
        }

        public void Subscribe<T>(UnityAction<T> callback)
        {
            var type = typeof(T);
            if (!_listeners.TryGetValue(type, out var list))
            {
                list = new List<Delegate>();
                _listeners[type] = list;
            }

            if (!list.Contains(callback))
                list.Add(callback);
        }

        public void UnSubscribe<T>(UnityAction<T> callback)
        {
            var type = typeof(T);
            if (!_listeners.TryGetValue(type, out var list))
                return;

            list.Remove(callback);

            if (list.Count == 0)
                _listeners.Remove(type);
        }
    }
}