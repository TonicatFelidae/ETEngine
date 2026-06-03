using ETEngine;
using Game.UI;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using System;
using DG.Tweening;
public enum BottomNavButtonType
{
    None,
    Shop,
    Mission,
    Home,
    Gacha,
    FanGame
}

public class BottomNavButtonBase : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private GameObject notificationBadge;
    public string viewID; // This should be PlayScreen, HomeScreen, etc. It should match the sheet name registered in MainScreen.
    public void SetNotificationBadge(bool active)
    {
        if (notificationBadge != null)
            notificationBadge.SetActive(active);
    }
    public void SetOnClick(UnityEngine.Events.UnityAction action)
    {
        button.onClick.AddListener(action);
    }
    public virtual void SetActive(bool isActive)
    {
        float targetScale = isActive ? 1.2f : 1f;
        transform.DOScale(targetScale, 0.3f).SetEase(Ease.OutBack);
    }
    public void SetInteractable(bool isInteractable)
    {
        button.interactable = isInteractable;
    }
}