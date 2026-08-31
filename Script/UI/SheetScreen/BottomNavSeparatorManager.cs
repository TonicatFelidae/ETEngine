using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BottomNavSeparatorManager : MonoBehaviour
{
    [Header("Separators")]
    [SerializeField] private Transform[] separatorLines; // this set by hand and fix amount

    [Header("Layout Settings")]
    [Tooltip("Percentage by which the active button's section is wider than inactive sections (e.g. 20 = 20% wider)")]
    [Range(0f, 100f)]
    [SerializeField] private float widerPercentage = 20f;

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private Ease animationEase = Ease.OutQuad;

    public float WiderPercentage
    {
        get => widerPercentage;
        set => widerPercentage = value;
    }

    /// <summary>
    /// Initializes separators. Automatically discovers child transforms if separatorLines is not assigned in the inspector.
    /// </summary>
    public void Init()
    {
        // Disable any layout group that would conflict with custom positioning
        if (TryGetComponent<HorizontalLayoutGroup>(out var layoutGroup))
        {
            layoutGroup.enabled = false;
        }

        if (separatorLines == null || separatorLines.Length == 0)
        {
            List<Transform> childSeparators = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                childSeparators.Add(transform.GetChild(i));
            }
            separatorLines = childSeparators.ToArray();
        }

        for (int i = 0; i < separatorLines.Length; i++)
        {
            if (separatorLines[i] == null) continue;

            if (separatorLines[i].TryGetComponent<LayoutElement>(out var layoutElement))
            {
                layoutElement.ignoreLayout = true;
            }
        }

        UpdateSeparators(-1, immediate: true);
    }

    /// <summary>
    /// Updates separator positions dynamically so that all slots have equal distance,
    /// while the slot covering the active button is wider by widerPercentage.
    /// </summary>
    /// <param name="activeButtonIndex">Index of currently active button (0-based), or -1 if no button is active.</param>
    /// <param name="immediate">If true, positions are applied instantly without tweening.</param>
    public void UpdateSeparators(int activeButtonIndex, bool immediate = false)
    {
        if (separatorLines == null || separatorLines.Length == 0)
        {
            return;
        }

        int numSeparators = separatorLines.Length;
        int numButtons = numSeparators + 1;

        // Calculate slot weights
        float activeMultiplier = 1f + (widerPercentage / 100f);
        float totalWeight = 0f;
        float[] weights = new float[numButtons];

        for (int i = 0; i < numButtons; i++)
        {
            float w = (activeButtonIndex >= 0 && i == activeButtonIndex) ? activeMultiplier : 1f;
            weights[i] = w;
            totalWeight += w;
        }

        // Calculate target normalized positions for each separator line
        float accumulatedWeight = 0f;
        for (int i = 0; i < numSeparators; i++)
        {
            if (separatorLines[i] == null) continue;

            accumulatedWeight += weights[i];
            float targetNormalizedX = totalWeight > 0f ? (accumulatedWeight / totalWeight) : ((i + 1f) / numButtons);

            if (separatorLines[i] is RectTransform rect)
            {
                ApplySeparatorPosition(rect, targetNormalizedX, immediate);
            }
        }
    }

    /// <summary>
    /// Overload for default calls without explicit immediate flag.
    /// </summary>
    public void UpdateSeparators(int activeButtonIndex)
    {
        UpdateSeparators(activeButtonIndex, immediate: false);
    }

    private void ApplySeparatorPosition(RectTransform rect, float targetNormalizedX, bool immediate)
    {
        rect.DOKill();

        rect.pivot = new Vector2(0.5f, rect.pivot.y);
        rect.anchoredPosition = new Vector2(0f, rect.anchoredPosition.y);

        if (immediate || animationDuration <= 0f || !Application.isPlaying)
        {
            rect.anchorMin = new Vector2(targetNormalizedX, rect.anchorMin.y);
            rect.anchorMax = new Vector2(targetNormalizedX, rect.anchorMax.y);
        }
        else
        {
            float startAnchorX = rect.anchorMin.x;
            DOTween.To(() => startAnchorX, x =>
            {
                if (rect != null)
                {
                    rect.anchorMin = new Vector2(x, rect.anchorMin.y);
                    rect.anchorMax = new Vector2(x, rect.anchorMax.y);
                }
            }, targetNormalizedX, animationDuration)
            .SetEase(animationEase)
            .SetTarget(rect);
        }
    }

    /// <summary>
    /// Sets all separators' visibility.
    /// </summary>
    public void ShowAllSeparators(bool show = true)
    {
        if (separatorLines == null)
        {
            return;
        }

        for (int i = 0; i < separatorLines.Length; i++)
        {
            if (separatorLines[i] != null)
            {
                separatorLines[i].gameObject.SetActive(show);
            }
        }
    }

    private void OnDestroy()
    {
        if (separatorLines != null)
        {
            for (int i = 0; i < separatorLines.Length; i++)
            {
                if (separatorLines[i] != null)
                {
                    separatorLines[i].DOKill();
                }
            }
        }
    }
}
