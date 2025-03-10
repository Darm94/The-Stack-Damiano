using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    PrimitiveType primitiveToPlace;
    private Color _colorToPlace;
    Vector3 nextShapePreviewPos = new Vector3(-7, 1, 1);
    GameObject previewObject;
    private Dictionary<int, Color> _colorsEnum; 
    private void Start()
    {
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
            case 0: primitiveToPlace = PrimitiveType.Cube; break;
            case 1: primitiveToPlace = PrimitiveType.Sphere; break;
            case 2: primitiveToPlace = PrimitiveType.Capsule; break;
            case 3: primitiveToPlace = PrimitiveType.Cylinder; break;
            default: primitiveToPlace = PrimitiveType.Cube; break;
        }
        //Color match
        _colorToPlace = _colorsEnum[Random.Range(0, 5)];
            
        if (previewObject) Destroy(previewObject);

        previewObject = GameObject.CreatePrimitive(primitiveToPlace);
        previewObject.GetComponent<MeshRenderer>().material.color = _colorToPlace;
        previewObject.name = "Preview shape";
        previewObject.transform.position = nextShapePreviewPos;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Clic del mouse destro
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100)) // 100 metri/unità di distanza massima
            {
                GameObject go = GameObject.CreatePrimitive(primitiveToPlace);

                go.transform.localScale = Vector3.one * 0.3f;
                go.transform.position = hit.point + new Vector3(0, 1, 0); // Sposta di 1 metro verso l'alto
                go.transform.rotation = Random.rotation;

                go.AddComponent<Rigidbody>();
                
                
                MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
                meshRenderer.material.color = _colorToPlace;

                // DEVE ESSERE ALL'INTERNO DI Assets/Resources
                Texture texture = Resources.Load<Texture>("unsic-texture");
                Debug.Log(texture);

                meshRenderer.material.mainTexture = texture; // Correzione: mainTexture

                go.AddComponent<DestroyOnFall>();
                go.AddComponent<DragWithMouse>();

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