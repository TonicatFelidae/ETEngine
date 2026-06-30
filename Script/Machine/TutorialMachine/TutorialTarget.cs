using UnityEngine;

namespace Game
{
    public class TutorialTarget : MonoBehaviour
    {
        public virtual void HighlightTarget()
        {
        }

        public virtual void StepFeedback()
        {
        }

        // Keep typo-compatible API to avoid breaking old call sites.
        public virtual void SetpFeedback()
        {
            StepFeedback();
        }
    }
}