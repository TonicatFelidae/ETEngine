using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityScreenNavigator.Runtime.Core.Shared;
using UnityScreenNavigator.Runtime.Foundation;
using ET.ResourceManagement;
using UnityScreenNavigator.Runtime.Core.Page;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;
using VContainer;

namespace UnityScreenNavigator.Runtime.Core.Sheet
{
    [RequireComponent(typeof(RectMask2D))]
    public class SheetContainer : MonoBehaviour, IScreenContainer
    {
        PageFactory _prefabFactory;

        [Inject]
        public void Init(
            PageFactory pageFactory)
        {
            _prefabFactory = pageFactory;
            Debug.Log("Resolve SheetContainer");
        }

        private static readonly Dictionary<int, SheetContainer> InstanceCacheByTransform =
            new Dictionary<int, SheetContainer>();

        private static readonly Dictionary<string, SheetContainer> InstanceCacheByName =
            new Dictionary<string, SheetContainer>();

        [SerializeField] private string _name;

        [SerializeField]
        [Tooltip("Destroy and release a sheet as soon as a different one is shown. Leave off to " +
                 "keep every sheet the player has opened resident (the historical behaviour).")]
        private bool _unloadInactiveSheets;

        private readonly Dictionary<string, AssetLoadHandle<GameObject>> _assetLoadHandles
            = new Dictionary<string, AssetLoadHandle<GameObject>>();

        private readonly List<ISheetContainerCallbackReceiver> _callbackReceivers =
            new List<ISheetContainerCallbackReceiver>();

        private readonly Dictionary<string, string> _sheetNameToId = new Dictionary<string, string>();
        private readonly Dictionary<string, Sheet> _sheets = new Dictionary<string, Sheet>();

        private IAssetLoader _assetLoader;
        private CanvasGroup _canvasGroup;
        public static List<SheetContainer> Instances { get; } = new List<SheetContainer>();

        private ScreenContainerTransitionHandler _transitionHandler;
        private SheetLifecycleHandler _lifecycleHandler;

        /// <summary>
        ///     By default, <see cref="IAssetLoader" /> in <see cref="UnityScreenNavigatorSettings" /> is used.
        ///     If this property is set, it is used instead.
        /// </summary>
        public IAssetLoader AssetLoader
        {
            get => _assetLoader ?? UnityScreenNavigatorSettings.Instance.AssetLoader;
            set => _assetLoader = value;
        }

        public string ActiveSheetId { get; private set; }

        public Sheet ActiveSheet
        {
            get
            {
                if (ActiveSheetId == null)
                    return null;

                return _sheets[ActiveSheetId];
            }
        }

        /// <summary>
        ///     True if in transition.
        /// </summary>
        public bool IsInTransition => _transitionHandler.IsInTransition;

        /// <summary>
        ///     Registered sheets.
        /// </summary>
        public IReadOnlyDictionary<string, Sheet> Sheets => _sheets;

        /// <summary>
        ///     When true, showing a sheet destroys the previously active one and releases its
        ///     asset handle, so its bundle can leave memory. Off by default because every
        ///     existing caller assumed registration was permanent.
        /// </summary>
        public bool UnloadInactiveSheets
        {
            get => _unloadInactiveSheets;
            set => _unloadInactiveSheets = value;
        }

        public bool Interactable
        {
            get => _canvasGroup.interactable;
            set => _canvasGroup.interactable = value;
        }

        private void Awake()
        {
            Instances.Add(this);

            _callbackReceivers.AddRange(GetComponents<ISheetContainerCallbackReceiver>());

            if (!string.IsNullOrWhiteSpace(_name)) InstanceCacheByName.Add(_name, this);
            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            _transitionHandler = new ScreenContainerTransitionHandler(this);
            _lifecycleHandler = new SheetLifecycleHandler((RectTransform)transform, _callbackReceivers);
        }

        private void OnDestroy()
        {
            UnregisterAll();

            InstanceCacheByName.Remove(_name);
            var keysToRemove = new List<int>();
            foreach (var cache in InstanceCacheByTransform)
                if (Equals(cache.Value))
                    keysToRemove.Add(cache.Key);

            foreach (var keyToRemove in keysToRemove) InstanceCacheByTransform.Remove(keyToRemove);

            Instances.Remove(this);
        }

        /// <summary>
        ///     Get the <see cref="SheetContainer" /> that manages the sheet to which <see cref="transform" /> belongs.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="useCache">Use the previous result for the <see cref="transform" />.</param>
        /// <returns></returns>
        public static SheetContainer Of(Transform transform, bool useCache = true)
        {
            return Of((RectTransform)transform, useCache);
        }

        /// <summary>
        ///     Get the <see cref="SheetContainer" /> that manages the sheet to which <see cref="rectTransform" /> belongs.
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="useCache">Use the previous result for the <see cref="rectTransform" />.</param>
        /// <returns></returns>
        public static SheetContainer Of(RectTransform rectTransform, bool useCache = true)
        {
            var hashCode = rectTransform.GetInstanceID();

            if (useCache && InstanceCacheByTransform.TryGetValue(hashCode, out var container)) return container;

            container = rectTransform.GetComponentInParent<SheetContainer>();
            if (container != null)
            {
                InstanceCacheByTransform.Add(hashCode, container);
                return container;
            }

            return null;
        }

