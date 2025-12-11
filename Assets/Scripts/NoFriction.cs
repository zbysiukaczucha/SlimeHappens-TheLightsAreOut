using UnityEngine;

public class NoFriction : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    

    
    
    
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        PhysicsMaterial2D lowFrictionMaterial = new()
        {
            friction = 0
        };

        boxCollider.sharedMaterial = lowFrictionMaterial;
    }
}
