using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETSimpleKit.UI;
using ET.UIKit;

namespace ETSimpleKit
{
    public class EOBindEU : MonoBehaviour, IUISizeChangeListener
    {
        public GameObject EUElement;
        public bool bindLocation;

        public void OnUISizeChange()
        {
            if(bindLocation) 
            {
                //Vector3 screenPosition = Camera.main.WorldToScreenPoint(EUElement.transform.position);

                // Convert the screen space position to world space.
                Vector3 buttonWorldPosition = Camera.main.ScreenToWorldPoint(EUElement.transform.position);

                // Set the object's position to the button's position in the world.
                transform.position = new Vector3(buttonWorldPosition.x, buttonWorldPosition.y, transform.position.z);
            }
        }
    }
}
