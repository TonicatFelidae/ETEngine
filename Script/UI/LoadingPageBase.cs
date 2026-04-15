using UnityEngine;

public abstract class LoadingPageBase: MonoBehaviour
{
    public abstract void Show(string messege = null);
    public abstract void Hide();


    public abstract void SetText(string value);
}
