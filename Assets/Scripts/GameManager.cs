using UnityEngine;

public class GameManager : MonoBehaviour
{
    PrimitiveType primitiveToPlace;
    Vector3 nextShapePreviewPos = new Vector3(-7, 1, 1);
    GameObject previewObject;

    private void Start()
    {
        GenerateNextShape();
    }

    void GenerateNextShape()
    {
        switch (Random.Range(0, 4)) // 0..3 (4 è escluso)
        {
            case 0: primitiveToPlace = PrimitiveType.Cube; break;
            case 1: primitiveToPlace = PrimitiveType.Sphere; break;
            case 2: primitiveToPlace = PrimitiveType.Capsule; break;
            case 3: primitiveToPlace = PrimitiveType.Cylinder; break;
            default: primitiveToPlace = PrimitiveType.Cube; break;
        }

        if (previewObject) Destroy(previewObject);

        previewObject = GameObject.CreatePrimitive(primitiveToPlace);
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

                // Controllo casualità del colore
                Color randomColor = Random.ColorHSV();

                float H, S, V;
                Color.RGBToHSV(randomColor, out H, out S, out V);

                S = 0.8f;
                V = 0.8f;

                randomColor = Color.HSVToRGB(H, S, V);

                go.GetComponent<MeshRenderer>().material.color = randomColor;

                // DEVE ESSERE ALL'INTERNO DI Assets/Resources
                Texture texture = Resources.Load<Texture>("wood_texture");

                Debug.Log(texture);

                go.GetComponent<MeshRenderer>().material.mainTexture = texture; // Correzione: mainTexture

                go.AddComponent<DestroyOnFall>();
                go.AddComponent<DragWithMouse>();

                GenerateNextShape();
            }
        }
    }
    
    
}