        /// <summary>
        ///     Find the <see cref="SheetContainer" /> of <see cref="containerName" />.
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public static SheetContainer Find(string containerName)
        {
            if (InstanceCacheByName.TryGetValue(containerName, out var instance)) return instance;

            return null;
        }

        /// <summary>
        ///     Add a callback receiver.
        /// </summary>
        /// <param name="callbackReceiver"></param>
        public void AddCallbackReceiver(ISheetContainerCallbackReceiver callbackReceiver)
        {
            _callbackReceivers.Add(callbackReceiver);
        }

        /// <summary>
        ///     Remove a callback receiver.
        /// </summary>
        /// <param name="callbackReceiver"></param>
        public void RemoveCallbackReceiver(ISheetContainerCallbackReceiver callbackReceiver)
        {
            _callbackReceivers.Remove(callbackReceiver);
        }

        /// <summary>
        ///     Show a sheet.
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <param name="playAnimation"></param>
        /// <returns></returns>
        public AsyncProcessHandle ShowByResourceKey(string resourceKey, bool playAnimation)
        {
            return CoroutineManager.Instance.Run(ShowByResourceKeyRoutine(resourceKey, playAnimation));
        }

        /// <summary>
        ///     Show a sheet.
        /// </summary>
        /// <param name="sheetId"></param>
        /// <param name="playAnimation"></param>
        /// <returns></returns>
        public AsyncProcessHandle Show(string sheetId, bool playAnimation)
        {
            return CoroutineManager.Instance.Run(ShowRoutine(sheetId, playAnimation));
        }

        /// <summary>
        ///     Hide a sheet.
        /// </summary>
        /// <param name="playAnimation"></param>
        public AsyncProcessHandle Hide(bool playAnimation)
        {
            return CoroutineManager.Instance.Run(HideRoutine(playAnimation));
        }

        /// <summary>
        ///     Register a sheet.
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <param name="onLoad"></param>
        /// <param name="loadAsync"></param>
        /// <param name="sheetId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public AsyncProcessHandle Register(string resourceKey,
            Action<(string sheetId, Sheet sheet)> onLoad = null, bool loadAsync = true, string sheetId = null)
        {
            return CoroutineManager.Instance.Run(
                RegisterRoutine(typeof(Sheet), resourceKey, onLoad, loadAsync, sheetId));
        }

