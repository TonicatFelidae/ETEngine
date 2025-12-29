using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace  Game
{
    public class WelcomeUI : MonoBehaviour
    {
        public List<string> listMessegers;
        [SerializeField] private TextMeshProUGUI tx_messeger;
        private int currentMessageIndex = 0;

        void Start()
        {
            Debug.Log("WelcomeUI Start");
            if (listMessegers != null && listMessegers.Count > 0 && tx_messeger != null)
            {
                currentMessageIndex = 0;
                tx_messeger.text = listMessegers[currentMessageIndex];
            }
        }

        public void OnClickNextButton()
        {
            Debug.Log("Next Button Clicked");
            if (listMessegers == null || listMessegers.Count == 0 || tx_messeger == null)
                return;

            currentMessageIndex = (currentMessageIndex + 1) % listMessegers.Count;
            tx_messeger.text = listMessegers[currentMessageIndex];
        }
    }
}