using Unity.VisualScripting;
using UnityEngine;

public class DestroyOnFall : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10)
        {
            GameManager gm = Camera.main.GetComponent<GameManager>();
            gm.LifePoints--;
            Destroy(gameObject);
        }
    }
}
