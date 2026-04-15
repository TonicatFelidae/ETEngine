using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETSimpleKit
{
    [Serializable]
    public class ButtonNavigation : MonoBehaviour
    {
        public bool autoGetButton = true;
        public bool enableKeyCodeInvokeButton = false;
        public KeyCode keyCode = KeyCode.Escape;
        public ButtonNavigationData data;
        private void Awake()
        {
            if(autoGetButton)
            {
                if (GetComponent<Button>()) data.button = GetComponent<Button>();
            }
        }
        private void Start()
        {
            ETNavigation.Instance.BindButtonNavigation(data);
        }
        private void Update()
        {
            if (enableKeyCodeInvokeButton)
            {
                if (Input.GetKeyDown(keyCode))
                {
                    data.button.onClick.Invoke();   
                }
            }
        }
    }
}
