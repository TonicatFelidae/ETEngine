using ET;
using ETSimpleKit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETSimpleKit
{
    public class ETNavigation : Singleton<ETNavigation>
    {

        [Header("Manual binding")]
        public ButtonNavigationData[] _buttonNavigations;
        public ButtonNavigationData[] _levelNavigations;
        private void Start()
        {
            foreach (var buttonx in _buttonNavigations) { BindButtonNavigation(buttonx); }
            foreach (var buttonx in _levelNavigations) { BindButtonNavigation(buttonx); }
        }
        public void BindButtonNavigation(ButtonNavigationData buttonData)
        {
            buttonData.button.onClick.RemoveAllListeners(); 
            buttonData.button.onClick.AddListener(()=>
            {
                switch (buttonData.uIButtonAction)
                {
                    case UIButtonAction.None:
                        UIManager.Instance.LoadUI_FromButtonNavigation(buttonData.uIType, buttonData.index, buttonData.dataInt, buttonData.dataString);
                        break;
                    case UIButtonAction.BackToPreviousUI:
                        UIManager.Instance.LoadPreviousUI_FromButtonNavigation();
                        break;
                    default:
                        break;
                }
            });

        }
    }
    public enum UIType
    {
        Home,

        Navigation=10,
        Navigation1,
        Navigation2,
        Navigation3,
        Navigation4,
        Navigation5,
        Navigation6,
        Navigation7,
        Navigation8,
        NavigationEnd = 49,

        Result,
        HowToPlay,
        Credit,
        Setting,
        Shop,

        Level = 100,
        Levelx0, // from levelx to end should not use for simple platform, we only focus on loopable class
        Levelx1,
        Levelx2,
        Levelx3,
        Levelx4,
        Levelx5,
        Levelx6,
        Levelx7,
        Levelx8,
        Levelx9,

        [InspectorName(null)]
        Null = 9999
    }
    public enum UIButtonAction
    {
        None,
        BackToPreviousUI,
    }

}
