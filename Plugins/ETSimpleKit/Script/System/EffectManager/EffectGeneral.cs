using ET;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ETSimpleKit.EffectSystem
{
    public class EffectGeneral : Singleton<EffectGeneral>
    {
        [SerializeField] private GameObject _effectBox;
        private ETPoolManager _eTPoolManager = new();
        public GameObject EffectBox
        {
            get
            {
                if (_effectBox == null)
                {
                    _effectBox = new GameObject();
                    _effectBox.name = "EffectBox";
                }
                return _effectBox;
            }
        }
        #region Single Effect
        [Header("Single Effects")]
        public List<GameObject> effects;
        public void ShowEffect0(Transform transform) => ShowEffect(0, transform);
        public void ShowEffect1(Transform transform) => ShowEffect(1, transform);
        public void ShowEffect2(Transform transform) => ShowEffect(2, transform);
        public void ShowEffect3(Transform transform) => ShowEffect(3, transform);
        public void ShowEffect4(Transform transform) => ShowEffect(4, transform);
        public void ShowEffect5(Transform transform) => ShowEffect(5, transform);
        public void ShowEffect(int ID, Transform transform) => ShowEffect(ID, transform.position);
        /// <summary>
        /// Normal GO effect
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="position"></param>
        public void ShowEffect(int ID, Vector3 position)
        {
            try
            {
                ETEffect go = _eTPoolManager.GetObjectFromPool<ETEffect>(ID.ToString(), EffectBox.transform, effects[ID]);
                go.transform.position = position;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.LogError("ShowEffect effectsCount :" + effects.Count);
                Debug.LogError("ShowEffect ID :" + ID);
                Debug.LogError("ShowEffect position :" + position);
            }
        }
        #endregion
        #region Buildin Effect
        [Header("Buildin Partial Effects")]
        public List<ParticleSystem> buildinParticleEffects;
        public void StopAllBPE(bool isForceDisableAll = false)
        {
            foreach (var item in buildinParticleEffects)
            {
                item.Stop();
                if (isForceDisableAll) item.gameObject.SetActive(false);
            }
        }
        public void ShowBPE(int index, BPEShowType showType = BPEShowType.Single)
        {
            switch (showType)
            {
                case BPEShowType.Single:
                    StopAllBPE();
                    break;
                case BPEShowType.SingleForceDisableAll:
                    StopAllBPE(true);
                    break;
                case BPEShowType.Additional:
                    break;
                default:
                    break;
            }
            buildinParticleEffects[index].gameObject.SetActive(true);
            buildinParticleEffects[index].Play();
        }
        public ParticleSystem GetBPE(int index)
        {
            return buildinParticleEffects[index];
        }
        #endregion
        #region String Effect
        /// <summary>
        /// ShowEffect Popup With String Data
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="position"></param>
        /// <param name="text"></param>
        public void ShowEffect(int ID, Vector3 position, string textData)
        {
            try
            {
                ETEffect go = _eTPoolManager.GetObjectFromPool<ETEffect>(ID.ToString(), EffectBox.transform, effects[ID]);
                go.Init(textData);
                go.transform.position = position;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.LogError("ShowEffect effectsCount :" + effects.Count);
                Debug.LogError("ShowEffect ID :" + ID);
                Debug.LogError("ShowEffect position :" + position);
            }
        }
        #endregion

    }
    public enum BPEShowType
    {
        Single,
        SingleForceDisableAll,
        Additional,
    }

}
