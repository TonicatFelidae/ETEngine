using UnityEngine;
using System;

namespace ETEngine.Service.UserSystem
{
    public static class AppUUID
    {
        private const string UUID_KEY = "app_uuid";

        public static bool Exists()
        {
            return PlayerPrefs.HasKey(UUID_KEY);
        }

        public static string Get()
        {
            if (PlayerPrefs.HasKey(UUID_KEY))
            {
                return PlayerPrefs.GetString(UUID_KEY);
            }
            else
            {
                string uuid = Guid.NewGuid().ToString();
                PlayerPrefs.SetString(UUID_KEY, uuid);
                PlayerPrefs.Save();
                return uuid;
            }
        }
    }
}
