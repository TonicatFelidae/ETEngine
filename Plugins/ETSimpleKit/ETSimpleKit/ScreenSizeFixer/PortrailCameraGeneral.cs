using UnityEngine;

/// <summary>
/// PortrailCameraGeneral resize camera witdh to fix the 2D gameobject witdh
/// </summary>
public class PortrailCameraGeneral : MonoBehaviour
{
    public Transform targetObject; // Assign your game object here

    private Camera _camera;
    private float _initialOrthographicSize;
    private float _initialAspectRatio;  
    private float _targetWidth;

    private void Start()
    {
        Resize();
    }
    private float CalculateTargetWidth()
    {
        Bounds bounds = CalculateObjectBounds(targetObject);
        return bounds.size.x;
    }

    private Bounds CalculateObjectBounds(Transform objTransform)
    {
        Renderer renderer = objTransform.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds;
        }
        else
        {
            Debug.LogWarning("Object does not have a renderer!");
            return new Bounds(objTransform.position, Vector3.zero);
        }
    }
    public void Resize()
    {
        _camera = GetComponent<Camera>();
        _initialOrthographicSize = _camera.orthographicSize;
        _initialAspectRatio = (float)Screen.width / Screen.height;

        if (targetObject != null)
        {
            float targetWidth = CalculateTargetWidth();

            float newOrthographicSize = targetWidth * 0.5f / _initialAspectRatio;
            Debug.Log("[PortrailCameraGeneral] new OrthographicSize: "+ newOrthographicSize);
            _camera.orthographicSize = newOrthographicSize;
        }
    }
}
