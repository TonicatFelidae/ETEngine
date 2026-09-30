using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Modal;
using UnityScreenNavigator.Runtime.Core.Page;
using VContainer;
using VContainer.Unity;

namespace ETEngine
{
    public class Toast : MonoBehaviour
    {
        static readonly int ShowState = Animator.StringToHash("Show");
        static readonly int HideState = Animator.StringToHash("Hide");

        [SerializeField] private TextMeshProUGUI _toastMessage;
        // Optional: with an Animator (states "Show" / "Hide") the toast animates in and
        // out; without one it simply appears and disappears.
        [SerializeField] private Animator _animator;
        [SerializeField] private float _hideDuration = 0.4f;

        public void PushNoti(string message, float duration)
        {
            StopAllCoroutines();
            gameObject.SetActive(true);
            _toastMessage.text = message;
            if (_animator != null) _animator.Play(ShowState, 0, 0f);
            StartCoroutine(HideAfterSeconds(duration));
        }
        IEnumerator HideAfterSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            if (_animator != null)
            {
                _animator.Play(HideState, 0, 0f);
                yield return new WaitForSeconds(_hideDuration);
            }
            gameObject.SetActive(false);
        }
    }
}
