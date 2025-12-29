using System;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    public sealed class AnonymousModalContainerCallbackReceiver : IModalContainerCallbackReceiver
    {
        public AnonymousModalContainerCallbackReceiver(
            Action<(Popup enterModal, Popup exitModal)> onBeforePush = null,
            Action<(Popup enterModal, Popup exitModal)> onAfterPush = null,
            Action<(Popup enterModal, Popup exitModal)> onBeforePop = null,
            Action<(Popup enterModal, Popup exitModal)> onAfterPop = null)
        {
            OnBeforePush = onBeforePush;
            OnAfterPush = onAfterPush;
            OnBeforePop = onBeforePop;
            OnAfterPop = onAfterPop;
        }

        public void BeforePush(Popup enterPopup, Popup exitPopup)
        {
            OnBeforePush?.Invoke((enterPopup, exitPopup));
        }

        public void AfterPush(Popup enterPopup, Popup exitPopup)
        {
            OnAfterPush?.Invoke((enterPopup, exitPopup));
        }

        public void BeforePop(Popup enterPopup, Popup exitPopup)
        {
            OnBeforePop?.Invoke((enterPopup, exitPopup));
        }

        public void AfterPop(Popup enterPopup, Popup exitPopup)
        {
            OnAfterPop?.Invoke((enterPopup, exitPopup));
        }

        public event Action<(Popup enterModal, Popup exitModal)> OnAfterPop;
        public event Action<(Popup enterModal, Popup exitModal)> OnAfterPush;
        public event Action<(Popup enterModal, Popup exitModal)> OnBeforePop;
        public event Action<(Popup enterModal, Popup exitModal)> OnBeforePush;
    }
}