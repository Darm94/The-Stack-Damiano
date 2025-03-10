using System;
using UnityEngine;

public class DragWithMouse : MonoBehaviour
{
    private Vector3 _screenPoint;
    private Vector3 _offset;
    private Rigidbody _rigidBody;
    private Vector3 _nextPosition;

    // Event function
    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    // Event function
    void OnMouseDown()
    {
        _screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

        _offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));

        _rigidBody.isKinematic = true;
        _nextPosition = Vector3.zero;
    }

    // (Manca il metodo OnMouseDrag)
    // (Manca il metodo OnMouseUpAsButton)


    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
        _nextPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + _offset;
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(_nextPosition, Vector3.zero) > 0.01f) 
        {
            _rigidBody.position = _nextPosition; 
        }
    }

    private void OnMouseUpAsButton()
    {
        _rigidBody.isKinematic = false; 
        _nextPosition = Vector3.zero;
    }
    
}