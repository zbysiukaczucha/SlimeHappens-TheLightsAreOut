using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private float startingPos;
    private float lengthOfSprite;
    public float amountOfParallax;
    public Camera mainCamera;
    



    void Start()
    {
        startingPos = transform.position.x;
        lengthOfSprite = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    


    
    void Update()
    {
        Vector3 position = mainCamera.transform.position;
        float Temp = position.x * (1 - amountOfParallax);
        float distance = position.x * amountOfParallax;

        Vector3 newPosition = new Vector3(startingPos + distance, transform.position.y, transform.position.z);

        transform.position = newPosition;
    }
}
