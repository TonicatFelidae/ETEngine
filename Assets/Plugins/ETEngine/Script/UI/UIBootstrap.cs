using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Page;
using VContainer;
using VContainer.Unity;
namespace ETEngine
{
    public class UIBootstrap : IStartable
    {
        readonly PageFactory _pageFactory;
        readonly PopupFactory _popupFactory;
        readonly MainCanvas _mainCanvas;

        public UIBootstrap(MainCanvas mainCanvas, PageFactory pageFactory, PopupFactory popupFactory)
        {
           // _mainCanvas = mainCanvas;
            mainCanvas.ReInitiation(pageFactory, popupFactory);
        }

        public void Start()
        {
            // nothing needed, resolving is enough
        }
    }
}