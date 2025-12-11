using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PowerUP : MonoBehaviour
{
    [Header("##  GENERAL  ##")]
    [SerializeField] private CircleCollider2D circleCollider;
    private GameObject gameManager;
    private LayerMask playerLayer;
    private GameObject PowerUpChoiceScreen;
    private Rigidbody2D rigid;
    private Upgrades upgrades;
    
    [Header("##  GROW ANIMATION  ##")]   
    private Light2D glow;
    private float amplitude = 0.33f;
    private float frequency = 1;
    private Vector2 startingScale;
    private Vector2 tempScale;
    private float tempFalloff;
    
    // [Header("##  GLOW ANIMATION  ##")]
    // [SerializeField] private float glowAmplitude;
    // [SerializeField] private float glowFrequency;

    




    void Awake()
    {
        gameManager = GameObject.Find("GameManager");
        playerLayer = 1 << LayerMask.NameToLayer("Player");
        PowerUpChoiceScreen = FindInactiveObjectByName("PowerUpChoiceScreen");
        upgrades = gameManager.GetComponent<Upgrades>();

        rigid = GetComponent<Rigidbody2D>();
        rigid.linearVelocity = new Vector2(0, 5);
        
        glow = GetComponent<Light2D>();
    }
    




    void Start()
    {
        startingScale = transform.localScale;
    }
    

    


    void Update()
    {
        if(PlayerTouch())
        {
            upgrades.CreateChoice();
            Destroy(gameObject);
            Time.timeScale = 0;
            PowerUpChoiceScreen.SetActive(true);
        }

        // GROW ANIMATION
        {
            tempScale = startingScale;

            var sinus = Mathf.Sin(Time.timeSinceLevelLoad * Mathf.PI * frequency) * amplitude / 10;
            tempScale.x += sinus;
            tempScale.y += sinus;

            transform.localScale = tempScale;
        }

        // GLOW ANIMATION
        {
            tempFalloff = 0.8f;

            var sinus = Mathf.Sin(Time.timeSinceLevelLoad * Mathf.PI * frequency) * amplitude / 2;
            tempFalloff -= sinus;

            glow.falloffIntensity = tempFalloff;
        }
    }
    
    



    private bool PlayerTouch()
    {
        RaycastHit2D hit = Physics2D.CircleCast(circleCollider.bounds.center, 0.45f, Vector2.zero, 0, playerLayer);
        return hit.collider != null;
    }
    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(circleCollider.bounds.center, circleCollider.radius);
    }
    
    
    GameObject FindInactiveObjectByName(string name)
    {
        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>();
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].hideFlags == HideFlags.None && objs[i].name == name)
                return objs[i].gameObject;
        }
        
        return null;
    }
}
