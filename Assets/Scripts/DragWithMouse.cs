using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class DragWithMouse : MonoBehaviour
{
    private Vector3 _screenPoint;
    private Vector3 _offset;
    private Rigidbody _rigidBody;
    private Vector3 _nextPosition;
    
    
    //variable for rotation va spostato in MAINCLASS
    private float _rotationSpeed = 100f;
    private KeyCode _rotateForwardKey = KeyCode.W;
    private KeyCode _rotateBackwardKey = KeyCode.S;
    private KeyCode _rotateLeftKey = KeyCode.A;
    private KeyCode _rotateRightKey = KeyCode.D;
    private KeyCode _deepMovementKey =KeyCode.LeftShift; 
    private Vector3 _rotationAxis = Vector3.zero; // Rotation Axis
    private float _rotationDirection = 0f; // Rotation Direction (-1 or 1)
    private bool _isDragging = false;
    private bool _isShifting = false;
    private float _currentZ; // Variable for actual Z axis position of the object
    private float _initialMouseY; // Variable for actual Y position when start deep movement
    private TeamDestroy _collisionCounter;

   public void KeysMapSet(KeyCode rotateForwardKey, KeyCode rotateBackwardKey, KeyCode rotateLeftKey, KeyCode rotateRightKey, KeyCode deepMovementKey)
    {
        _rotateForwardKey = rotateForwardKey;
        _rotateBackwardKey = rotateBackwardKey;
        _rotateLeftKey = rotateLeftKey;
        _rotateRightKey = rotateRightKey;
        _deepMovementKey = deepMovementKey;
    }

    public void RotationSpeedSet(float rotationSpeed)
    {
        _rotationSpeed = rotationSpeed;
    }
    
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _collisionCounter = GetComponent<TeamDestroy>();
    }
    
    private void Update()
    {
        RotateCheck();
        
        bool wasShifting = _isShifting;  // Save precedent state of isShifting
        _isShifting = Input.GetKey(_deepMovementKey);
        
        
        if (_isDragging && Input.GetKeyDown(_deepMovementKey))
        {
            _currentZ = transform.position.z;
            _initialMouseY = Input.mousePosition.y;
            _screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            _offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,  _screenPoint.z));
            
        }
        
        // If before was Shifting, update the offset to avoid position teleport
        if (wasShifting && !_isShifting)
        {
            _screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            _offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));
        }

    }

    private void RotateCheck()
    {
        if (_isDragging)
        {
            if (Input.GetKey(_rotateForwardKey))
            {
                _rotationAxis = Vector3.forward;
                _rotationDirection = 1f;
            }
            else if (Input.GetKey(_rotateBackwardKey))
            {
                _rotationAxis = Vector3.forward;
                _rotationDirection = -1f;
            }
            else if (Input.GetKey(_rotateLeftKey))
            {
                _rotationAxis = Vector3.right;
                _rotationDirection = -1f;
            }
            else if (Input.GetKey(_rotateRightKey))
            {
                _rotationAxis = Vector3.right;
                _rotationDirection = 1f;
            }
            else
            {
                _rotationAxis = Vector3.zero;
                _rotationDirection = 0f;
            }
        }
        else
        {
            _rotationAxis = Vector3.zero;
            _rotationDirection = 0f;
        }
    }
    
    // Event function
    void OnMouseDown()
    {
        
        _screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        _offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,  _screenPoint.z));
        
        _rigidBody.isKinematic = true;
        _nextPosition = Vector3.zero;
        _isDragging = true;
        
        // If we are already pressing Shift upload actual position (to evoid deep z axis teleport of the object)
        if (_isShifting)
        {
            _currentZ = transform.position.z;
            _initialMouseY = Input.mousePosition.y;
        }
        
        // Disable the component to avoid the distruction during Drag Session
        if (_collisionCounter)
        {
            _collisionCounter.enabled = false;
            Debug.Log("Componente TeamDestroy disabilitata");
        }
    }
    

    void OnMouseDrag()
    {

        Vector3 curScreenPoint;
        if (_isShifting)
        {
            //curScreenPoint = new Vector3(Input.mousePosition.x, _screenPoint.y, _screenPoint.z); 
            //How much move the Z position
            float zOffset = (Input.mousePosition.y - _initialMouseY) * 0.01f;

            // Keep actual XY position, modifying the Z using the Zoffset
            _nextPosition = new Vector3(transform.position.x, transform.position.y, _currentZ + zOffset);
        }
        else
        {
            curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
            _nextPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + _offset;
        }
    }

    private void FixedUpdate()
    {
        //Position
        if (Vector3.Distance(_nextPosition, Vector3.zero) > 0.01f) 
        {
            _rigidBody.position = _nextPosition; 
        }
        
        //  Rotation through the Rigidbody 
        if (_isDragging && _rotationAxis != Vector3.zero)
        {
            Quaternion rotation = Quaternion.AngleAxis(_rotationSpeed * _rotationDirection * Time.fixedDeltaTime, _rotationAxis);
            _rigidBody.MoveRotation(_rigidBody.rotation * rotation); // fix: apply the rotation to rigidBody us Quaternion (costruction of the rotation taken from web)
        }
    }

    private void OnMouseUp()
    {
            
            _isDragging = false;
            _isShifting = false;
            // TeamDestroy component restoration
            if (_collisionCounter)
            {
                _collisionCounter.enabled = true;
                
            }
            _rigidBody.isKinematic = false;
            _nextPosition = Vector3.zero;
    }
}