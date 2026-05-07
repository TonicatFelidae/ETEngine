using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
                _splashScreen.UpdateProgressBar(value);
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
            for (int i = 0; i < _fakeBeginDelayProgressAndMessages.Length; i++)
            {
                var item = _fakeBeginDelayProgressAndMessages[i];
                if (i == _fakeBeginDelayProgressAndMessages.Length - 1)
                {
                    await SceneManager.LoadSceneAsync(_nextSceneName, _loadSceneMode);

                }
                _splashScreen.UpdateProgressBar(item.progress, item.message);
                await Task.Delay(item.delayMilliseconds);
            }
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
    }
}
