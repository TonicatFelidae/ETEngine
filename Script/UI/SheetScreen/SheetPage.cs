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

        /// <summary>
        ///     Addressables keys of the sheets this page owns, in bottom-bar order.
        ///     Deliberately plain strings and not <see cref="Sheet" /> references: the old
        ///     <c>Sheet[]</c> was only ever read for its name, yet a direct object reference
        ///     made every sheet prefab a hard build dependency of this page. That single field
        ///     pulled all five tabs - roughly 40 MB of art - into MainScreen's bundle and made
        ///     them impossible to split or unload.
        /// </summary>
        [SerializeField] private string[] sheetScreenIds;

        public BottomNavigationBar bottomNavigationBar;
        public bool registerSheetsOnStart = true;
        public int defaultEnableSheetIndex = 0;

        /// <summary>
        ///     Load a sheet on the first touch of its tab and drop it again when another tab is
        ///     shown. Costs one bundle load when switching back; saves keeping every visited
        ///     tab resident for the whole session.
        /// </summary>
        [SerializeField] private bool unloadInactiveSheets = true;

        /// <summary>Sheet ids this page owns, in bottom-bar order.</summary>
        public IReadOnlyList<string> SheetScreenIds => sheetScreenIds;

        public async void Start()
        {
            if (registerSheetsOnStart)
            {
                _uIManager.RegisterSheetContainer(sheetContainer, Identifier);
                sheetContainer.UnloadInactiveSheets = unloadInactiveSheets;

                if (unloadInactiveSheets)
                {
                    // Register only the tab we are about to show. The rest are registered on
                    // demand by BottomNavigationBar.TouchNavButton, so their bundles are never
                    // touched unless the player actually opens them.
                    string defaultSheetId = DefaultSheetId();
                    if (!string.IsNullOrEmpty(defaultSheetId))
                    {
                        await sheetContainer.Register(defaultSheetId, null, true, defaultSheetId);
                    }
                }
                else
                {
                    foreach (string screenId in sheetScreenIds)
                    {
                        if (string.IsNullOrEmpty(screenId))
                        {
                            continue;
                        }

                        await sheetContainer.Register(screenId, null, true, screenId);
                    }
                }

                EnableDefaultSheet();
            }

            await OnStart();
        }

        /// <summary>The sheet shown when the page opens, or null when none is configured.</summary>
        private string DefaultSheetId()
        {
            if (sheetScreenIds == null || sheetScreenIds.Length == 0)
            {
                return null;
            }

            int index = Mathf.Clamp(defaultEnableSheetIndex, 0, sheetScreenIds.Length - 1);
            return sheetScreenIds[index];
        }

        private void EnableDefaultSheet()
        {
            string defaultSheetId = DefaultSheetId();
            if (!string.IsNullOrEmpty(defaultSheetId))
            {
                bottomNavigationBar.TouchNavButton(defaultSheetId);
            }
        }
        public virtual async UniTask OnStart()
        {

        }
    }
}
