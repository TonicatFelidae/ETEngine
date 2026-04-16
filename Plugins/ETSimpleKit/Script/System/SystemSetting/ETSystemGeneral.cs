using ET;
using ET.ETPlayerPref;
using UnityEngine;
using UnityEngine.Events;

namespace ETSimpleKit
{
    public class ETSystemGeneral : Singleton<ETSystemGeneral>
    {
        const string INDEX_VIBRATION = "Vibration";
        const string INDEX_NIGHTMODE = "NightMode";

        [HideInInspector] public bool isVibration;
        [HideInInspector] public bool isNightMode;

        [HideInInspector] public UnityEvent<bool> onVibrationSettingChange = new();
        [HideInInspector] public UnityEvent<bool> onNightModeSetting = new();

        public void Awake()
        {
            OnToggleVibration();
            OnToggleNightMode();
        }
        public void InitPlayerPrefData()
        {
            ETPlayerPrefManager eTPlayerPrefManager = FindAnyObjectByType<ETPlayerPrefManager>();
            if (eTPlayerPrefManager.floatKeys == null) eTPlayerPrefManager.floatKeys = new();
            if (eTPlayerPrefManager.intKeys == null) eTPlayerPrefManager.intKeys = new();
            if (eTPlayerPrefManager.stringKeys == null) eTPlayerPrefManager.stringKeys = new();
            // Default vibration is now controlled by Remote Config key "AB_vibration_cat".
            // Do not preseed PlayerPrefs["Vibration"] here so first-run default can be
            // applied right after Remote Config fetch (see GeneralObject.InitializeUnityServicesAsync).
            // If needed, Settings UI or ETSystemGeneral.OnToggleVibration() will write the value.
            // eTPlayerPrefManager.intKeys.Add(new PlayerPrefInt(INDEX_VIBRATION, 0));
            eTPlayerPrefManager.intKeys.Add(new PlayerPrefInt(INDEX_NIGHTMODE, 0));
        }

        #region SP
        public void OnToggleVibration()
        {
            isVibration = PlayerPrefs.GetInt(INDEX_VIBRATION, 0) == 1;
            onVibrationSettingChange.Invoke(isVibration);
        }
        public void OnToggleNightMode()
        {
            isNightMode = PlayerPrefs.GetInt(INDEX_NIGHTMODE, 0) == 1;
            onNightModeSetting.Invoke(isNightMode);
        }
        #endregion

    }
}
