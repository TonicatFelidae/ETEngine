using ET.SupportKit;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class BackgroundSizeFix : MonoBehaviour
{
    RectTransform _rectTransform;
    Image _image;
    public float width = 1;
    public float height = 1;
    public FixType fixType;
    public bool enableDebug;
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        if (enableDebug)
        {
            Debug.Log($"BackGroundSize {_rectTransform.rect.width}:{_rectTransform.rect.height}");
        }
    }
    private void Start()
    {
        float ratio = width / height;
        float spriteRatio = _image.sprite.bounds.size.x / _image.sprite.bounds.size.y;
        switch (fixType)
        {
            case FixType.FixRatioBaseOnWidth:
                SetHeight(_rectTransform.rect.width / ratio);
                break;
            case FixType.FixRatioBaseOnHeight:
                SetWidth(_rectTransform.rect.height * ratio);
                //_rectTransform.rect = r1;

                break;
            case FixType.SpriteRatioBaseOnWidth:
                SetHeight(_rectTransform.rect.width / ratio);
                break;
            case FixType.SpriteRatioBaseOnHeight:
                SetWidth(_rectTransform.rect.height * ratio);
                break;

            default:
                break;
        }
        if (enableDebug)
        {
            Debug.Log($"BackGroundSize AfterFix {_rectTransform.rect.width}:{_rectTransform.rect.height}");
        }
    }
    public void SetWidth(float size)
    {
        Vector2 sizeDelta = _rectTransform.sizeDelta;
        sizeDelta.x = size;
        _rectTransform.sizeDelta = sizeDelta;
    }
    public void SetHeight(float size)
    {
        Vector2 sizeDelta = _rectTransform.sizeDelta;
        sizeDelta.y = size;
        _rectTransform.sizeDelta = sizeDelta;
    }
    public enum FixType
    {
        FixRatioBaseOnWidth,
        FixRatioBaseOnHeight,
        SpriteRatioBaseOnWidth,
        SpriteRatioBaseOnHeight,
    }
}
