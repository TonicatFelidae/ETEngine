using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
namespace ETSimpleKit
{
    public class UB_ShowHideAfterSeconds : MonoBehaviour
    {
        //DO NOT TARGET SELF
        public GameObject target;
        public float time;
        public bool isShow;
        void OnEnable()
        {
            target.SetActive(!isShow);
            StartCoroutine(ShowHideAfterTime(time));
        }
        IEnumerator ShowHideAfterTime(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            target.SetActive(isShow);
        }
    }
}