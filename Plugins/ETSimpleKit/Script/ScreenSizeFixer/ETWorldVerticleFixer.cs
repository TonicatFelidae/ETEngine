using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
/// <summary>
/// Put this in world that have sprite on it to determine gameplay zone
/// </summary>
public class ETWorldVerticleFixer : MonoBehaviour
{
    public ETWorldLine topWorldLine;
    public ETWorldLine botWorldLine;
    private ETWorldVerticleFixer_UIAgent _eTWorldVerticleFixer_UIAgent;
    public ETWorldVerticleFixer_UIAgent UIAgent
    {
        get
        {
            if (_eTWorldVerticleFixer_UIAgent == null)
            {
                _eTWorldVerticleFixer_UIAgent = GameObject.FindAnyObjectByType<ETWorldVerticleFixer_UIAgent>(FindObjectsInactive.Include);
            }
            return _eTWorldVerticleFixer_UIAgent;
        }
    }
    private SpriteRenderer _spriteRenderer;
    private float curWitdh;
    private float curHeight;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        curWitdh = _spriteRenderer.size.x;
        curHeight = curWitdh * UIAgent.GetUIAspectRation_HeightOnWitdh();
        topWorldLine.transform.position = new Vector3(0, curHeight/2, 0);
        botWorldLine.transform.position = new Vector3(0, -curHeight/2, 0);
        //UIAgent
    }
}

