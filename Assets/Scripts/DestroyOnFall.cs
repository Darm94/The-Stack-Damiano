using Unity.VisualScripting;
using UnityEngine;

public class DestroyOnFall : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10)
        {
            Camera mainCamera = Camera.main;
            GameManager gm = null;
            if(mainCamera)
                gm = mainCamera.GetComponent<GameManager>();
            if (gm)
            {
                gm.LifePoints--;
                Debug.Log("LIFE POINTS: " + gm.LifePoints);
            }
            
            Destroy(gameObject);
        }
    }
}
