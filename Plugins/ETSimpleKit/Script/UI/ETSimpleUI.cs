using System.Collections.Generic;
using UnityEngine;

namespace ETSimpleKit
{
    public class ETSimpleUI : MonoBehaviour
    {
        public UIType type;
        public List<float> _numValue;
        public List<string> _stringValues;

        public float ID => _numValue[0];
        public float DataNum0 => _numValue[1];
        public void Active(bool enable, List<float> numValues, List<string> stringValues = null)
        {
            _numValue = numValues;
            _stringValues = stringValues;
            Active(enable);
        }
        public void Active(bool enable, List<string> stringValues)
        {
            _stringValues = stringValues;
            Active(enable);
        }
        /// <summary>
        /// Active type 3: inject nothing, no change on already had datas
        /// </summary>
        /// <param name="enable"></param>
        public void Active(bool enable)
        {
            gameObject.SetActive(enable);
            if (enable)
            {
                OnActive();
            }
            else
            {
                OnUnActive();
            }
        }
        public virtual void OnActive()
        {

        }
        public virtual void OnUnActive()
        {

        }


    }

}


