using UnityEngine;
namespace ETSimpleKit
{
    /// <summary>
    /// PLEASE build in mind of player pref interactable
    /// </summary>
    public class ETGameSystemSetting
    {
        public void ToggleVibration(string key)
        {
            if (PlayerPrefs.GetInt(key, 0) == 0)
            {
                PlayerPrefs.SetInt(key, 1);
            }
            else
            {
                PlayerPrefs.SetInt(key, 0);
            }
        }
    }
}