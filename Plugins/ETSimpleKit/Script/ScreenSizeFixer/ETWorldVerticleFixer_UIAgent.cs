using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
/// <summary>
/// UIAgent of ETWorldVerticleFixer
/// Put this on gameplay frame
/// </summary>
public class ETWorldVerticleFixer_UIAgent : MonoBehaviour
{
    public RectTransform RectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
            return _rectTransform ;
        }
    }

    RectTransform _rectTransform;
    /// <summary>
    /// a = h/w
    /// </summary>
    /// <returns></returns>
    public float GetUIAspectRation_HeightOnWitdh()
    {

        return RectTransform.rect.y / RectTransform.rect.x;
    }
}
