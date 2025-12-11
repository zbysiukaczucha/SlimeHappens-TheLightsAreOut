using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HealUP : MonoBehaviour
{
    [Header("##  GENERAL  ##")]
    private GameManager gameManager;
    private CircleCollider2D circleCollider;
    private GameObject player;
    private PlayerCombat playerCombat;
    private LayerMask playerLayer;
    private Rigidbody2D rigid;
    
    [Header("##  GROW ANIMATION  ##")]   
    private float amplitude = 0.33f;
    private float frequency = 1;
    private Vector2 startingScale;
    private Vector2 tempScale;
    
    // [Header("##  GLOW ANIMATION  ##")]
    // [SerializeField] private float glowAmplitude;
    // [SerializeField] private float glowFrequency;

    [Header("##  GLOW ANIMATION  ##")]
    private Light2D glow;
    private float tempFalloff;
    




    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        
        rigid.linearVelocity = new Vector2(0, 5);

        player = GameObject.Find("Player");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerCombat = player.GetComponent<PlayerCombat>();
        playerLayer = 1 << player.layer;

        glow = GetComponent<Light2D>();
    }
    




    void Start()
    {
        startingScale = transform.localScale;
    }
    




    void Update()
    {
        if(PlayerTouch() && playerCombat.currentHealth < playerCombat.maxHealth)
        {
            playerCombat.HealPlayer(gameManager.healAmount);
            Destroy(gameObject);
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
}

