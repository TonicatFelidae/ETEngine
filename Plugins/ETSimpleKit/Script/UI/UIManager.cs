using ET;
using ET.SupportKit;
using ET.UIKit;
using ETSimpleKit;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

namespace ETSimpleKit.UI
{
    /// <summary>
    /// UI principle:
    /// - Load means enable UI
    /// - For multilevel scene use UIIndex.index to create varity 
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        private ETSimpleUI[] UIInstances;
        private List<IUISizeChangeListener> _UISizeChangeListeners;
        private List<IUIEventInvoker> _UIEventInvoker;
        private UIType _currentUI = UIType.Null;
        private UIType _previousUI = UIType.Null; // only can back one time with one level layer of UI.
                                                  // Example Home => popup, can back to Home,
                                                  // Home => popup => popup2, can back to popup, but it wont remember Home
        public bool enablePlaySellectedUIOnStart;
        public UIType sellectedUIOnStart;
        private void Awake()
        {
            UIInstances = GameObject.FindObjectsOfType<ETSimpleUI>(true);
            _UISizeChangeListeners = ET_GO.FindObjectsByComponent<IUISizeChangeListener>(true);
            _UIEventInvoker = ET_GO.FindObjectsByComponent<IUIEventInvoker>(true);
            this.Log($"UIEventInvoker : {_UIEventInvoker.Count} UISizeChangeListeners : {_UISizeChangeListeners.Count}");
            EventInvokerInitiation();
        }
        private void Start()
        {
            if (enablePlaySellectedUIOnStart)
            {
                foreach (var instance in UIInstances)
                {
                    if (instance.type == sellectedUIOnStart)
                    {
                        instance.Active(true);
                        _currentUI = sellectedUIOnStart;
                    }
                    else
                    {
                        instance.Active(false);
                    }
                }
            }
            EventInvoker(UIEvent.SizeChange);
        }
        /// <summary>
        /// Load UI and inject Data
        /// </summary>
        /// <param name="uiType"></param>
        /// <param name="index">is data 0 of num</param>
        /// <param name="data1">is data 1 of num</param>
        /// <param name="stringData0">is data 0 of string</param>
        public void LoadUI_FromButtonNavigation(UIType uiType, int index = 0, int data1 = 0, string stringData0 = "")
        {
            List<float> injectData = new List<float>() { index, data1 };
            List<string> injectDataString = new List<string>() { stringData0 };
            LoadUI(uiType, injectData, injectDataString);
        }
        public void LoadPreviousUI_FromButtonNavigation() => LoadPreviousUI();
        public void LoadUI(UIType type, List<float> numInjectionData, List<string> stringInjectionData = null)
        {
            MemoratePrevious(type);

            foreach (ETSimpleUI uiObj in UIInstances)
            {
                if (uiObj.type == type)
                {
                    uiObj.Active(true, numInjectionData, stringInjectionData);
                }
                else
                {
                    uiObj.Active(false, numInjectionData, stringInjectionData);
                }
            }
        }
        /// <summary>
        /// LoadUI type 3: inject nothing, no change on already had datas
        /// </summary>
        /// <param name="type"></param>
        public void LoadUI(UIType type)
        {
            MemoratePrevious(type);

            foreach (ETSimpleUI uiObj in UIInstances)
            {
                if (uiObj.type == type)
                { 
                    uiObj.Active(true);//Active type 3
                }
                else
                {
                    uiObj.Active(false);//Active type 3
                }
            }
        }
        private void MemoratePrevious(UIType type)
        {
            if (_currentUI != UIType.Null) _previousUI = _currentUI;
            _currentUI = type;
        }
        private void LoadPreviousUI()
        {
            LoadUI(_previousUI);
        }
        #region Event Invoker  
        public void EventInvokerInitiation()
        {
            if (_UIEventInvoker == null || _UIEventInvoker.Count == 0) return;
            foreach (IUIEventInvoker item in _UIEventInvoker)
            {
                if (item.OnEventInvoker == null) item.OnEventInvoker = new();
                item.OnEventInvoker.AddListener(EventInvoker);
            }
        }
        public void EventInvoker(UIEvent uIEvent)
        {
            switch (uIEvent)
            {
                case UIEvent.SizeChange:

                    foreach (IUISizeChangeListener item in _UISizeChangeListeners)
                    {
                        item.OnUISizeChange();
                    }
                    break;
            }
        }
        #endregion
    }
    [CreateAssetMenu]
    public class MyEnumWrapper : ScriptableObject
    {
        //public UIType myEnumValue;
    }
    public interface SimpleUI
    {

    }

}
