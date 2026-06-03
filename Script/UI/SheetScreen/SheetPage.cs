using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Page;
using UnityScreenNavigator.Runtime.Core.Sheet;
using VContainer;
using Cysharp.Threading.Tasks;
using ETEngine;
using VContainer.Unity;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using ETEngine.SignalSystem;
using UnityEngine.Events;

namespace Game.UI
{
    public class SheetPage : Page
    {
        [Inject] IUIManager _uIManager;
        [Inject] ISignalBus _signalBus;
        [SerializeField] private SheetContainer sheetContainer;
        [SerializeField] private Page[] sheetScreens;
        public BottomNavigationBar _bottomNavigationBar;
        public bool registerSheetsOnStart = true;

        public async void Start()
        {
            if (registerSheetsOnStart)
            {
                _uIManager.RegisterSheetContainer(sheetContainer, Identifier);
                for (int i = 0; i < sheetScreens.Length; i++)
                {
                    var screen = sheetScreens[i];
                    string screenID = screen.Identifier;
                    await sheetContainer.Register(screenID, null, true, screenID);
                }
            }
            OnStart();

        }
        public virtual async void OnStart()
        {

        }
    }
}
