using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ETSimpleKit.ItemSystem
{
    public abstract class ItemBase : MonoBehaviour
    {
        public abstract void Init(ItemDat itemData, UnityAction<int> ifIsLockTrueAction = null, UnityAction<int> ifIsLockFalseAction = null);
        public abstract void UpdateLock();
        public abstract void UpdateSelect();
    }
}


