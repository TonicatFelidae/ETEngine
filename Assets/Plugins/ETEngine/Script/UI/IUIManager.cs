using System;
using Cysharp.Threading.Tasks;
using UnityScreenNavigator.Runtime.Core.Modal;
using UnityScreenNavigator.Runtime.Core.Page;

namespace ETEngine
{
    public interface IUIManager
    {
        public PageContainer PageContainer { get; }
        public PopupContainer PopupContainer { get; }

        /// <summary>
        /// Pushes a new page onto the stack.
        /// </summary>
        /// <param name="pageId">Id</param>
        /// <param name="playAnimation">Play Anim or not</param>
        /// <param name="loadCallback">Need callback</param>
        UniTask PushPage<T>(string pageId = null, bool playAnimation = true,
            Action<(string pageId, Page page)> loadCallback = null
        ) where T : Page;
        /// <summary>
        /// Pops the current page from the stack.
        /// </summary>
        /// <param name="playAnimation"></param>
        UniTask PopPage(bool playAnimation = true);


        UniTask PushPopup<T>(string modelId = null, bool playAnimation = true,
            Action<(string modalId, T modal)> loadCallback = null
        ) where T : Popup;
        async UniTask PushPopup<T>(Action<(string modalId, T modal)> loadCallback
        ) where T : Popup => await PushPopup(null,true, loadCallback);

        UniTask PopPopup(bool playAnimation = true);
        async UniTask PopAllPopup(bool playAnimation = true) => await PopupContainer.Pop(playAnimation, PopupContainer.Count).Task;

        
        /// <summary>
        /// A small notice: no get right after Push like below due plugin using Coroutine
        /// uiManager.Push<Page>();
        /// var page = uiManger.Get<Page>();
        /// </summary>
        T GetPage<T>() where T : Page;
        T GetPopup<T>() where T : Popup;
    }
}