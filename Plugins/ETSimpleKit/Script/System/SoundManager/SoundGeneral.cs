using ET;
using ET.ETPlayerPref;
using ET.SupportKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
namespace ETSimpleKit.SoundSystem
{
    /// <summary>
    /// ET Solo general class: Sound_general
    /// Purpose:
    /// - Play BG sound
    /// - Change pintch, Volume
    /// - Play Button sound
    /// - Play eeffect sound
    /// How to use
    /// - Put this on a game object
    /// Update 3/2025: Toggle volumn channel
    /// Update 6/2025: Toggle volumn global
    /// </summary>
    public class SoundGeneral : Singleton<SoundGeneral>
    {
        public AudioSource _soAB;
        public AudioSource _soBG;
        public AudioSource _soUI;
        public AudioSource _soEF;
        public AudioMixer _audioMixer;
        public List<AudioClip> amberClips;
        public List<AudioClip> backgroundClips;
        public List<AudioClip> effectClips;
        public List<AudioClip> buttonClips;
        public List<AudioClip> UIClips;
        public List<AudioClip> catTouchClips;
        private int _lastCatTouchIndex = -1;
        private Dictionary<string, AudioClip> soundsDict;
        [Header("OPTIONS")]
        public float amberMultiply = 1;
        private ETPoolManager _eTPoolManager = new();
        //sound
        [SerializeField] private SoundObject3D _pp_soundObject3D;
        private Transform _sound3DContainer;
        private Transform Sound3DContainer
        {
            get
            {
                if (_sound3DContainer == null)
                {
                    _sound3DContainer = (new GameObject()).transform;
                    _sound3DContainer.parent = transform;
                }

                return _sound3DContainer;
            }
        }

        public float PitchBG
        {
            get => _soBG.pitch;
            set
            {
                _soBG.pitch = value;
            }
        }
        CoroutineUnit _supressVolumeBG = new();
        CoroutineUnit _lerpPitchBG = new();


