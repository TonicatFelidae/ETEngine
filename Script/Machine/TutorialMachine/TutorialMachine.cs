using Game;
using UnityEngine;
using UnityEngine.Events;

namespace ETEngine
{
    /// <summary>
    /// The machine should already in screen 
    /// </summary>
    public class TutorialMachine : MonoBehaviour
    {
        [SerializeField] private TutorialStep[] tutorialSteps;
        [SerializeField] private bool autoFindAllTargetsOnInit = true;
        [SerializeField] private TutorialTarget[] tutorialTargets;
        private bool _isIgnoreTutorialFeedback = false;
        private bool _isTutorialCompleted = false;
        private int _currentStepIndex = -1;
        private GameObject _activePopup;
        public UnityEvent onTutorialCompleted;
        public void Init() => Init(true, false, false);
        public void Init(bool isFirstTime, bool skipTutorial, bool ignoreTutorialFeedback = false)
        {
            _isIgnoreTutorialFeedback = ignoreTutorialFeedback;

            if (autoFindAllTargetsOnInit)
            {
                tutorialTargets = GetComponentsInChildren<TutorialTarget>(true);
            }
            if (skipTutorial)
            {
                // Logic to skip the tutorial
                DisableAllSteps();
                ClearActivePopup();
                _isTutorialCompleted = true;
                gameObject.SetActive(false);
                return;
            }

            if (tutorialSteps == null || tutorialSteps.Length == 0)
            {
                Debug.LogWarning("[TutorialMachine] No tutorial steps found.");
                CompleteTutorial();
                return;
            }

            _isTutorialCompleted = false;

            if (isFirstTime)
            {
                // Logic to start the tutorial for the first time
                _currentStepIndex = 0;
                ActivateStep(_currentStepIndex);
            }
            else
            {
                OnStepComplete(OnTutorialStepComplete.DisableAllTutorials);
                _isTutorialCompleted = true;
                gameObject.SetActive(false);
            }
        }

        public void NextStep()
        {
            if (_isTutorialCompleted || tutorialSteps == null || tutorialSteps.Length == 0)
            {
                return;
            }

            if (_currentStepIndex >= 0 && _currentStepIndex < tutorialSteps.Length)
            {
                var currentStep = tutorialSteps[_currentStepIndex];
                OnStepComplete(currentStep.onCompleted);
            }

            _currentStepIndex++;

            if (_currentStepIndex < tutorialSteps.Length)
            {
                ActivateStep(_currentStepIndex);
            }
            else
            {
                CompleteTutorial();
            }
        }

        public void OnStepComplete(OnTutorialStepComplete action)
        {
            if (tutorialSteps == null) return;

            // If we are not currently running a step (index is out of range)
            if (_currentStepIndex < 0 || _currentStepIndex >= tutorialSteps.Length)
            {
                if (action == OnTutorialStepComplete.DisableAllTutorials)
                {
                    DisableAllSteps();
                }
                return;
            }

            var currentStep = tutorialSteps[_currentStepIndex];

            switch (action)
            {
                case OnTutorialStepComplete.DisableTutorialOnTarget:
                    if (currentStep.target != null)
                    {
                        currentStep.target.DisableTutorial();
                    }
                    ClearActivePopup();
                    break;
                case OnTutorialStepComplete.DisableAllTutorials:
                    DisableAllSteps();
                    break;
                case OnTutorialStepComplete.Feedback:
                    if (!_isIgnoreTutorialFeedback && currentStep.target != null)
                    {
                        currentStep.target.StepFeedback();

                        if (currentStep.showText && !string.IsNullOrWhiteSpace(currentStep.instructionText))
                        {
                            Debug.Log($"[TutorialMachine] Step {_currentStepIndex} feedback: {currentStep.instructionText}");
                        }
                    }

                    currentStep.onCompletedFeedback?.Invoke();
                    break;
                default:
                    break;
            }
        }

        private void ActivateStep(int index)
        {
            if (tutorialSteps == null || index < 0 || index >= tutorialSteps.Length)
            {
                return;
            }


            var currentStep = tutorialSteps[index];

            if (currentStep.showHighlight && currentStep.target != null)
            {
                currentStep.target.HighlightTarget();
            }

            if (currentStep.showStandout && currentStep.target != null)
            {
                currentStep.target.StandOutTarget();
            }

            if (currentStep.showText && !string.IsNullOrWhiteSpace(currentStep.instructionText))
            {
                Debug.Log($"[TutorialMachine] {currentStep.instructionText}");
            }

            ClearActivePopup();
            if (currentStep.showPopup && currentStep.pp_popup != null)
            {
                Vector3 popupPosition = transform.position + currentStep.popupOffset;
                if (currentStep.target != null)
                {
                    popupPosition = currentStep.target.transform.position + currentStep.popupOffset;
                }

                _activePopup = Instantiate(currentStep.pp_popup, popupPosition, Quaternion.identity, transform);
            }
        }

        private void DisableAllSteps()
        {
            if (tutorialSteps == null) return;
            foreach (var step in tutorialSteps)
            {
                if (step.target != null)
                {
                    step.target.DisableTutorial();
                }
            }

            ClearActivePopup();
        }

        private void ClearActivePopup()
        {
            if (_activePopup == null)
            {
                return;
            }

            Destroy(_activePopup);
            _activePopup = null;
        }

        private void CompleteTutorial()
        {
            if (_isTutorialCompleted)
            {
                return;
            }

            _isTutorialCompleted = true;
            ClearActivePopup();
            onTutorialCompleted?.Invoke();
            gameObject.SetActive(false);
        }
    }

    [System.Serializable]
    public struct TutorialStep
    {
        public TutorialTarget target;
        public bool showHighlight;
        public bool showStandout;
        public bool showSpotLight;
        public bool showText;
        public string instructionText;
        public bool showPopup;
        public GameObject pp_popup;
        public Vector3 popupOffset;
        public OnTutorialStepComplete onCompleted;
        public UnityEvent onCompletedFeedback;
    }

    public enum OnTutorialStepComplete
    {
        None,
        DisableTutorialOnTarget,
        DisableAllTutorials,
        Feedback,
    }
}