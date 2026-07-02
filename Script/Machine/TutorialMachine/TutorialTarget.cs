using UnityEngine;

namespace Game
{
    public abstract class TutorialTarget : MonoBehaviour
    {
        public abstract void HighlightTarget();
        public abstract void StandOutTarget();
        public abstract void SpotlightTarget();
        public abstract void StepFeedback();

        public abstract void DisableTutorial();
    }
}