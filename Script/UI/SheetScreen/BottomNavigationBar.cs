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
    public string sheetContainerID = "SheetContainer";
    public UnityAction<string> OnTouchNavButton { get; set; }
    public GameObject[] bottomButtonDecorations; // Array to hold references to the decorative elements for each button

    [Header("Layout & Animation Settings")]
    [SerializeField] private float _scaleFactor = 1.2f;
    [SerializeField] private float _animationDuration = 0.3f;
    [SerializeField] private Ease _animationEase = Ease.OutQuad;

    private RectTransform _rectTransform;

    public float ScaleFactor
    {
        get => _scaleFactor;
        set => _scaleFactor = value;
    }

    public float AnimationDuration
    {
        get => _animationDuration;
        set => _animationDuration = value;
    }

    public Ease AnimationEase
    {
        get => _animationEase;
        set => _animationEase = value;
    }
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateState();
        InteracEffect(immediate: true);
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
    private bool _isSetup = false;
    private void SetupButtons()
    {
        if (_buttons == null || _buttons.Length == 0)
        {
            _buttons = GetComponentsInChildren<BottomNavButtonBase>(true);
        }
        if (!_isSetup && _buttons != null)
        {
            _isSetup = true;
            foreach (var btn in _buttons)
            {
                if (btn != null)
                {
                    btn.SetOnClick(() => TouchNavButton(btn.viewID));
                }
            }
        }
    }
    public virtual void UpdateState()
    {
        SetupButtons();
        foreach (var btn in _buttons)
        {
            if (btn != null)
            {
                btn.SetActive(btn.viewID == currentViewID);
            }
        }

    }
    protected virtual void EnableButtons(bool enable)
    {
        foreach (var btn in _buttons)
        {
            if (btn != null)
            {
                btn.SetInteractable(enable);
            }
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
            var container = _UIManager.GetSheetContainer<SheetPage>(sheetContainerID);

            // Sheets are registered lazily so an unopened tab costs nothing: the first touch
            // of a tab is what loads its bundle. Registering an already-registered id is a
            // no-op, so this stays cheap on every later touch.
            if (!container.IsRegistered(viewID))
            {
                await container.Register(viewID, null, true, viewID);
            }

            await container.Show(viewID, true);
            EnableButtons(true);
        }
    }


    public void InteracEffect(bool immediate = false)
    {
        if (_buttons == null || _buttons.Length == 0)
        {
            SetupButtons();
            if (_buttons == null || _buttons.Length == 0)
                return;
        }

        int selectedIndex = -1;
        for (int i = 0; i < _buttons.Length; i++)
        {
            if (_buttons[i] != null && _buttons[i].viewID == currentViewID)
            {
                selectedIndex = i;
                break;
            }
        }

        int numButtons = _buttons.Length;
        if (_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
        }
        float totalWidth = (_rectTransform != null && _rectTransform.rect.width > 0f) ? _rectTransform.rect.width : 1080f;

        float activeMultiplier = Mathf.Max(1f, _scaleFactor);
        float totalWeight = 0f;
        float[] weights = new float[numButtons];

        for (int i = 0; i < numButtons; i++)
        {
            float w = (selectedIndex >= 0 && i == selectedIndex) ? activeMultiplier : 1f;
            weights[i] = w;
            totalWeight += w;
        }

        float accumulatedWeight = 0f;
        for (int i = 0; i < numButtons; i++)
        {
            var btn = _buttons[i];
            if (btn == null) continue;

            float slotWeight = weights[i];
            float startNormX = totalWeight > 0f ? (accumulatedWeight / totalWeight) : ((float)i / numButtons);
            accumulatedWeight += slotWeight;
            float endNormX = totalWeight > 0f ? (accumulatedWeight / totalWeight) : ((float)(i + 1) / numButtons);

            float slotWidth = (endNormX - startNormX) * totalWidth;
            float startX = startNormX * totalWidth;

            if (btn.transform is RectTransform btnRect)
            {
                float targetX = startX + btnRect.pivot.x * slotWidth;
                float targetWidth = slotWidth;
                float height = btnRect.sizeDelta.y > 0f ? btnRect.sizeDelta.y : 160f;
                float posY = height / 2f;

                btnRect.DOKill();

                if (immediate || _animationDuration <= 0f || !Application.isPlaying)
                {
                    btnRect.sizeDelta = new Vector2(targetWidth, height);
                    btnRect.anchoredPosition = new Vector2(targetX, posY);
                }
                else
                {
                    float startWidth = btnRect.sizeDelta.x;
                    float startAnchoredPosX = btnRect.anchoredPosition.x;

                    DOTween.To(() => 0f, t =>
                    {
                        if (btnRect != null)
                        {
                            float currentWidth = Mathf.LerpUnclamped(startWidth, targetWidth, t);
                            float currentX = Mathf.LerpUnclamped(startAnchoredPosX, targetX, t);
                            btnRect.sizeDelta = new Vector2(currentWidth, height);
                            btnRect.anchoredPosition = new Vector2(currentX, posY);
                        }
                    }, 1f, _animationDuration)
                    .SetEase(_animationEase)
                    .SetTarget(btnRect);
                }
            }
        }

    }
    public void ShowAllButtons(bool show)
    {
        foreach (var btn in _buttons)
        {
            btn.gameObject.SetActive(show);
        }
        foreach (var deco in bottomButtonDecorations)
        {
            deco.gameObject.SetActive(show);
        }

    }

    private void OnDestroy()
    {
        if (_buttons != null)
        {
            for (int i = 0; i < _buttons.Length; i++)
            {
                if (_buttons[i] != null && _buttons[i].transform != null)
                {
                    _buttons[i].transform.DOKill();
                }
            }
        }
    }
}
