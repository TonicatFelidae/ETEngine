using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ETEngine.Utils
{
    /// <summary>
    /// Downloads a <see cref="Sprite"/> from a URL with an in-memory cache and
    /// retry/backoff. Shared by FanGameItem, GioShopItemSlot and the offerwall
    /// buttons — these used to each carry their own copy of this logic.
    /// </summary>
    public static class RemoteSpriteLoader
    {
        private static readonly Dictionary<string, Sprite> _cache = new();

        /// <summary>Clears the in-memory sprite cache (debug/testing — forces re-download).</summary>
        public static void Clear() => _cache.Clear();

        /// <summary>
        /// Returns the loaded sprite, or <c>null</c> on an empty URL or after all
        /// retries fail. Results are cached per URL for the app session.
        /// </summary>
        public static async UniTask<Sprite> LoadAsync(string url, int maxRetries = 3)
        {
            if (string.IsNullOrEmpty(url)) return null;
            if (_cache.TryGetValue(url, out var cached)) return cached;

            float delay = 1f;
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                using var request = UnityWebRequestTexture.GetTexture(url);
                var op = request.SendWebRequest();
                // WaitUntil (core UniTask) rather than awaiting the operation
                // directly, so this doesn't depend on the optional UnityWebRequest
                // awaiter extension being referenced by the asmdef.
                await UniTask.WaitUntil(() => op.isDone);

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var texture = DownloadHandlerTexture.GetContent(request);
                    var sprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f));
                    _cache[url] = sprite;
                    return sprite;
                }

                Debug.LogWarning($"[RemoteSpriteLoader] load failed {url} (attempt {attempt}/{maxRetries}): {request.error}");
                if (attempt < maxRetries)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(delay));
                    delay *= 2f;
                }
            }

            Debug.LogError($"[RemoteSpriteLoader] load failed {url} after {maxRetries} attempts");
            return null;
        }
    }
}