        /// <summary>
        ///     Register a sheet.
        /// </summary>
        /// <param name="sheetType"></param>
        /// <param name="resourceKey"></param>
        /// <param name="onLoad"></param>
        /// <param name="loadAsync"></param>
        /// <param name="sheetId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public AsyncProcessHandle Register(Type sheetType, string resourceKey,
            Action<(string sheetId, Sheet sheet)> onLoad = null, bool loadAsync = true, string sheetId = null)
        {
            return CoroutineManager.Instance.Run(RegisterRoutine(sheetType, resourceKey, onLoad, loadAsync, sheetId));
        }

        /// <summary>
        ///     Register a sheet.
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <param name="onLoad"></param>
        /// <param name="loadAsync"></param>
        /// <param name="sheetId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public AsyncProcessHandle Register<TSheet>(string resourceKey,
            Action<(string sheetId, TSheet sheet)> onLoad = null, bool loadAsync = true, string sheetId = null)
            where TSheet : Sheet
        {
            return CoroutineManager.Instance.Run(RegisterRoutine(typeof(TSheet), resourceKey,
                x => onLoad?.Invoke((x.sheetId, (TSheet)x.sheet)), loadAsync, sheetId));
        }

        private IEnumerator RegisterRoutine(Type sheetType, string resourceKey,
            Action<(string sheetId, Sheet sheet)> onLoad = null, bool loadAsync = true, string sheetId = null)
        {
            Assert.IsNotNull(resourceKey);

            sheetId ??= Guid.NewGuid().ToString();

            // Sheets are now registered lazily, on the first touch of their tab, so the same
            // id legitimately arrives here more than once. This used to throw out of
            // _sheets.Add; a second registration has to be a no-op instead.
            if (_sheets.ContainsKey(sheetId))
            {
                yield return sheetId;
                yield break;
            }

            Sheet sheet = null;
            yield return LoadSheet(sheetType,
                resourceKey,
                loadAsync,
                (s, lh) =>
                {
                    sheet = s;
                    _sheets.Add(sheetId, sheet);
                    _sheetNameToId[resourceKey] = sheetId;
                    _assetLoadHandles.Add(sheetId, lh);
                    onLoad?.Invoke((sheetId, s));
                });

            var context = new SheetRegisterContext(sheetId, sheet);

            yield return _lifecycleHandler.AfterLoad(context);

            yield return context.SheetId;
        }

        private IEnumerator ShowByResourceKeyRoutine(string resourceKey, bool playAnimation)
        {
            var sheetId = _sheetNameToId[resourceKey];
            yield return ShowRoutine(sheetId, playAnimation);
        }

        private IEnumerator ShowRoutine(string sheetId, bool playAnimation)
        {
            Assert.IsFalse(IsInTransition,
                "Cannot transition because the screen is already in transition.");
            Assert.IsFalse(ActiveSheetId != null && ActiveSheetId == sheetId,
                "Cannot transition because the sheet is already active.");

            var previousSheetId = ActiveSheetId;

            var context = SheetShowContext.Create(sheetId, ActiveSheetId, _sheets);

            _transitionHandler.Begin();

            yield return _lifecycleHandler.BeforeShow(context);

            yield return _lifecycleHandler.Show(context, playAnimation);

            _lifecycleHandler.AfterShow(context);

            ActiveSheetId = sheetId;

            _transitionHandler.End();

            // Only now, after the outgoing sheet has finished animating away, is it safe to
            // drop it: the transition needed both sheets alive. Skipping this is what kept
            // every tab the player ever opened - and its whole bundle - resident for the
            // rest of the session.
            if (_unloadInactiveSheets && previousSheetId != null && previousSheetId != sheetId)
                Deregister(previousSheetId);
        }

        private IEnumerator HideRoutine(bool playAnimation)
        {
            Assert.IsFalse(IsInTransition,
                "Cannot transition because the screen is already in transition.");
            Assert.IsNotNull(ActiveSheetId,
                "Cannot transition because there is no active sheets.");

            _transitionHandler.Begin();

            var context = SheetHideContext.Create(_sheets[ActiveSheetId]);

            yield return _lifecycleHandler.BeforeHide(context);

            yield return _lifecycleHandler.Hide(context, playAnimation);

            _lifecycleHandler.AfterHide(context);

            ActiveSheetId = null;

            _transitionHandler.End();
        }

        /// <summary>
        ///     True when <paramref name="sheetId" /> is already registered, and therefore
        ///     already instantiated and holding an asset handle.
        /// </summary>
        public bool IsRegistered(string sheetId)
        {
            return sheetId != null && _sheets.ContainsKey(sheetId);
        }

        /// <summary>
        ///     Destroy one registered sheet and release the asset handle behind it. Pairs with
        ///     <see cref="Register(string,Action{ValueTuple{string,Sheet}},bool,string)" />;
        ///     releasing the handle is what lets Addressables unload the sheet's bundle once
        ///     nothing else references it.
        ///     Refuses to drop the active sheet - that would leave the container showing a
        ///     destroyed object.
        /// </summary>
        public void Deregister(string sheetId)
        {
            if (sheetId == null || !_sheets.TryGetValue(sheetId, out var sheet))
                return;

            if (ActiveSheetId == sheetId)
            {
                Debug.LogWarning(
                    $"[SheetContainer] Refused to deregister '{sheetId}' because it is the active sheet.");
                return;
            }

            if (UnityScreenNavigatorSettings.Instance.CallCleanupWhenDestroy)
                sheet.BeforeReleaseAndForget();

            if (sheet != null)
                Destroy(sheet.gameObject);

            _sheets.Remove(sheetId);

            // _sheetNameToId is keyed by resource key, so the entry has to be found by value.
            string resourceKey = null;
            foreach (var pair in _sheetNameToId)
                if (pair.Value == sheetId)
                {
                    resourceKey = pair.Key;
                    break;
                }

            if (resourceKey != null)
                _sheetNameToId.Remove(resourceKey);

            if (_assetLoadHandles.TryGetValue(sheetId, out var assetLoadHandle))
            {
                AssetLoader.Release(assetLoadHandle);
                _assetLoadHandles.Remove(sheetId);
            }
        }

        /// <summary>
        ///     Destroy and release all sheets.
        /// </summary>
        public void UnregisterAll()
        {
            foreach (var sheet in _sheets.Values)
            {
                if (UnityScreenNavigatorSettings.Instance.CallCleanupWhenDestroy)
                    sheet.BeforeReleaseAndForget();
                Destroy(sheet.gameObject);
            }

            foreach (var assetLoadHandle in _assetLoadHandles.Values)
                AssetLoader.Release(assetLoadHandle);

            _assetLoadHandles.Clear();
        }

        private IEnumerator LoadSheet(
            Type sheetType,
            string resourceKey,
            bool loadAsync,
            Action<Sheet, AssetLoadHandle<GameObject>> onLoaded
        )
        {
            var assetLoadHandle = loadAsync
                ? AssetLoader.LoadAsync<GameObject>(resourceKey)
                : AssetLoader.Load<GameObject>(resourceKey);

            if (!assetLoadHandle.IsDone)
                yield return new WaitUntil(() => assetLoadHandle.IsDone);

            if (assetLoadHandle.Status == AssetLoadStatus.Failed)
                throw assetLoadHandle.OperationException;

            var instance = _prefabFactory.Create<Sheet>(assetLoadHandle.Result);

            onLoaded?.Invoke(instance, assetLoadHandle);
        }
    }
}