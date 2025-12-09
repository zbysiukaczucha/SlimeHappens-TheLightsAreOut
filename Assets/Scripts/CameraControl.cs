using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private Transform playerTransform;
    private float centerDelay = 0.4f;
    private Vector3 offset = new(0, 2, -10);
    private Vector3 velocity = Vector3.zero;

    private Animator anim;
    private Vector3 targetPosition;
    
    
    void Awake()
    {
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        
        anim = playerTransform.GetComponent<Animator>();
    }
    
    void Update()
    {
        if(anim.GetBool("Finished_Falling_Touchdown"))
            targetPosition = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y + offset.y / 2,
                offset.z);
        else
            targetPosition = new Vector3(
                playerTransform.position.x,
                offset.y,
                offset.z);

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, centerDelay);
    }

}
