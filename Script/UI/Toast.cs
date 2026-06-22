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
        [SerializeField] private TextMeshProUGUI _toastMessage;
        public void PushNoti(string message, float duration)
        {
            StopAllCoroutines();
            gameObject.SetActive(true);
            _toastMessage.text = message;
            StartCoroutine(HideAfterSeconds(duration));
        }
        IEnumerator HideAfterSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            gameObject.SetActive(false);
        }
    }
}