        const string INDEX_BG = "BG";
        const string INDEX_EF = "EF";
        const string INDEX_UI = "UI";
        const string INDEX_MA = "MA";
        const string TOGGLE_BG = "ToggleBG";
        const string TOGGLE_EF = "ToggleEF";
        const string TOGGLE_UI = "ToggleUI";
        const string TOGGLE_MA = "ToggleMA";
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
        private void Start()
        {
           InitiationCheck();
           LoadSnapshot("Normal");
        }
        #region InitiationCheck
        private void InitiationCheck()
        {
            ToggleCheck();
            void ToggleCheck()
            {
                BG.InitiationChannelCheck(TOGGLE_BG);
                EF.InitiationChannelCheck(TOGGLE_EF);
                UI.InitiationChannelCheck(TOGGLE_UI);
            }
        }
        #endregion
        /// <summary>
        /// For use with odata loader
        /// </summary>
        /// <param name="soundsDict"></param>
        public void BindSoundDict(Dictionary<string, AudioClip> soundsDict)
        {
            this.soundsDict = soundsDict;
        }
        /// <summary>
        /// Play Effect3D at mainCamera
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pitch"></param>
        public void PlayEF3D(int index, float pitch = 1, bool allowOverlap = false) => PlayEF3D(index, Camera.main.transform.position, pitch, allowOverlap);
        public void PlayEF3D(string id, float pitch = 1) => PlayEF3D(id, Camera.main.transform.position, pitch);
        /// <summary>
        /// Play effect in 3D so player can hear it loud or soft depend on distance
        /// Feature with pool system
        /// </summary>
        /// <param name="index"></param>
        /// <param name="loc"></param>
        /// <param name="pitch"></param>
        public void PlayEF3D(int index, Vector3 loc, float pitch = 1, bool allowOverlap = false)
        {
            if (allowOverlap)
            {
                // Bypass pool system for overlapping sounds - use PlayOneShot directly
                _soEF.PlayOneShot(effectClips[index], EFVolume);
                return;
            }
            SoundObject3D go = _eTPoolManager.GetObjectFromPool<SoundObject3D>(index.ToString(), Sound3DContainer, _pp_soundObject3D);
            go.Init(this);
            go.Play(loc, effectClips[index], EFVolume, pitch);
        }
        public void PlayEF3D(string index, Vector3 loc, float pitch = 1)
        {
            SoundObject3D go = _eTPoolManager.GetObjectFromPool<SoundObject3D>(index.ToString(), Sound3DContainer, _pp_soundObject3D);
            go.Init(this);
            go.Play(loc, soundsDict[index], EFVolume, pitch);
        }
        //pitch
        public void SetPitch(SoundType soundType, float pitch)
        {
            switch (soundType)
            {
                case SoundType.BG:
                    _soBG.pitch = pitch;
                    break;
                case SoundType.EF:
                    _soEF.pitch = pitch;
                    break;
                case SoundType.UI:
                    _soUI.pitch = pitch;
                    break;
                case SoundType.MA:
                    _soBG.pitch = pitch;
                    _soEF.pitch = pitch;
                    _soUI.pitch = pitch;
                    break;
                default:
                    break;
            }
        }
        #region VolumeEffect
        // currently might have issue that sound Volume not reset itself
        public void SupressVolumeBG(float targetVolumeMod, float supressSpeed, float keepSupressIn, float recoverSpeed)
        {
            _supressVolumeBG.StartCoroutine(this, SupressVolumeBGIE(targetVolumeMod, supressSpeed, keepSupressIn, recoverSpeed));
        }
        IEnumerator SupressVolumeBGIE(float targetVolumeMod, float supressSpeed, float keepSupressIn, float recoverSpeed)
        {
            float oriValue = BG.VolumeMod;
            while (BG.VolumeMod > targetVolumeMod)
            {
                BG.VolumeMod = ETMath.LerpToFloat(BG.VolumeMod, targetVolumeMod, supressSpeed * Time.deltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(keepSupressIn);
            while (BG.VolumeMod < 1)
            {
                BG.VolumeMod = ETMath.LerpToFloat(BG.VolumeMod, 1, recoverSpeed * Time.deltaTime);
                yield return null;
            }
        }
        #endregion

        public void LerpPitchBG(float targetPitch, float speed = 1)
        {
            _lerpPitchBG.StartCoroutine(this, LerpPitchBGIE(targetPitch, speed));
        }
        IEnumerator LerpPitchBGIE(float targetPitch, float speed = 1)
        {
            while (PitchBG < targetPitch)
            {
                PitchBG = Mathf.Lerp(PitchBG, targetPitch, speed * Time.deltaTime);
                yield return null;
            }
            yield return null;
        }
        #region AB
        public void StopAB() => _soAB.Stop();
        public void StopBG() => _soBG.Stop();
        public void PlayAB0() => PlayAB(0);
        public void PlayAB1() => PlayAB(1);
        public void PlayAB2() => PlayAB(2);
        public void PlayAB3() => PlayAB(3);
        public void PlayAB4() => PlayAB(4);
        public void PlayAB(int index)
        {
            _soAB.clip = amberClips[index];
            _soAB.volume = BGVolume * amberMultiply;
            _soAB.Play();
        }
        #endregion
        #region BG
        public void PlayBG0() => PlayBG(0);
        public void PlayBG1() => PlayBG(1);
        public void PlayBG2() => PlayBG(2);
        public void PlayBG3() => PlayBG(3);
        public void PlayBG4() => PlayBG(4);
        public void PlayBG(int index)
        {
            _soBG.clip = backgroundClips[index];
            _soBG.volume = BGVolume;
            _soBG.Play();
        }
        public void PlayBG(int fromIncluded, int toExcluded, float pitch = 1)
        {
            _soBG.clip = GetClip(backgroundClips, UnityEngine.Random.Range(fromIncluded, toExcluded));
            _soBG.volume = BGVolume;
            _soBG.pitch = pitch;
            _soBG.Play();
        }
        #endregion
        #region EF
        public void PlayEF0() => PlayEF(0);
        public void PlayEF1() => PlayEF(1);
        public void PlayEF2() => PlayEF(2);
        public void PlayEF3() => PlayEF(3);
        public void PlayEF4() => PlayEF(4); 
        public void PlayEF(int fromIncluded, int toExcluded, float pitch = 1)
        {
            _soEF.clip = GetClip(effectClips, UnityEngine.Random.Range(fromIncluded, toExcluded));
            _soEF.volume = EFVolume;
            _soEF.pitch = pitch;
            _soEF.Play();
        }
        public void PlayEF(int index)
        {
            _soEF.clip = effectClips[index];
            _soEF.volume = EFVolume;
            _soEF.Play();
        }
        #endregion
        #region UI
        public void PlayUI(int index)
        {
            _soUI.clip = UIClips[index];
            _soUI.volume = UIVolume;
            _soUI.Play();
        }
        //but
        public void PlayUIBut0() => PlayUIBut(0);
        public void PlayUIBut1() => PlayUIBut(1);
        public void PlayUIBut2() => PlayUIBut(2);
        public void PlayUIBut3() => PlayUIBut(3);
        public void PlayUIBut4() => PlayUIBut(4);
        public void PlayUIBut(int index)
        {
            _soUI.clip = buttonClips[index];
            _soUI.volume = UIVolume;
            _soUI.Play();
        }
        #endregion
        #region Cat Touch
        public void PlayCatTouch(int index = -1)
        {
            if (index >= 0)
            {
                // Play specific cat touch sound by index
                if (catTouchClips != null && index < catTouchClips.Count)
                {
                    _lastCatTouchIndex = index;
                    // Use PlayOneShot to allow overlapping sounds
                    _soUI.PlayOneShot(catTouchClips[index], UIVolume);
                }
                else
                {
                    Debug.LogError($"PlayCatTouch: Index {index} out of range for catTouchClips (count: {catTouchClips?.Count ?? 0})");
                }
            }
            else
            {
                // Play random cat touch sound
                if (catTouchClips != null && catTouchClips.Count > 0)
                {
                    int randomIndex;
                    if (catTouchClips.Count == 1)
                    {
                        // Only one clip, play it
                        randomIndex = 0;
                    }
                    else
                    {
                        // Multiple clips, avoid playing the same one twice in a row
                        do
                        {
                            randomIndex = Random.Range(0, catTouchClips.Count);
                        } while (randomIndex == _lastCatTouchIndex);
                    }

                    _lastCatTouchIndex = randomIndex;
                    // Use PlayOneShot to allow overlapping sounds
                    _soUI.PlayOneShot(catTouchClips[randomIndex], UIVolume);
                }
                else
                {
                    // Fall back to the default cat touch button sound (index 7)
                    PlayUIBut(7);
                }
            }
        }
        #endregion

        //sp
        AudioClip GetRandomClip(List<AudioClip> list)
        {
            int ran = Random.Range(0, list.Count);
            AudioClip au = list[ran];
            return au;
        }
        #region Sound Channel BG
        SoundChannel _bg;
        public SoundChannel BG
        {
            get
            {
                if (_bg == null)
                {
                    _bg = new SoundChannel("BG", OnBGVolumeChange);
                }
                return _bg;
            }
        }
        public void OnBGVolumeChange()
        {
            _soBG.volume = BG.Volume;
            _soAB.volume = BG.Volume * amberMultiply;
        }
        public float BGVolume => BG.Volume;
        #endregion
        #region Sound Channel UI
        SoundChannel _ui;
        public SoundChannel UI
        {
            get
            {
                if (_ui == null)
                {
                    _ui = new SoundChannel("UI", OnUIVolumeChange);
                }
                return _ui;
            }
        }
        public void OnUIVolumeChange()
        {
            _soUI.volume = UI.Volume;
        }
        public float UIVolume => UI.Volume;
        #endregion
        #region Sound Channel EF
        SoundChannel _ef;
        public SoundChannel EF
        {
            get
            {
                if (_ef == null)
                {
                    _ef = new SoundChannel("EF", OnEFVolumeChange);
                }
                return _ef;
            }
        }
        public void OnEFVolumeChange()
        {
            _soEF.volume = EF.Volume;
        }
        public float EFVolume => EF.Volume;
        #endregion
        //function
        public void InitPlayerPrefData()
        {
            ETPlayerPrefManager eTPlayerPrefManager = FindAnyObjectByType<ETPlayerPrefManager>();
            if (eTPlayerPrefManager.floatKeys == null) eTPlayerPrefManager.floatKeys = new();
            if (eTPlayerPrefManager.intKeys == null) eTPlayerPrefManager.intKeys = new();
            eTPlayerPrefManager.floatKeys.Add(new PlayerPrefFloat(INDEX_BG, 1));
            eTPlayerPrefManager.floatKeys.Add(new PlayerPrefFloat(INDEX_EF, 1));
            eTPlayerPrefManager.floatKeys.Add(new PlayerPrefFloat(INDEX_UI, 1));
            eTPlayerPrefManager.floatKeys.Add(new PlayerPrefFloat(INDEX_MA, 1));
            eTPlayerPrefManager.intKeys.Add(new PlayerPrefInt(TOGGLE_BG, 1));
            eTPlayerPrefManager.intKeys.Add(new PlayerPrefInt(TOGGLE_EF, 1));
            eTPlayerPrefManager.intKeys.Add(new PlayerPrefInt(TOGGLE_UI, 1));
            eTPlayerPrefManager.intKeys.Add(new PlayerPrefInt(TOGGLE_MA, 1));
        }
        public void UpdateVolume()
        {
            _soBG.volume = BGVolume;
            _soUI.volume = UIVolume;
            _soEF.volume = EFVolume;
            _soAB.volume = BGVolume * amberMultiply;
        }
        #region Mute sound 
        public void ToggleChannelVolume(string channelID) => GetChannel(channelID).ToggleVolume();
        #endregion
        #region SP
        public SoundChannel GetChannel(string channelID)
        {
            switch (channelID)
            {
                case ChannelID.BG:
                    return BG;
                case ChannelID.UI:
                    return UI;
                case ChannelID.EF:
                    return EF;
                default:
                    return BG;
            }
        }
        AudioClip GetClip(List<AudioClip> list, int index)
        {
            AudioClip ret = null;
            try
            {
                ret = list[index];
            }
            catch
            {
                Debug.LogError($"SoundGeneral GetClip index = {index} not in the list with count {list.Count}");
            }
            return ret;
        }
        #endregion
        private void RestartCoroutine(ref Coroutine coroutine, IEnumerator routine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(routine);
        }
        #region Master channel
        private const string MAKey = "MA";
        private float MA => PlayerPrefs.GetFloat(MAKey, 1);
        private float memorisedMAVolume;
        public void ToggleMasterVolume()
        {
            if (MA > 0)
            {
                memorisedMAVolume = MA;
                PlayerPrefs.SetFloat(MAKey, 0);
            }
            else
            {
                if (memorisedMAVolume == 0) memorisedMAVolume = 1;
                PlayerPrefs.SetFloat(MAKey, memorisedMAVolume);
            }
            UpdateVolume();
        }
        public void SetVolume(float value)
        {
            PlayerPrefs.SetFloat(MAKey, value);
            UpdateVolume();
        }
        #endregion
        #region AudioMixer Snapshots
        /// <summary>
        /// Load an AudioMixer snapshot immediately
        /// </summary>
        /// <param name="snapshotName">Name of the snapshot to load</param>
        public void LoadSnapshot(string snapshotName)
        {
            if (_audioMixer == null)
            {
                Debug.LogError("SoundGeneral: AudioMixer is not assigned!");
                return;
            }

            AudioMixerSnapshot snapshot = _audioMixer.FindSnapshot(snapshotName);
            if (snapshot == null)
            {
                Debug.LogError($"SoundGeneral: Snapshot '{snapshotName}' not found in AudioMixer!");
                return;
            }

            snapshot.TransitionTo(0);
        }

        /// <summary>
        /// Transition to an AudioMixer snapshot with a smooth duration
        /// </summary>
        /// <param name="snapshotName">Name of the snapshot to transition to</param>
        /// <param name="duration">Time in seconds for the transition</param>
        public void TransitionToSnapshot(string snapshotName, float duration = 0.5f)
        {
            if (_audioMixer == null)
            {
                Debug.LogError("SoundGeneral: AudioMixer is not assigned!");
                return;
            }

            AudioMixerSnapshot snapshot = _audioMixer.FindSnapshot(snapshotName);
            if (snapshot == null)
            {
                Debug.LogError($"SoundGeneral: Snapshot '{snapshotName}' not found in AudioMixer!");
                return;
            }

            snapshot.TransitionTo(duration);
        }
        #endregion
        public static class ChannelID
        {
            public const string BG = "BG";
            public const string EF = "EF";
            public const string UI = "UI";
        }

    }
    public class SoundChannel
    {
        private float SAVED_VOl => PlayerPrefs.GetFloat(_prefKey, 1);
        private float MA => PlayerPrefs.GetFloat("MA", 1);
        private string _prefKey = "";
        public float VolumeMod
        {
            get => _VolumeMod;
            set
            {
                _VolumeMod = value;
                _onVolumeChange.Invoke();
            }
        }
        private float _VolumeMod = 1;
        public float Volume => SAVED_VOl * MA * VolumeMod;
        private float _Volume;
        private float memorisedVolume = 1;
        private UnityAction _onVolumeChange;
        public SoundChannel(string prefKey, UnityAction onVolumeChange)
        {
            _prefKey = prefKey;
            _onVolumeChange = onVolumeChange;
        }
        // A is toggle key
        // B is volumn key
        public void InitiationChannelCheck(string toggleKey) // Do not confuse this with ToggleVolume,  this dont need to change volumn key because it saved, this is for toggle rout B set A
        {
            if (SAVED_VOl > 0)
            {
                memorisedVolume = SAVED_VOl;
                PlayerPrefs.SetInt(toggleKey, 1);
            }
            else
            {
                if (memorisedVolume == 0) memorisedVolume = 1;
                PlayerPrefs.SetInt(toggleKey, 0);
            }
        }
        public void ResetMod()
        {
            _VolumeMod = 1;
        }
        public void ToggleVolume() // this dont need to change toggle key because it change outside, this A set B
        {
            if (SAVED_VOl > 0)
            {
                memorisedVolume = SAVED_VOl;
                PlayerPrefs.SetFloat(_prefKey, 0);
            }
            else
            {
                if (memorisedVolume == 0) memorisedVolume = 1;
                PlayerPrefs.SetFloat(_prefKey, memorisedVolume);
            }
            _onVolumeChange.Invoke();
        }
        public void SetVolume(float value)
        {
            PlayerPrefs.SetFloat(_prefKey, value);
            _onVolumeChange.Invoke();
        }
    }
}

