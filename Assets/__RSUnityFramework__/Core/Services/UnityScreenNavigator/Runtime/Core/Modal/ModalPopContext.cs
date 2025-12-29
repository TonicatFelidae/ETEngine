using System.Collections.Generic;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    internal readonly struct ModalPopContext
    {
        public IReadOnlyList<string> ExitModalIds { get; }
        public IReadOnlyList<Popup> ExitModals { get; }
        public IReadOnlyList<int> ExitModalIndices { get; }

        public string EnterModalId { get; }
        public Popup EnterPopup { get; }

        public Popup FirstExitPopup => ExitModals[0];

        public int FirstExitModalIndex => ExitModalIndices[0];

        private ModalPopContext(
            List<string> exitModalIds,
            List<Popup> exitModals,
            List<int> exitModalIndices,
            string enterModalId,
            Popup enterPopup
        )
        {
            ExitModalIds = exitModalIds;
            ExitModals = exitModals;
            ExitModalIndices = exitModalIndices;
            EnterModalId = enterModalId;
            EnterPopup = enterPopup;
        }

        public static ModalPopContext Create(
            List<string> orderedModalIds,
            Dictionary<string, Popup> modals,
            int popCount
        )
        {
            var exitIds = new List<string>();
            var exitModals = new List<Popup>();
            var indices = new List<int>();

            for (var i = orderedModalIds.Count - 1; i >= orderedModalIds.Count - popCount; i--)
            {
                var id = orderedModalIds[i];
                exitIds.Add(id);
                exitModals.Add(modals[id]);
                indices.Add(i);
            }

            var enterIndex = orderedModalIds.Count - popCount - 1;
            var enterId = enterIndex >= 0 ? orderedModalIds[enterIndex] : null;
            var enterModal = enterId != null ? modals[enterId] : null;

            return new ModalPopContext(exitIds, exitModals, indices, enterId, enterModal);
        }
    }
}