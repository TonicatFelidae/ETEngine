using UnityEngine;

namespace ETEngine
{
    public class LoadingUISample : LoadingPageBase
    {
        [SerializeField] private TMPro.TMP_Text _messageText;
        public override void Hide()
        {
            gameObject.SetActive(false);
            if (_messageText != null)
            {
                _messageText.text = string.Empty;
            }
        }

        public override void SetText(string value)
        {
            if (_messageText != null)
            {
                _messageText.text = value ?? string.Empty;
            }
        }

        public override void Show(string messege = null)
        {
            if (!string.IsNullOrEmpty(messege))
            {
                SetText(messege);
            }
            gameObject.SetActive(true);
        }
    }
}