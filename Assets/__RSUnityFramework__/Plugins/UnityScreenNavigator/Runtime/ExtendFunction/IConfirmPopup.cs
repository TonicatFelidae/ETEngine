using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public interface IActionPopup
{
    public void Init(UnityAction onConfirm,  UnityAction onCancle, UnityAction action1= null, UnityAction action2= null, UnityAction action3= null);
}
