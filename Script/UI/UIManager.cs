using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityScreenNavigator.Runtime.Core.Modal;
using UnityScreenNavigator.Runtime.Core.Page;
using UnityScreenNavigator.Runtime.Core.Sheet;
using VContainer;

namespace ETEngine
{
    public class UIManager : IUIManager
    {
        [Inject] MainCanvas _mainCanvas;
        public PageContainer PageContainer => _mainCanvas.pageContainer;
        public PopupContainer PopupContainer => _mainCanvas.popupContainer;
        public Dictionary<string, SheetContainer> SheetContainers { get => _sheetContainers; set => _sheetContainers = value; }

        private System.Collections.Generic.Dictionary<string, SheetContainer> _sheetContainers = new();
        public void Init(PageContainer pageContainer, PopupContainer popupContainer)
        {
            //PageContainer = pageContainer;
            // PopupContainer = popupContainer;
        }

        public async UniTask<T> PushPage<T>(string pageId = null, bool playAnimation = true,
            Action<(string pageId, T page)> loadCallback = null
        )
            where T : Page
        {
            if (string.IsNullOrEmpty(pageId)) pageId = typeof(T).Name;
            var handle = PageContainer.Push<T>(pageId, playAnimation: playAnimation, onLoad: loadCallback);
            var result = await handle.Task;

            // The result should be the Page object returned from the coroutine
            if (result is Page page)
            {
                return page as T;
            }

            // Fallback: try to get the page from the container if the result was null
            // This provides backward compatibility and handles edge cases
            Debug.LogWarning($"PushPage<{typeof(T).Name}> returned null result, attempting to retrieve from container");
            return PageContainer.Get<T>();
        }

        public async UniTask PopPage(bool playAnimation = true)
        {
            await PageContainer.Pop(playAnimation);
        }


        public async UniTask<T> PushPopup<T>(string modelId = null, bool playAnimation = true,
            Action<(string modalId, T modal)> loadCallback = null
        ) where T : Popup
        {

            if (string.IsNullOrEmpty(modelId)) modelId = typeof(T).Name;
            var handle = PopupContainer.Push<T>(modelId, playAnimation, onLoad: loadCallback);
            var result = await handle.Task;

            // The result should be the Popup object returned from the coroutine
            if (result is Popup popup)
            {
                return popup as T;
            }
            // Fallback: try to get the popup from the container if the result was null
            // This provides backward compatibility and handles edge cases
            Debug.LogWarning($"PushPopup<{typeof(T).Name}> returned null result, attempting to retrieve from container");
            return PopupContainer.Get<T>();
        }

        public async UniTask PopPopup(bool playAnimation = true)
        {
            await PopupContainer.Pop(playAnimation);
        }

        public T GetPopup<T>() where T : Popup
        {
            return PopupContainer.Get<T>();
        }

        public T GetPage<T>() where T : Page
        {
            return PageContainer.Get<T>();
        }


        public async UniTask PopPageAsync()
        {
            if (PageContainer.IsInTransition)
            {
                Debug.LogWarning("Transition is running, skipping Pop!");
                return;
            }

            var handle = PageContainer.Pop(false);
            await handle.Task; // Wait for Pop animation to complete

            Debug.Log("Pop page completed!");
        }

        /// <summary>
        /// Pops the current modal from the stack.
        /// </summary>
        /// <param name="playAnimation">Whether to play the transition animation.</param>
        public void PopModal(int numberModal = 1, bool playAnimation = false)
        {
            if (PopupContainer == null) return;
            if (numberModal <= 1)
            {
                PopupContainer.Pop(playAnimation);
                return;
            }

            PopupContainer.Pop(playAnimation, numberModal);
        }



        /// <summary>
        /// Shows a new modal.
        /// </summary>
        /// <param name="modalName">The resource name of the modal prefab.</param>
        /// <param name="playAnimation">Whether to play the transition animation.</param>
        public void PushModal(string modalName, bool playAnimation = false)
        {
            if (PopupContainer == null) return;
            PopupContainer.Push(modalName, playAnimation);
        }

        public void PushModal<T>(Action<(string modalId, T modal)> loadCallback = null)
            where T : Popup
        {
            PopupContainer.Push(typeof(T).Name, true, onLoad: loadCallback);
        }

        public void PushModal<T>(string modalName, UnityAction onConfirm, UnityAction onCancle,
            bool playAnimation = true) where T : Popup =>
            PushModal<T>(modalName, onConfirm, onCancle, null, null, null, playAnimation);

