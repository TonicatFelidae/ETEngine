using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Modal;
using UnityScreenNavigator.Runtime.Core.Page;
using VContainer;
using VContainer.Unity;

namespace ETEngine
{
    public class MainCanvas : MonoBehaviour
    {
        [SerializeField] private bool _isVerticalMobile;
        public PageContainer pageContainer;
        public PopupContainer popupContainer;
        public LoadingPageBase loadingPage;
        public Toast toastManager;
        public Transform overlayContainer;
        public Transform ISOsafeArea;
        void Awake()
        {
#if UNITY_ANDROID
            ISOsafeArea.gameObject.SetActive(false);
#else 
            ISOsafeArea.gameObject.SetActive(true && _isVerticalMobile);
#endif

        }
        public void ReInitiation(PageFactory pageFactory, PopupFactory popupFactory)
        {
            pageContainer.Init(pageFactory);
            popupContainer.Init(popupFactory);
        }
    }
}
