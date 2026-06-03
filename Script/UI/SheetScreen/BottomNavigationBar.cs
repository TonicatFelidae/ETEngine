using Cysharp.Threading.Tasks;
using DG.Tweening;
using ETEngine;
using Game;
using Game.UI;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityScreenNavigator.Runtime.Core.Page;
using UnityScreenNavigator.Runtime.Core.Sheet;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;
using VContainer;

public class BottomNavigationBar : MonoBehaviour
{
    [Inject] IUIManager _UIManager;

    [Header("Bottom Navigation Bar")]
    [SerializeField] BottomNavButtonBase[] _buttons;
    public string currentViewID = "None";
    public UnityAction<string> OnTouchNavButton { get; set; }

    private void Start()
    {
        SetupButtons();
        UpdateState();
    }

    public BottomNavButtonBase GetBottomNavButtonBase(string viewID)
    {
        foreach (var btn in _buttons)
        {
            if (btn.viewID == viewID)
                return btn;
        }
        return null;
    }
    private void SetupButtons()
    {
        foreach (var btn in _buttons)
        {
            btn.SetOnClick(() => TouchNavButton(btn.viewID));
        }
    }
    protected virtual void UpdateState()
    {
        foreach (var btn in _buttons)
        {
            btn.SetActive(btn.viewID == currentViewID);
        }

    }
    protected virtual void EnableButtons(bool enable)
    {
        foreach (var btn in _buttons)
        {
            btn.SetInteractable(enable);
        }
    }

    public async void TouchNavButton(string viewID)
    {
        if (currentViewID != viewID)
        {
            Debug.Log($"[BottomNavigationBar] TouchNavButton: viewID={viewID}, currentViewID={currentViewID}");
            currentViewID = viewID;
            OnTouchNavButton?.Invoke(viewID);
            EnableButtons(false);
            UpdateState();
            InteracEffect();
            await (_UIManager.GetSheetContainer<SheetPage>()).Show(viewID, true);
            EnableButtons(true);
        }
    }


    public void InteracEffect()
    {
        // Punch scale effect from 1 to 1.03 for both resource displays
        Vector3 punchScale = new Vector3(0.03f, 0.03f, 0.03f);
        float duration = 0.4f;
        transform.DOPunchScale(punchScale, duration, 2, 0.5f);
    }
}
