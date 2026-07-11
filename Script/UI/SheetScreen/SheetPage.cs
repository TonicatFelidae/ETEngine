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
        [SerializeField] private Sheet[] sheetScreens;
        public BottomNavigationBar bottomNavigationBar;
        public bool registerSheetsOnStart = true;
        public int defaultEnableSheetIndex = 0;


        public async void Start()
        {
            if (registerSheetsOnStart)
            {
                _uIManager.RegisterSheetContainer(sheetContainer, Identifier);
                for (int i = 0; i < sheetScreens.Length; i++)
                {
                    var screen = sheetScreens[i];
                    string screenID = screen.Identifier;
                    if (string.IsNullOrEmpty(screenID))
                    {
                        screenID = sheetScreens[i].name;
                    }
                    Debug.Log($"[SheetPage] Registering sheet: {screenID}");
                    await sheetContainer.Register(screenID, null, true, screenID);
                }
                await EnableDefaultSheet();

            }
            await OnStart();

        }
        private async UniTask EnableDefaultSheet()
        {
            if (sheetScreens.Length > 0)
            {
                string defaultSheetID = sheetScreens[defaultEnableSheetIndex].Identifier;
                if (string.IsNullOrEmpty(defaultSheetID))
                {
                    defaultSheetID = sheetScreens[defaultEnableSheetIndex].name;
                }
                bottomNavigationBar.TouchNavButton(defaultSheetID);
            }
        }
        public virtual async UniTask OnStart()
        {

        }
    }
}
