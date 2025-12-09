using System.Collections;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("##  GENERAL  ##")]
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private GameObject healthPackPrefab;
    private BackgroundMusic bgMusic;
    private Animator anim;
    private Rigidbody2D enemyRigid;
    private GameManager gameManager;
    private TMP_Text killCount;
    private GameObject player;
    private PlayerCombat playerCombat;
    private LayerMask playerLayer;
    private PlayerMovementLO playerMovement;
    private Rigidbody2D playerRigid;
    private Animator scoreAnim;

    [Header("##  RAGDOLL  ##")]
    private SpriteRenderer enemyRenderer;
    private GameObject enemyEye;
    private GameObject IKControls;
    private GameObject body_bottom;
    private Rigidbody2D rigidBody_bottom;
    private GameObject body_up;
    private Rigidbody2D rigidBody_up;
    private GameObject l_arm;
    private Rigidbody2D rigidL_arm;
    private GameObject l_hand;
    private Rigidbody2D rigidL_hand;
    private GameObject r_arm;
    private Rigidbody2D rigidR_arm;
    private GameObject r_hand;
    private Rigidbody2D rigidR_hand;
    private GameObject l_thigh;
    private Rigidbody2D rigidL_thigh;
    private GameObject l_leg;
    private Rigidbody2D rigidL_leg;
    private GameObject r_thigh;
    private Rigidbody2D rigidR_thigh;
    private GameObject r_leg;
    private Rigidbody2D rigidR_leg;

    [Header("##  MOVEMENT  ##")]
    private bool canMove = true;
    private Vector3 direction;
    private bool isAttacking = false;
    private float speed = 4.5f;
    
    [Header("##  COMBAT  ##")]
    [ShowOnly] public ParticleSystem onHitBleed;
    private float attackCooldown = 1.5f;
    private float attackRange = -1.05f;
    private int chosenAttack;
    private float cooldownTimer = 0;
    private int currentHealth;
    private int damage;
    private bool isKnocked = false;
    private float knockbackSpeed = 3f;
    private int maxHealth;
    private ParticleSystem onDeathBleed;
    
    [Header("##  DETECTION  ##")]
    private float detectionHeight = 0.7f;
    private float detectionWidth = 1.3f;
    private bool detectionWidthUnchanged = true;
    
    
    [Header("## HEIGHT LIMIT ##")]
    public float minY = 0f;
    public float maxY = 3f;
    

    void Awake()
    {
        // BLEED PARTICLES
        onHitBleed = gameObject.transform.Find("BloodOnHit").gameObject.GetComponent<ParticleSystem>();
        onDeathBleed = gameObject.transform.Find("BloodOnDeath").gameObject.GetComponent<ParticleSystem>();
        
        // RAGDOLL
        body_bottom = transform.Find("body_bottom").gameObject;
        rigidBody_bottom = body_bottom.GetComponent<Rigidbody2D>();
        body_up  = body_bottom.transform.Find("body_up").gameObject;
        rigidBody_up = body_up.GetComponent<Rigidbody2D>();
        enemyEye = body_up.transform.Find("head/EnemyEye").gameObject;
        l_arm = body_up.transform.Find("l_arm").gameObject;
        rigidL_arm = l_arm.GetComponent<Rigidbody2D>();
        l_hand = l_arm.transform.Find("l_hand").gameObject;
        rigidL_hand = l_hand.GetComponent<Rigidbody2D>();
        r_arm = body_up.transform.Find("r_arm").gameObject;
        rigidR_arm = r_arm.GetComponent<Rigidbody2D>();
        r_hand = r_arm.transform.Find("r_hand").gameObject;
        rigidR_hand = r_hand.GetComponent<Rigidbody2D>();
        l_thigh = body_bottom.transform.Find("l_thigh").gameObject;
        rigidL_thigh = l_thigh.GetComponent<Rigidbody2D>();
        l_leg = l_thigh.transform.Find("l_leg").gameObject;
        rigidL_leg = l_leg.GetComponent<Rigidbody2D>();
        r_thigh = body_bottom.transform.Find("r_thigh").gameObject;
        rigidR_thigh = r_thigh.GetComponent<Rigidbody2D>();
        r_leg = r_thigh.transform.Find("r_leg").gameObject;
        rigidR_leg = r_leg.GetComponent<Rigidbody2D>();
        IKControls = transform.Find("IKControls").gameObject;
        enemyRenderer = GetComponent<SpriteRenderer>();
        
        // KILLCOUNT TEXT
        killCount = GameObject.Find("Counter").GetComponent<TMP_Text>();
        
        // HEALTH AND DAMAGE DEPENDING ON SCORE
            // CHANGE = KILLCOUNT / SCORE INTERVAL * HOW MUCH
        maxHealth = 80 + int.Parse(killCount.text) / 5 * 10;
        currentHealth = maxHealth;
        damage = 50 + int.Parse(killCount.text) / 10 * 5;
        bgMusic = GameObject.Find("Audio Source").GetComponent<BackgroundMusic>();
        
        // INITIALIZATIONS
        anim = GetComponent<Animator>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        enemyRigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerCombat = player.GetComponent<PlayerCombat>();
        playerLayer = 1 << LayerMask.NameToLayer("PlayerHitbox");
        playerMovement = player.GetComponent<PlayerMovementLO>();
        playerRigid = player.GetComponent<Rigidbody2D>();
        scoreAnim = GameObject.Find("AddedScore").GetComponent<Animator>();
    }
    
    



    void Update()
    {
        cooldownTimer += Time.deltaTime;
        Vector3 playerPos = player.transform.position;
        Vector3 enemyPos = transform.position;

        
        direction = (playerPos - enemyPos).normalized;
        direction.z = 0;
        

        // FLIP
        if(direction.x >= 0 && canMove)
            transform.localScale = new Vector3(-System.Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        if(direction.x < 0)
            onHitBleed.transform.localScale = new Vector2(1, 1);
        else
            onHitBleed.transform.localScale = new Vector2(-1, 1);
            
            
        // PLAYER IN SIGHT
        if(currentHealth > 0 && PlayerInSight(direction))
        {
            // ENEMY STOP
            if(!isKnocked)
                enemyRigid.linearVelocity = Vector2.zero;
            
            
            // ATTACK
            if(cooldownTimer >= attackCooldown && playerCombat.currentHealth > 0)
            {
                cooldownTimer = 0;
                StartCoroutine(Attack());

                anim.ResetTrigger("Run");   // So enemy won't run after first punch
            }

        }
        

        // RUN
        if (canMove && currentHealth > 0 && !PlayerInSight(direction) && !isAttacking)
        {
            anim.SetTrigger("Run");

            Vector3 move = new Vector3(direction.x * speed, direction.y * speed * 0.75f, 0);
            transform.position += move * Time.deltaTime;
            
            Vector3 pos = transform.position;
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            transform.position = pos;
        
            // SCALE BASED ON Y POSITION
            if (direction.x > 0)
                transform.localScale = new Vector3(-0.2f + pos.y * 0.01f, 0.2f - pos.y * 0.01f, 0.2f - pos.y * 0.01f);
            else
                transform.localScale = new Vector3(0.2f - pos.y * 0.01f, 0.2f - pos.y * 0.01f, 0.2f - pos.y * 0.01f);
        }
        
        
        // STOP RUNNING WHEN PLAYER IS DEAD
        if (playerCombat.currentHealth <= 0)
        {
            // DETECTION WIDTH DRAWED
            if (detectionWidthUnchanged)
            {
                detectionWidth = Random.Range(4f, 11f);
                detectionWidthUnchanged = false;
            }
            

            // STOP ENEMY
            if (PlayerInSight(direction))
            {
                anim.ResetTrigger("Run");
                anim.SetTrigger("Stop");
            }
        }

    }

    



    // COMBAT 

    public void TakeDamage(int damage, bool powerUsed)
    {
        currentHealth -= damage;
        
        // DEAD
        if(currentHealth <= 0)
        {
            scoreAnim.ResetTrigger("+1");
            scoreAnim.SetTrigger("+1");
            bgMusic.PlayEnemyDeathSound();
            onDeathBleed.Play();
            boxCollider.enabled = false;
            gameManager.score += 1;
            killCount.text = (int.Parse(killCount.text) + 1).ToString();
            enemyRigid.linearVelocity = Vector2.zero;


            // DROP HEALUP CHANCE
            if (Random.Range(1, 101) <= gameManager.healUpChance)
                Instantiate(healthPackPrefab, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);

            // GRANT POWER POINTS
            if (player.GetComponent<PlayerCombat>().powerPoints < 10 && !powerUsed)
                player.GetComponent<PlayerCombat>().powerPoints += 1;
            
            
            anim.enabled = false;
            IKControls.SetActive(false);
            enemyRigid.linearVelocity = Vector2.zero;
            RagdollOn();
            StartCoroutine(FadeEnemy());
            enabled = false;
            GameObject.Find("Enemy Spawner").GetComponent<SpawnEnemy>().OnEnemyKilled();
        }
            
    }
    

    public void Knockback()
    {
        Vector3 direction = (player.transform.position - enemyRigid.transform.position).normalized;
        direction.y = 0;
        enemyRigid.linearVelocity = new Vector3(-direction.x * knockbackSpeed, enemyRigid.linearVelocity.y);
        StartCoroutine(Knocked());
        
    }
    

    private bool PlayerInSight(Vector3 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + new Vector3(transform.localScale.x, 0)
            * attackRange, new Vector2(boxCollider.bounds.size.x * detectionWidth, boxCollider.bounds.size.y * detectionHeight), 0, direction, 0, playerLayer);
        return hit.collider != null;
    }
    

    public void DamagePlayer()
    {
        if(PlayerInSight(direction) && !playerMovement.isDashing)
        {
            player.GetComponent<PlayerCombat>().rigidBody_up.linearVelocity = new Vector2(direction.x * 16, 0);
            playerRigid.linearVelocity = new Vector2(direction.x * 3, 0);
            playerCombat.TakeDamage(damage);
            playerMovement.anim.ResetTrigger("AD_Pressed");
            playerMovement.anim.SetTrigger("AD_Dropped");
            playerMovement.enabled = false;

            // KNOCKBACK PLAYER
            StartCoroutine(KnockPlayer());
        }
    }
    

    private void RagdollOn()
    {
        rigidBody_bottom.simulated = true;
        rigidBody_up.simulated = true;
        rigidL_arm.simulated = true;
        rigidL_hand.simulated = true;
        rigidR_arm.simulated = true;
        rigidR_hand.simulated = true;
        rigidL_thigh.simulated = true;
        rigidL_leg.simulated = true;
        rigidR_thigh.simulated = true;
        rigidR_leg.simulated = true;
    }


    // GIZMOS - FOR VISUALIZATION
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + new Vector3(transform.localScale.x, 0) * attackRange,
            new Vector2(boxCollider.bounds.size.x * detectionWidth, boxCollider.bounds.size.y * detectionHeight));
    }
    
    
    
    

    // IENUMERATORS

    IEnumerator Knocked()
    {
        canMove = false;
        isKnocked = true;
        
        anim.SetTrigger("Knockbacked");
        
        yield return new WaitForSeconds(0.5f);
        
        anim.ResetTrigger("Knockbacked");
        
        if (!isAttacking)
            canMove = true;
        
        isKnocked = false; 
        
        if (currentHealth > 0)
            enemyRigid.linearVelocity = Vector2.zero;
    }
    

    IEnumerator FadeEnemy()
    {
        Vector3 direction = (player.transform.position - enemyRigid.transform.position).normalized;
        enemyEye.SetActive(false);
        float time = 0;
        float duration = 2;

        // RAGDOLL KICKBACK
        var rand = Random.Range(1, 10);
        if (rand % 2 == 0)
            rigidBody_up.linearVelocity = new Vector2(-direction.x * 40, 0);
        else
            rigidBody_bottom.linearVelocity = new Vector2(-direction.x * 40, 0);


        while (time < duration)
        {
            enemyRigid.linearVelocity = new Vector2(0, 0);
            float alpha = Mathf.Lerp(1, 0, time / duration);

            Color newColorEnemy = enemyRenderer.color;
            Color newColorEye = enemyRenderer.color;
            newColorEnemy.a = alpha;
            newColorEye.a = alpha;
            enemyRenderer.color = newColorEnemy;

            time += Time.deltaTime;
            yield return null;
        }
        
        Destroy(gameObject);
    }
    

    IEnumerator Attack()
    {
        // enemyRigid.bodyType = RigidbodyType2D.Static;
        
        canMove = false;
        isAttacking = true;
        
        // ATTACK ANIMATION
        chosenAttack = Random.Range(1, 4);
        switch (chosenAttack)
        {
            case 1:
                anim.SetTrigger("Attack1");
                break;
            case 2:
                anim.SetTrigger("Attack2");
                break;
            case 3:
                anim.SetTrigger("Attack3");
                break;
            default:
                Debug.Log("Bruh");
                break;
        }

        // CHARGE
        
        yield return new WaitForSeconds(0.15f);
        //transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x + -transform.localScale.x * 5, transform.position.y), 0.9f);
        yield return new WaitForSeconds(0.85f);
        
        canMove = true;

        isAttacking = false;
        
        // enemyRigid.bodyType = RigidbodyType2D.Dynamic;
    
    }
    

    IEnumerator KnockPlayer()
    {
        yield return new WaitForSeconds(0.5f);

        if (playerCombat.currentHealth > 0)
            playerMovement.enabled = true;
    }
}
