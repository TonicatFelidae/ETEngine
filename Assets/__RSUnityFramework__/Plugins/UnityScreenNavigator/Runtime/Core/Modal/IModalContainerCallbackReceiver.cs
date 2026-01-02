namespace UnityScreenNavigator.Runtime.Core.Modal
{
    public interface IModalContainerCallbackReceiver
    {
        void BeforePush(Popup enterPopup, Popup exitPopup);

        void AfterPush(Popup enterPopup, Popup exitPopup);

        void BeforePop(Popup enterPopup, Popup exitPopup);

        void AfterPop(Popup enterPopup, Popup exitPopup);
    }
}