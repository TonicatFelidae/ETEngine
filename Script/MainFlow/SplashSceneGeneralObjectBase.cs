using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
namespace ETEngine
{
    public class SplashSceneGeneralObjectBase : MonoBehaviour
    {
        private ISplashScreen _splashScreen;
        [Header("Scene initialization")]
        [SerializeField] private FPSConfig _initFPSConfig;
        [SerializeField] private bool _initUnityService;
        [Header("Load next scene")]
        [SerializeField] private bool _loadNextSceneAutomatically = true;
        [SerializeField] private string _nextSceneName = "Game";
        [SerializeField] private LoadSceneMode _loadSceneMode = LoadSceneMode.Single;
        [SerializeField] private string[] _unloadSceneNamesAfterLoad = new string[0];

        [Tooltip("How long the splash takes to fade before it is taken off the canvas. " +
                 "Runs alongside the scene load, so the next scene is revealed through it.")]
        [SerializeField] private float _splashFadeOutSeconds = 0.4f;

        [Tooltip("The last one will show after the scene is loaded")]
        [SerializeField]
        private DelayProgressAndMessage[] _fakeBeginDelayProgressAndMessages = new DelayProgressAndMessage[]
        {
            new DelayProgressAndMessage
            {
                progress = 0.01f,
                message = "Starting Initialization",
                delayMilliseconds = 500
            },
            new DelayProgressAndMessage
            {
                progress = 0.05f,
                message = "Loading",
                delayMilliseconds = 2000
            },
            new DelayProgressAndMessage
            {
                progress = 0.1f,
                message = "Done loading scene",
                delayMilliseconds = 100
            }
        };

        public Progress<float> Progress { get; set; } = new();
        private async void Start()
        {
            Progress = new Progress<float>(value =>
            {
                // Null-conditional: the splash is destroyed at the scene swap, well
                // before InitializeAll reports its final 1.0.
                _splashScreen?.UpdateProgressBar(value);
            });
            await InitializeServices();
            await OnInitialize();
            if (_loadNextSceneAutomatically)
            {
                await LoadNextScene();
            }
        }
        public async Task LoadNextScene()
        {
            await ShowFakeBeginProgress();
            await StartInitialization();
            UnloadOtherScenes();
        }
        private async Task ShowFakeBeginProgress()
        {
            _splashScreen = await CreateSplashScreen();
            if (_splashScreen == null)
            {
                Debug.LogWarning("SplashScreen is null, skipping fake begin progress");
                return;
            }
            var from = 0f;
            for (int i = 0; i < _fakeBeginDelayProgressAndMessages.Length; i++)
            {
                var item = _fakeBeginDelayProgressAndMessages[i];
                await AnimateProgressAsync(from, item.progress, item.delayMilliseconds, item.message);
                from = item.progress;

                if (i == _fakeBeginDelayProgressAndMessages.Length - 1)
                {
                    _ = HideSplashScreenAsync();
                    await SceneManager.LoadSceneAsync(_nextSceneName, _loadSceneMode);
                }
            }
        }

        private async Task AnimateProgressAsync(float from, float to, int durationMilliseconds, string message)
        {
            _splashScreen.UpdateProgressBar(from, message);

            if (durationMilliseconds <= 0)
            {
                _splashScreen.UpdateProgressBar(to);
                return;
            }

            var value = from;
            Tween tween = DOTween
                .To(() => value, v =>
                {
                    value = v;
                    _splashScreen.UpdateProgressBar(v);
                }, to, durationMilliseconds / 1000f)
                .SetEase(Ease.Linear)
                .SetUpdate(true);

            await UniTask.WaitUntil(() => tween == null || !tween.IsActive());
        }
        private async Task InitializeServices()
        {
            if (_initFPSConfig != FPSConfig.UseUnityDefault)
            {
                Application.targetFrameRate = (int)_initFPSConfig;
            }
            if (_initUnityService)
            {
                await UnityServiceSP.InitializeServices();
            }
        }
        private void UnloadOtherScenes()
        {
            foreach (var sceneName in _unloadSceneNamesAfterLoad)
            {
                if (SceneManager.GetSceneByName(sceneName).isLoaded)
                {
                    SceneManager.UnloadSceneAsync(sceneName);
                }
            }
        }
        public virtual async Task OnInitialize()
        {
        }


        public virtual async Task<ISplashScreen> CreateSplashScreen()
        {
            return null;
        }

        /// <summary>
        /// Fades the splash out and then takes it off the canvas for good.
        ///
        /// <see cref="_splashScreen"/> is deliberately not cleared: it is an interface
        /// reference, so it does not become Unity's fake null when the object behind it
        /// is destroyed, and the implementation already no-ops once that happens.
        /// Clearing it would turn every later progress report into a null dereference.
        /// </summary>
        private async Task HideSplashScreenAsync()
        {
            if (_splashScreen == null)
            {
                return;
            }

            await _splashScreen.FadeOutAsync(_splashFadeOutSeconds);
            await DestroySplashScreen();
        }

        /// <summary>
        /// Removes the splash the same way <see cref="CreateSplashScreen"/> put it up.
        /// The base does nothing, because the base does not know what created it.
        /// </summary>
        public virtual async Task DestroySplashScreen()
        {
        }

        public async Task StartInitialization()
        {
            List<Func<IProgress<float>, Task>> initMethods = new List<Func<IProgress<float>, Task>>()
            {
                // Init Func Go Here
                progress => InititilizeSceneEntry(progress)
            };
            //
            Debug.Log("InitializeAll");
            await InitializeAll(initMethods, Progress);
            Debug.Log("InitializeAllFinished");
            //
        }
        public async Task InitializeAll(List<Func<IProgress<float>, Task>> initMethods, IProgress<float> progress)
        {
            int totalMethods = initMethods.Count;
            int completedMethods = 0;

            Debug.Log("InitializeAll method count: " + totalMethods);
            foreach (var initMethod in initMethods)
            {
                await initMethod(progress);
                completedMethods++;
                progress.Report((float)completedMethods / totalMethods);
            }
        }
        /// <summary>
        /// The scene contain this _nextEntryPointName should be init first
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>

        public async Task InititilizeSceneEntry(IProgress<float> progress)
        {
            var entryPoint = await FindSceneEntryPointInScene(_nextSceneName);
            await entryPoint.Init(progress);
        }
        public async Task<ISceneEntryPoint> FindSceneEntryPointInScene(string sceneName)
        {
            // Ensure the scene is loaded
            if (!SceneManager.GetSceneByName(sceneName).isLoaded)
                await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            var scene = SceneManager.GetSceneByName(sceneName);
            foreach (var rootObj in scene.GetRootGameObjects())
            {
                var entry = rootObj.GetComponentInChildren<ISceneEntryPoint>(true);
                if (entry != null)
                    return entry;
            }
            return null;
        }
        [Serializable]
        public struct DelayProgressAndMessage
        {
            public float progress;
            public string message;
            public int delayMilliseconds;
        }

    }
    public interface ISplashScreen
    {
        void UpdateProgressBar(float progress, string message = null);

        /// <summary>
        /// Fades the splash to fully transparent. Awaited before it is destroyed, so
        /// the scene coming up underneath is revealed rather than cut to.
        /// </summary>
        Task FadeOutAsync(float duration);
    }
}
