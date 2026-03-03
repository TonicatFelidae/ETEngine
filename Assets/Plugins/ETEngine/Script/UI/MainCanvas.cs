using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Modal;
using UnityScreenNavigator.Runtime.Core.Page;
using VContainer;
using VContainer.Unity;

namespace ETEngine
{
    public class MainCanvas: MonoBehaviour
    {
        public PageContainer pageContainer;
        public PopupContainer popupContainer;
        public LoadingPageBase loadingPage;
        public void ReInitiation(PageFactory pageFactory, PopupFactory popupFactory)
        {
            pageContainer.Init(pageFactory);
            popupContainer.Init(popupFactory);
        }
    }
}
