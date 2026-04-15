using UnityEngine;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    /// <summary>
    ///     Implementation of <see cref="IModalBackdropHandler" /> that generates a backdrop for each modal
    /// </summary>
    internal sealed class GeneratePerModalModalBackdropHandler : IModalBackdropHandler
    {
        private readonly ModalBackdrop _prefab;

        public GeneratePerModalModalBackdropHandler(ModalBackdrop prefab)
        {
            _prefab = prefab;
        }

        public AsyncProcessHandle BeforeModalEnter(Popup popup, int modalIndex, bool playAnimation)
        {
            var parent = (RectTransform)popup.transform.parent;
            var backdrop = Object.Instantiate(_prefab);
            backdrop.Setup(parent, modalIndex);
            var backdropSiblingIndex = modalIndex * 2;
            backdrop.transform.SetSiblingIndex(backdropSiblingIndex);
            return backdrop.Enter(playAnimation);
        }

        public void AfterModalEnter(Popup popup, int modalIndex, bool playAnimation)
        {
        }

        public AsyncProcessHandle BeforeModalExit(Popup popup, int modalIndex, bool playAnimation)
        {
            var backdropSiblingIndex = modalIndex * 2;
            var backdrop = popup.transform.parent.GetChild(backdropSiblingIndex).GetComponent<ModalBackdrop>();
            return backdrop.Exit(playAnimation);
        }

        public void AfterModalExit(Popup popup, int modalIndex, bool playAnimation)
        {
            var backdropSiblingIndex = modalIndex * 2;
            var backdrop = popup.transform.parent.GetChild(backdropSiblingIndex).GetComponent<ModalBackdrop>();
            Object.Destroy(backdrop.gameObject);
        }
    }
}