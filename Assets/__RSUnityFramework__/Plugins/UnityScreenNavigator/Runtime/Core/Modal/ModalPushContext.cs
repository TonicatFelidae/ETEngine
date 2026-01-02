using System.Collections.Generic;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    internal readonly struct ModalPushContext
    {
        public string ModalId { get; }
        public Popup EnterPopup { get; }

        public string ExitModalId { get; }
        public Popup ExitPopup { get; }

        public int EnterModalIndex { get; }

        private ModalPushContext(
            string modalId,
            Popup enterPopup,
            string exitModalId,
            Popup exitPopup,
            int enterModalIndex
        )
        {
            ModalId = modalId;
            EnterPopup = enterPopup;
            ExitModalId = exitModalId;
            ExitPopup = exitPopup;
            EnterModalIndex = enterModalIndex;
        }

        public static ModalPushContext Create(
            string modalId,
            Popup enterPopup,
            List<string> orderedModalIds,
            Dictionary<string, Popup> modals
        )
        {
            var hasExit = orderedModalIds.Count > 0;
            var exitId = hasExit ? orderedModalIds[orderedModalIds.Count - 1] : null;
            var exitModal = hasExit ? modals[exitId] : null;

            var enterIndex = modals.Count;

            return new ModalPushContext(modalId, enterPopup, exitId, exitModal, enterIndex);
        }
    }
}