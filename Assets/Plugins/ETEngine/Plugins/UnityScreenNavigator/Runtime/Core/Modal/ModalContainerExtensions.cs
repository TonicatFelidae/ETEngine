using System;
using UnityScreenNavigator.Runtime.Core.Shared;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    public static class ModalContainerExtensions
    {
        /// <summary>
        ///     Add callbacks.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="onBeforePush"></param>
        /// <param name="onAfterPush"></param>
        /// <param name="onBeforePop"></param>
        /// <param name="onAfterPop"></param>
        public static void AddCallbackReceiver(this PopupContainer self,
            Action<(Popup enterModal, Popup exitModal)> onBeforePush = null,
            Action<(Popup enterModal, Popup exitModal)> onAfterPush = null,
            Action<(Popup enterModal, Popup exitModal)> onBeforePop = null,
            Action<(Popup enterModal, Popup exitModal)> onAfterPop = null)
        {
            var callbackReceiver =
                new AnonymousModalContainerCallbackReceiver(onBeforePush, onAfterPush, onBeforePop, onAfterPop);
            self.AddCallbackReceiver(callbackReceiver);
        }

        /// <summary>
        ///     Add callbacks.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="popup"></param>
        /// <param name="onBeforePush"></param>
        /// <param name="onAfterPush"></param>
        /// <param name="onBeforePop"></param>
        /// <param name="onAfterPop"></param>
        public static void AddCallbackReceiver(this PopupContainer self, Popup popup,
            Action<Popup> onBeforePush = null, Action<Popup> onAfterPush = null,
            Action<Popup> onBeforePop = null, Action<Popup> onAfterPop = null)
        {
            var callbackReceiver = new AnonymousModalContainerCallbackReceiver();
            callbackReceiver.OnBeforePush += x =>
            {
                var (enterModal, exitModal) = x;
                if (enterModal.Equals(popup))
                {
                    onBeforePush?.Invoke(exitModal);
                }
            };
            callbackReceiver.OnAfterPush += x =>
            {
                var (enterModal, exitModal) = x;
                if (enterModal.Equals(popup))
                {
                    onAfterPush?.Invoke(exitModal);
                }
            };
            callbackReceiver.OnBeforePop += x =>
            {
                var (enterModal, exitModal) = x;
                if (exitModal.Equals(popup))
                {
                    onBeforePop?.Invoke(enterModal);
                }
            };
            callbackReceiver.OnAfterPop += x =>
            {
                var (enterModal, exitModal) = x;
                if (exitModal.Equals(popup))
                {
                    onAfterPop?.Invoke(enterModal);
                }
            };

            var gameObj = self.gameObject;
            if (!gameObj.TryGetComponent<MonoBehaviourDestroyedEventDispatcher>(out var destroyedEventDispatcher))
            {
                destroyedEventDispatcher = gameObj.AddComponent<MonoBehaviourDestroyedEventDispatcher>();
            }

            destroyedEventDispatcher.OnDispatch += () => self.RemoveCallbackReceiver(callbackReceiver);

            self.AddCallbackReceiver(callbackReceiver);
        }
    }
}