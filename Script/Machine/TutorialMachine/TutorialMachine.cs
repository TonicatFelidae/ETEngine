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
                OnStepComplete(OnTutorialStepComplete.Disable);
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

            // Logic to move to the next tutorial step
            if (_currentStepIndex >= 0 && _currentStepIndex < tutorialSteps.Length)
            {
                OnStepComplete(OnTutorialStepComplete.Feedback);
                OnStepComplete(OnTutorialStepComplete.Disable);
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
                if (action == OnTutorialStepComplete.Disable)
                {
                    DisableAllSteps();
                }
                else if (action == OnTutorialStepComplete.Destroy)
                {
                    DestroyAllSteps();
                }
                return;
            }

            var currentStep = tutorialSteps[_currentStepIndex];

            switch (action)
            {
                case OnTutorialStepComplete.Disable:
                    // Logic to disable the current tutorial step
                    if (currentStep.target != null)
                    {
                        currentStep.target.gameObject.SetActive(false);
                    }
                    ClearActivePopup();
                    break;
                case OnTutorialStepComplete.Destroy:
                    // Logic to destroy the current tutorial step
                    if (currentStep.target != null)
                    {
                        Destroy(currentStep.target.gameObject);
                    }
                    ClearActivePopup();
                    break;
                case OnTutorialStepComplete.Feedback:
                    // Logic to provide feedback for the current tutorial step
                    if (!_isIgnoreTutorialFeedback && currentStep.target != null)
                    {
                        currentStep.target.StepFeedback();

                        if (currentStep.showText && !string.IsNullOrWhiteSpace(currentStep.instructionText))
                        {
                            Debug.Log($"[TutorialMachine] Step {_currentStepIndex} feedback: {currentStep.instructionText}");
                        }
                    }
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

            // Activate the current step's target, deactivate others
            for (int i = 0; i < tutorialSteps.Length; i++)
            {
                var step = tutorialSteps[i];
                if (step.target != null)
                {
                    step.target.gameObject.SetActive(i == index);
                }
            }

            var currentStep = tutorialSteps[index];

            if (currentStep.showHighlight && currentStep.target != null)
            {
                currentStep.target.HighlightTarget();
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
                    step.target.gameObject.SetActive(false);
                }
            }

            ClearActivePopup();
        }

        private void DestroyAllSteps()
        {
            if (tutorialSteps == null) return;
            foreach (var step in tutorialSteps)
            {
                if (step.target != null)
                {
                    Destroy(step.target.gameObject);
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
        public bool showText;
        public string instructionText;
        public bool showPopup;
        public GameObject pp_popup;
        public Vector3 popupOffset;
    }

    public enum OnTutorialStepComplete
    {
        Disable,
        Destroy,
        Feedback,
    }
}