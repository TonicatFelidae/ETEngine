using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ETSimpleKit
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class VersionText : MonoBehaviour
    {
        public string customVersionText = "";
        void Start()
        {
            GetComponent<TextMeshProUGUI>().text = "Version : " + Application.version;
            if (!string.IsNullOrEmpty(customVersionText)) GetComponent<TextMeshProUGUI>().text = "Version : " + customVersionText;
        }
    }

}
