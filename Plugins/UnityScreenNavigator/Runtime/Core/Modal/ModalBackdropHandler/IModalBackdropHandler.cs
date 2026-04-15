using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    internal interface IModalBackdropHandler
    {
        AsyncProcessHandle BeforeModalEnter(Popup popup, int modalIndex, bool playAnimation);

        void AfterModalEnter(Popup popup, int modalIndex, bool playAnimation);

        AsyncProcessHandle BeforeModalExit(Popup popup, int modalIndex, bool playAnimation);

        void AfterModalExit(Popup popup, int modalIndex, bool playAnimation);
    }
}