        /// <summary>
        /// Shows a specific confirmation modal with custom data and callbacks.
        /// </summary>
        /// <param name="data">The ConfirmPopupData containing message, callbacks, and button texts.</param>
        /// <param name="playAnimation">Whether to play the transition animation.</param>
        public void PushModal<T>(string modalName, UnityAction onConfirm, UnityAction onCancle, UnityAction action0,
            UnityAction action1 = null, UnityAction action2 = null, bool playAnimation = true) where T : Popup
        {
            if (PopupContainer == null) return;
            PopupContainer.Push("ConfirmModal", false, onLoad: x =>
            {
                if (x.modal is IActionPopup confirmPopupController)
                {
                    confirmPopupController.Init(onConfirm, onCancle, action0, action1, action2);
                }
            });
        }

        public void PopModal()
        {
            PopupContainer.Pop(true);
        }

        // Add a private dictionary to store registered sheets

        // Registers a Sheet instance by type and optional ID
        public void RegisterSheetContainer<T>(T sheetContainer, string sheetContainerId = null) where T : SheetContainer
        {
            if (sheetContainerId == null)
                sheetContainerId = typeof(T).Name;
            if (sheetContainer == null)
                throw new InvalidOperationException($"Sheet of type {typeof(T).Name} not found in the scene.");

            SheetContainers[sheetContainerId] = sheetContainer;
        }

        // Retrieves a registered Sheet by type (using type name as key)
        public SheetContainer GetSheetContainer<T>(string sheetContainerId = null)
        {
            if (sheetContainerId == null)
                sheetContainerId = typeof(T).Name;
            if (SheetContainers.TryGetValue(sheetContainerId, out var sheetContainer))
                return sheetContainer;

            throw new InvalidOperationException($"Sheet of type {typeof(T).Name} is not registered.");
        }

        public void ShowLoadingPage(string messege = null) => _mainCanvas.loadingPage.Show(messege);

        public void HideLoadingPage() => _mainCanvas.loadingPage.Hide();

        /*
            /// <summary>
            /// Shows a specific notification modal with custom data and callbacks.
            /// </summary>
            /// <param name="data">The NotificationPopupData containing message, callbacks, and button text.</param>
            /// <param name="playAnimation">Whether to play the transition animation.</param>
            public void ShowNotificationModal(NotificationPopupData data, string modalName = "NotiModal", bool playAnimation = true)
            {
                if (_modalContainer == null) return;
                _modalContainer.Push(modalName, false, onLoad: x =>
                {
                    if (x.modal is NotificationPopup notificationPopupController)
                    {
                        notificationPopupController.Setup(data);
                    }
                });
            }

            public void ShowResultModal(ResultPopupData data, bool playAnimation = true)
            {
                if (_modalContainer == null) return;
                _modalContainer.Push("ResultModal", false, onLoad: x =>
                {
                    if (x.modal is ResultPopup resultPopup)
                    {
                        resultPopup.Setup(data);
                    }
                });
            }

            public void ShowNextLevelModal(NextLevelPopupData data, bool playAnimation = true)
            {
                if (_modalContainer == null) return;
                _modalContainer.Push("NextLevel", false, onLoad: x =>
                {
                    if (x.modal is NextLevelPopup nextLevelPopup)
                    {
                        nextLevelPopup.Setup(data);
                    }
                });
            }

            public void ShowEndLevelModal(NextLevelPopupData data , string modalName = "", bool playAnimation = true)
            {
                if (_modalContainer == null) return;
                _modalContainer.Push(modalName, false, onLoad: x =>
                {
                    if (x.modal is NextLevelPopup nextLevelPopup)
                    {
                        nextLevelPopup.Setup(data);
                    }
                });
            }

            /// <summary>
            /// Shows a new sheet.
            /// </summary>
            /// <param name="sheetName">The resource name of the sheet prefab.</param>
            /// <param name="playAnimation">Whether to play the transition animation.</param>
            public void ShowSheet(string sheetName, bool playAnimation = true)
            {
                //  if (_sheetContainer == null) return;
                //  _sheetContainer.Show(sheetName, playAnimation);
            }

            public void ShowHUD(bool isShow)
            {
                HudGO.gameObject.SetActive(isShow);
            }
            */
    }

    public enum PushType
    {
        Single,
        Multi,
    }
}
/*Rule The Sea Company.*/