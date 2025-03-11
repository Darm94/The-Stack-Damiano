using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]

public class GameManager : MonoBehaviour
{
    PrimitiveType primitiveToPlace;
    private int _teamToAssign;
    Vector3 nextShapePreviewPos = new Vector3(-7, 1, 1);
    GameObject previewObject;
    private Dictionary<int, Color> _colorsEnum;
    private AudioSource _audioSource;
    private AudioClip _spawnSound; // AudioClip da riprodurre
    private int _scoreCounter = 0;
    [SerializeField] private int _maxLifePoints = 5;
    private int _lifePoints;
    
    //Editor Variables for spawned objects
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private KeyCode rotateForwardKey = KeyCode.W;
    [SerializeField] private KeyCode rotateBackwardKey = KeyCode.S;
    [SerializeField] private KeyCode rotateLeftKey = KeyCode.A;
    [SerializeField] private KeyCode rotateRightKey = KeyCode.D;
    [SerializeField] private KeyCode deepMovementKey =KeyCode.LeftShift;

    //lifePoints property
    public int LifePoints
    {
        get { return _lifePoints; }
        set { _lifePoints = value; }
    }
    
    private void Start()
    {
        _lifePoints = _maxLifePoints;
        _audioSource = GetComponent<AudioSource>();
        _spawnSound=Resources.Load<AudioClip>("spawn_sound");
        AssignTeamsColor();
        GenerateNextShape();
        
    }

    void AssignTeamsColor()
    {
        _colorsEnum =new Dictionary<int, Color>()
        {
            { 0, GetRandomColor() },
            { 1, GetRandomColor() },
            { 2, GetRandomColor() },
            { 3, GetRandomColor() },
            { 4, GetRandomColor() }
        };
    }
    void GenerateNextShape()
    {
        switch (Random.Range(0, 4)) // 0..3 (4 excluded)
        {
            case 0:
                primitiveToPlace = PrimitiveType.Cube;
                break;
            case 1:
                primitiveToPlace = PrimitiveType.Sphere;
                break;
            case 2: primitiveToPlace = PrimitiveType.Capsule; 
                break;
            case 3: primitiveToPlace = PrimitiveType.Cylinder;
                break;
            default: primitiveToPlace = PrimitiveType.Cube; 
                break;
        }
        //Color team match
        _teamToAssign = Random.Range(0, 5);
            
        if (previewObject) Destroy(previewObject);

        previewObject = GameObject.CreatePrimitive(primitiveToPlace);
        previewObject.GetComponent<MeshRenderer>().material.color = _colorsEnum[ _teamToAssign];
        previewObject.name = "Preview shape";
        previewObject.transform.position = nextShapePreviewPos;
    }

    void Update()
    {
        //GameOverCheck
        if (_lifePoints<=0)
        {
            //GameOver();
            Debug.Log("GAME OVER");
            GetComponent<Camera>().enabled = false;
            this.enabled = false;
            
        }
        
        //Spawn new Objects
        if (Input.GetMouseButtonDown(1)) // Right Click 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                //Score update
                _scoreCounter++;
                Debug.Log("Score: " + _scoreCounter);
                
                // Spawn Sound
                if (_spawnSound&& _audioSource)
                {
                    _audioSource.PlayOneShot(_spawnSound); 
                }
                
                //New Game Object
                GameObject go = GameObject.CreatePrimitive(primitiveToPlace);
                go.transform.localScale = Vector3.one * 0.3f;
                go.transform.position = hit.point + new Vector3(0, 1, 0);
                go.transform.rotation = Random.rotation;
                
                //add RigidBody
                go.AddComponent<Rigidbody>();
                
                //add color
                MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
                meshRenderer.material.color = _colorsEnum[ _teamToAssign];

                // add texture
                Texture texture = Resources.Load<Texture>("unsic-texture");
                Debug.Log(texture);
                meshRenderer.material.mainTexture = texture; // Correzione: mainTexture

                //add and assign team data
                ColorTeamData teamData = go.AddComponent<ColorTeamData>();
                teamData.ColorTeam = _teamToAssign;
                //Debug.Log(teamData.ColorTeam);
                
                //add custom scripts
                go.AddComponent<DestroyOnFall>();
                DragWithMouse dwm = go.AddComponent<DragWithMouse>();
                go.AddComponent<TeamDestroy>();

                //DragWithMouse variables set
                dwm.KeysMapSet(rotateForwardKey, rotateBackwardKey, rotateLeftKey, rotateRightKey, deepMovementKey);
                dwm.RotationSpeedSet(rotationSpeed);
                
                //next preview
                GenerateNextShape();
            }
        }
    }
    
    private static Color GetRandomColor()
    {
        // Controllo casualità del colore
        Color randomColor = Random.ColorHSV();

        float H, S, V;
        Color.RGBToHSV(randomColor, out H, out S, out V);
        S = 0.8f;
        V = 0.95f;
        randomColor = Color.HSVToRGB(H, S, V);
        return randomColor;
    }
    
    
}