using System;
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
        [Header("Containers")] private readonly PageContainer _pageContainer;
        public PageContainer PageContainer => _pageContainer;
        private readonly PopupContainer _popupContainer;
        public PopupContainer PopupContainer => _popupContainer;
        private UIManager(
            PageContainer pageContainer,
            PopupContainer popupContainer)
        {
            _pageContainer = pageContainer;
            _popupContainer = popupContainer;
        }

        public async UniTask PushPage<T>(string pageId = null, bool playAnimation = true,
            Action<(string pageId, Page page)> loadCallback = null
        )
            where T : Page
        {
            if (string.IsNullOrEmpty(pageId)) pageId = typeof(T).Name;
            await _pageContainer.Push<T>(pageId, playAnimation: playAnimation, onLoad: loadCallback).Task;
        }

        public async UniTask PopPage(bool playAnimation = true)
        {
            await _pageContainer.Pop(playAnimation);
        }


        public async UniTask PushPopup<T>(string modelId = null, bool playAnimation = true,
            Action<(string modalId, T modal)> loadCallback = null
        ) where T : Popup
        {
            if (string.IsNullOrEmpty(modelId)) modelId = typeof(T).Name;
            await _popupContainer.Push(modelId, playAnimation, onLoad: loadCallback).Task;
        }

        public async UniTask PopPopup(bool playAnimation = true)
        {
            await _popupContainer.Pop(playAnimation);
        }

        public T GetPopup<T>() where T : Popup
        {
            return _popupContainer.Get<T>();
        }

        public T GetPage<T>() where T : Page
        {
            return _pageContainer.Get<T>();
        }


        public async UniTask PopPageAsync()
        {
            if (_pageContainer.IsInTransition)
            {
                Debug.LogWarning("Transition is running, skipping Pop!");
                return;
            }

            var handle = _pageContainer.Pop(false);
            await handle.Task; // Wait for Pop animation to complete

            Debug.Log("Pop page completed!");
        }

        /// <summary>
        /// Pops the current modal from the stack.
        /// </summary>
        /// <param name="playAnimation">Whether to play the transition animation.</param>
        public void PopModal(int numberModal = 1, bool playAnimation = false)
        {
            if (_popupContainer == null) return;
            if (numberModal <= 1)
            {
                _popupContainer.Pop(playAnimation);
                return;
            }

            _popupContainer.Pop(playAnimation, numberModal);
        }
        


        /// <summary>
        /// Shows a new modal.
        /// </summary>
        /// <param name="modalName">The resource name of the modal prefab.</param>
        /// <param name="playAnimation">Whether to play the transition animation.</param>
        public void PushModal(string modalName, bool playAnimation = false)
        {
            if (_popupContainer == null) return;
            _popupContainer.Push(modalName, playAnimation);
        }

        public void PushModal<T>(Action<(string modalId, T modal)> loadCallback = null)
            where T : Popup
        {
            _popupContainer.Push(typeof(T).Name, true, onLoad: loadCallback);
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
            if (_popupContainer == null) return;
            _popupContainer.Push("ConfirmModal", false, onLoad: x =>
            {
                if (x.modal is IActionPopup confirmPopupController)
                {
                    confirmPopupController.Init(onConfirm, onCancle, action0, action1, action2);
                }
            });
        }

        public void PopModal()
        {
            _popupContainer.Pop(true);
        }

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