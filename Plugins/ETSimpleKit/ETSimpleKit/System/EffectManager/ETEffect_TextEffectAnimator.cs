using TMPro;
using UnityEngine;

namespace ETSimpleKit.EffectSystem
{
    public class ETEffect_TextEffectAnimator : ETEffect
    {
        [Header("REFERENCES")]
        public Animator animator;
        public TextMeshPro text;
        public override void Init(string textData)
        {
            text.text = textData;
        }
    }

}
