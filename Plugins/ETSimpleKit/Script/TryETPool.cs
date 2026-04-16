using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ET
{
    /// <summary>
    /// Simple pool system that handle turn on and off MonoBehaviour pool item gameobject
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TryETPool<T> where T : MonoBehaviour
    {
        List<T> _listObject;
        Transform _container;
        T _pp_poolObject;
        public TryETPool(Transform container, T pp_poolObject)
        {
            _listObject = new();
            _container = container;
            _pp_poolObject = pp_poolObject;
        }
        public T GetObject()
        {
            for (int i = 0; i < _listObject.Count; i++)
            {
                if (_listObject[i].gameObject.activeSelf == false)
                {
                    _listObject[i].gameObject.SetActive(true);
                    return _listObject[i];
                }
            }
            T newGo = GameObject.Instantiate<T>(_pp_poolObject, _container);
            newGo.gameObject.SetActive(true);
            _listObject.Add(newGo);
            return newGo;
        }
        public void CleanPool()
        {
            for (int i = 0; i < _listObject.Count; i++)
            {
                _listObject[i].gameObject.SetActive(false);
            }
        }
        public void ReturnToPool(T go)
        {
            go.gameObject.SetActive(false);
        }
    }
}
