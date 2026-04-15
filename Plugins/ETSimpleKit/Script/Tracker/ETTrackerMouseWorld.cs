using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ETTrackerMouseWorld : MonoBehaviour
{
    [SerializeField] private Camera _UICamera;
    [SerializeField] private GameObject pp_tracker; // have tag ETTracker
    [SerializeField] private Vector3 _objectOffset; //DO NOT SET VECTOR ZERO
    public Vector3 _mousePosition;
    public Vector3 _worldPosition;
    private GameObject _tracker;
    private void Start()
    {
        _tracker = Instantiate(pp_tracker,this.transform);
    }
    private void Update()
    {
        // Get the mouse position in screen space
        _mousePosition = Input.mousePosition + _objectOffset;

        // Convert the mouse position from screen space to world space
        _worldPosition = _UICamera.ScreenToWorldPoint(_mousePosition);

        // Update the position of the GameObject
        _tracker.transform.position = _worldPosition;
    }
}
