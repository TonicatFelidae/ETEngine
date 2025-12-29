using UnityEngine;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;
using Object = UnityEngine.Object;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    /// <summary>
    ///     Implementation of <see cref="IModalBackdropHandler" /> that generates a backdrop only for the first modal
    /// </summary>
    internal sealed class OnlyFirstBackdropModalBackdropHandler : IModalBackdropHandler
    {
        private readonly ModalBackdrop _prefab;

        public OnlyFirstBackdropModalBackdropHandler(ModalBackdrop prefab)
        {
            _prefab = prefab;
        }

        public AsyncProcessHandle BeforeModalEnter(Popup popup, int modalIndex, bool playAnimation)
        {
            var parent = (RectTransform)popup.transform.parent;

            // Do not generate a backdrop for the first modal
            if (modalIndex != 0)
                return AsyncProcessHandle.Completed();

            var backdrop = Object.Instantiate(_prefab);
            backdrop.Setup(parent, modalIndex);
            backdrop.transform.SetSiblingIndex(0);
            return backdrop.Enter(playAnimation);
        }

        public void AfterModalEnter(Popup popup, int modalIndex, bool playAnimation)
        {
        }

        public AsyncProcessHandle BeforeModalExit(Popup popup, int modalIndex, bool playAnimation)
        {
            // Do not remove the backdrop for the first modal
            if (modalIndex != 0)
                return AsyncProcessHandle.Completed();

            var backdrop = popup.transform.parent.GetChild(0).GetComponent<ModalBackdrop>();
            return backdrop.Exit(playAnimation);
        }

        public void AfterModalExit(Popup popup, int modalIndex, bool playAnimation)
        {
            // Do not remove the backdrop for the first modal
            if (modalIndex != 0)
                return;

            var backdrop = popup.transform.parent.GetChild(0).GetComponent<ModalBackdrop>();
            Object.Destroy(backdrop.gameObject);
        }
    }
}