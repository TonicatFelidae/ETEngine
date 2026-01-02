using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    internal sealed class ModalLifecycleHandler
    {
        private readonly IModalBackdropHandler _backdropHandler;
        private readonly IEnumerable<IModalContainerCallbackReceiver> _callbackReceivers;
        private readonly RectTransform _containerTransform;

        public ModalLifecycleHandler(
            RectTransform containerTransform,
            IEnumerable<IModalContainerCallbackReceiver> callbackReceivers,
            IModalBackdropHandler backdropHandler
        )
        {
            _containerTransform = containerTransform;
            _callbackReceivers = callbackReceivers;
            _backdropHandler = backdropHandler;
        }

        public IEnumerator AfterLoad(ModalPushContext context)
        {
            yield return context.EnterPopup.AfterLoad(_containerTransform);
        }

        public IEnumerator BeforePush(ModalPushContext context)
        {
            foreach (var receiver in _callbackReceivers)
                receiver.BeforePush(context.EnterPopup, context.ExitPopup);

            var handles = new List<AsyncProcessHandle>();
            if (context.ExitPopup != null)
                handles.Add(context.ExitPopup.BeforeExit(true, context.EnterPopup));

            handles.Add(context.EnterPopup.BeforeEnter(true, context.ExitPopup));
            foreach (var handle in handles)
                while (!handle.IsTerminated)
                    yield return handle;
        }

        public IEnumerator Push(ModalPushContext context, bool playAnimation)
        {
            var handles = new List<AsyncProcessHandle>
            {
                _backdropHandler.BeforeModalEnter(context.EnterPopup, context.EnterModalIndex, playAnimation)
            };

            if (context.ExitPopup != null)
                handles.Add(context.ExitPopup.Exit(true, playAnimation, context.EnterPopup));

            handles.Add(context.EnterPopup.Enter(true, playAnimation, context.ExitPopup));
            foreach (var handle in handles)
                while (!handle.IsTerminated)
                    yield return handle;

            _backdropHandler.AfterModalEnter(context.EnterPopup, context.EnterModalIndex, true);
        }

        public void AfterPush(ModalPushContext context)
        {
            if (context.ExitPopup != null)
                context.ExitPopup.AfterExit(true, context.EnterPopup);

            context.EnterPopup.AfterEnter(true, context.ExitPopup);

            foreach (var receiver in _callbackReceivers)
                receiver.AfterPush(context.EnterPopup, context.ExitPopup);
        }

        public IEnumerator BeforePop(ModalPopContext context)
        {
            foreach (var receiver in _callbackReceivers)
                receiver.BeforePop(context.EnterPopup, context.FirstExitPopup);

            var handles = new List<AsyncProcessHandle>();
            foreach (var exitModal in context.ExitModals)
                handles.Add(exitModal.BeforeExit(false, context.EnterPopup));

            if (context.EnterPopup != null)
                handles.Add(context.EnterPopup.BeforeEnter(false, context.FirstExitPopup));

            foreach (var handle in handles)
                while (!handle.IsTerminated)
                    yield return handle;
        }

        public IEnumerator Pop(ModalPopContext context, bool playAnimation)
        {
            var handles = new List<AsyncProcessHandle>();

            for (var i = 0; i < context.ExitModals.Count; i++)
            {
                var exitModal = context.ExitModals[i];
                var exitModalIndex = context.ExitModalIndices[i];
                var partner = i == context.ExitModals.Count - 1 ? context.EnterPopup : context.ExitModals[i + 1];

                handles.Add(_backdropHandler.BeforeModalExit(exitModal, exitModalIndex, playAnimation));
                handles.Add(exitModal.Exit(false, playAnimation, partner));
            }

            if (context.EnterPopup != null)
                handles.Add(context.EnterPopup.Enter(false, playAnimation, context.FirstExitPopup));

            foreach (var handle in handles)
                while (!handle.IsTerminated)
                    yield return handle;

            for (var i = 0; i < context.ExitModals.Count; i++)
            {
                var exitModal = context.ExitModals[i];
                var exitModalIndex = context.ExitModalIndices[i];
                _backdropHandler.AfterModalExit(exitModal, exitModalIndex, playAnimation);
            }
        }

        public void AfterPop(ModalPopContext context)
        {
            foreach (var exitModal in context.ExitModals)
                exitModal.AfterExit(false, context.EnterPopup);
            if (context.EnterPopup != null)
                context.EnterPopup.AfterEnter(false, context.FirstExitPopup);

            foreach (var receiver in _callbackReceivers)
                receiver.AfterPop(context.EnterPopup, context.FirstExitPopup);
        }

        public IEnumerator AfterPopRoutine(ModalPopContext context)
        {
            var handles = context.ExitModals.Select(exitModal => exitModal.BeforeRelease());

            foreach (var handle in handles)
                while (!handle.IsTerminated)
                    yield return handle;
        }
    }
}