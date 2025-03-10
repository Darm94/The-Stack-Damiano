using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    private int collisions = 0;

    private void OnCollisionEnter(Collision collision)
    {
        collisions++;
        Debug.Log($"Collisions: {collisions}");
    }

    private void OnCollisionExit(Collision collision)
    {
        collisions--;
        Debug.Log($"Collisions: {collisions}");
    }
}