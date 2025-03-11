
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
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private KeyCode rotateForwardKey = KeyCode.W;
    [SerializeField] private KeyCode rotateBackwardKey = KeyCode.S;
    [SerializeField] private KeyCode rotateLeftKey = KeyCode.A;
    [SerializeField] private KeyCode rotateRightKey = KeyCode.D;
    private Vector3 rotationAxis = Vector3.zero; // Asse di rotazione
    private float rotationDirection = 0f; // Direzione di rotazione (-1 o 1)
    private bool isDragging = false;
    private bool isShifting = false;
    private float currentZ; // Posizione Z corrente dell'oggetto
    private float initialMouseY; // Posizione Y iniziale del mouse quando Shift è premuto
    private TeamDestroy collisionCounter ;
    
    
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        collisionCounter = GetComponent<TeamDestroy>();
    }
    
    private void Update()
    {
        RotateCheck();
        
        bool wasShifting = isShifting;  // Salviamo lo stato precedente di isShifting
        isShifting = Input.GetKey(KeyCode.LeftShift);
        
        
        if (isDragging && Input.GetKeyDown(KeyCode.LeftShift))
        {
            currentZ = transform.position.z;
            initialMouseY = Input.mousePosition.y;
            _screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            _offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,  _screenPoint.z));
            
        }
        
        // Se prima era in modalità Shift e ora non lo è più, aggiorniamo l'offset per evitare scatti
        if (wasShifting && !isShifting)
        {
            _screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            _offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));
        }

    }

    private void RotateCheck()
    {
        if (isDragging)
        {
            if (Input.GetKey(rotateForwardKey))
            {
                rotationAxis = Vector3.forward;
                rotationDirection = 1f;
            }
            else if (Input.GetKey(rotateBackwardKey))
            {
                rotationAxis = Vector3.forward;
                rotationDirection = -1f;
            }
            else if (Input.GetKey(rotateLeftKey))
            {
                rotationAxis = Vector3.right;
                rotationDirection = -1f;
            }
            else if (Input.GetKey(rotateRightKey))
            {
                rotationAxis = Vector3.right;
                rotationDirection = 1f;
            }
            else
            {
                rotationAxis = Vector3.zero;
                rotationDirection = 0f;
            }
        }
        else
        {
            rotationAxis = Vector3.zero;
            rotationDirection = 0f;
        }
    }
    
    // Event function
    void OnMouseDown()
    {
        
        _screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        _offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,  _screenPoint.z));
        
        _rigidBody.isKinematic = true;
        _nextPosition = Vector3.zero;
        isDragging = true;
        
        // Disattiva la componente per evitare che distrugga l'oggetto durante il trascinamento
        if (collisionCounter)
        {
            collisionCounter.enabled = false;
            Debug.Log("Componente TeamDestroy disabilitata");
        }
    }
    

    void OnMouseDrag()
    {

        Vector3 curScreenPoint;
        if (isShifting)
        {
            curScreenPoint = new Vector3(Input.mousePosition.x, _screenPoint.y, _screenPoint.z); 
            float zOffset = (Input.mousePosition.y - initialMouseY) * 0.01f; // Sensibilità del movimento

            // Mantenere la posizione XY attuale, modificando solo la Z globale
            _nextPosition = new Vector3(transform.position.x, transform.position.y, currentZ + zOffset);
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
        if (isDragging && rotationAxis != Vector3.zero)
        {
            Quaternion rotation = Quaternion.AngleAxis(rotationSpeed * rotationDirection * Time.fixedDeltaTime, rotationAxis);
            _rigidBody.MoveRotation(_rigidBody.rotation * rotation); // Correzione: Applica la rotazione al Rigidbody
        }
    }

    private void OnMouseUp()
    {
            
            isDragging = false;
            isShifting = false;
            // Disattiva la componente
            if (collisionCounter)
            {
                collisionCounter.enabled = true;
                
            }
            _rigidBody.isKinematic = false;
            _nextPosition = Vector3.zero;
    }
}