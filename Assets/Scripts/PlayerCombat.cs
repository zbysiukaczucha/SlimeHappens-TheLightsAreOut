using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{   
    [Header("##  GENERAL  ##")]
    private Animator anim;
    private BackgroundMusic bgMusic;
    private AudioSource heartbeat;
    private LayerMask bossLayer;
    private LayerMask enemyLayer;
    private GameManager gameManager;
    private Image healthBarFill;
    private Image ultimateFill1;
    private Image ultimateFill2;
    private Image ultimateBorder1;
    private Image ultimateBorder2;
    private Image healthBarBorder;
    private float targetHealthBarFill;
    private float healthBarFillSpeed = 20f;
    private GameObject killCounter;
    private GameObject waveNumber;
    private AudioLowPassFilter[] passFilters;
    private GameObject player;
    private PlayerMovementLO playerMovement; 
    private Rigidbody2D playerRigid;
    private Light2D globalLight;
    private Light2D playerLight;
    private float maxPlayerLight;
    private float minPlayerLight;

    private Light2D[] enemyEyes;
    private TrailRenderer[] enemyEyeTrails;

    [Header("##  RAGDOLL  ##")]
    private GameObject playerEye;
    private GameObject IKControls;
    private GameObject body_bottom;
    private Rigidbody2D rigidBody_bottom;
    private GameObject body_up;
    public Rigidbody2D rigidBody_up;
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
    
    [Header("##  COMBAT  ##")]
    private Transform attackPoint;
    private float nextAttackTime = 0;
    private int baseAttackDamage = 50;
    private int chosenAttack;
    
    [Header("##  PUBLIC VARIABLES  ##")]
    [ShowOnly] public int currentHealth;
    [ShowOnly] public int maxHealth;
    [ShowOnly] public int attackDamage;
    [ShowOnly] public float attackRate;

    [Header("## MOVEMENT ##")]
    public InputActionAsset inputActions;
    private InputAction attackAction;
    private InputAction powerAction;

    public long powerPoints;
    
    private float healthPercent = 1;



    void Awake()
    {
        heartbeat = GameObject.Find("Heartbeat").GetComponent<AudioSource>();
        bossLayer = 1 << LayerMask.NameToLayer("Boss");
        enemyLayer = 1 << LayerMask.NameToLayer("Enemy");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        healthBarFill = GameObject.Find("HealthBar/HealthBar Fill").GetComponent<Image>();
        healthBarBorder = GameObject.Find("HealthBar/HealthBar Border").GetComponent<Image>();
        ultimateFill1 = GameObject.Find("UltimateBar/Ultimate Fill").GetComponent<Image>();
        ultimateFill2 = GameObject.Find("UltimateBar2/Ultimate Fill").GetComponent<Image>();
        ultimateBorder1 = GameObject.Find("UltimateBar/Ultimate Border").GetComponent<Image>();
        ultimateBorder2 = GameObject.Find("UltimateBar2/Ultimate Border").GetComponent<Image>();
        player = GameObject.Find("Player");
        playerMovement = GetComponent<PlayerMovementLO>();
        anim = player.GetComponent<Animator>();
        powerPoints = 0;
        globalLight = GameObject.Find("GlobalLight2D").GetComponent<Light2D>();
        playerLight = transform.Find("Light2D").gameObject.GetComponent<Light2D>();
        maxPlayerLight = 0.6f;
        minPlayerLight = 0.01f;
        playerLight.intensity = maxPlayerLight;

        ultimateFill1.fillAmount = 0f;
        ultimateFill2.fillAmount = 0f;
        ultimateBorder1.color = new Color32(0, 102, 128, 255);
        ultimateBorder2.color = new Color32(102, 0, 128, 255);
        
        

        // RAGDOLL
        body_bottom = transform.Find("body_bottom").gameObject;
        rigidBody_bottom = body_bottom.GetComponent<Rigidbody2D>();
        body_up  = body_bottom.transform.Find("body_up").gameObject;
        rigidBody_up = body_up.GetComponent<Rigidbody2D>();
        playerEye = body_up.transform.Find("head/PlayerEye").gameObject;
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
        

        bgMusic = GameObject.Find("Audio Source").GetComponent<BackgroundMusic>();
        passFilters = GameObject.Find("Audio Source").GetComponentsInChildren<AudioLowPassFilter>();
        playerRigid = GetComponent<Rigidbody2D>();
        attackPoint = transform.Find("AttackPoint");
        

        // PUBLIC VARIABLES
        attackDamage = baseAttackDamage;
        maxHealth = 250;
        currentHealth = maxHealth;
        attackRate = 1.3f;
        
        healthBarFill.fillAmount = Mathf.Clamp01((float)currentHealth / maxHealth);
        targetHealthBarFill = healthBarFill.fillAmount;
        killCounter = GameObject.Find("Counter");
        waveNumber = GameObject.Find("WaveNumber");
        
        // Input System
        var playerActionMap = inputActions.FindActionMap("Player");
        attackAction = playerActionMap.FindAction("Attack");
        powerAction = playerActionMap.FindAction("Power");
        attackAction.Enable();
        powerAction.Enable();
    }
    




    void Update()
    {
        if (anim.GetBool("Finished_Falling_Touchdown") && Time.time >= nextAttackTime
            && attackAction.WasPerformedThisFrame())
        {
            Attack();
            inputActions.Disable();
            nextAttackTime = Time.time + 1 / attackRate;
        }

        if (anim.GetBool("Finished_Falling_Touchdown") && powerAction.WasPerformedThisFrame())
        {
            UsePower();
        }

        // SMOOTH HEALTH BAR
        if (Mathf.Abs(healthBarFill.fillAmount - targetHealthBarFill) > 0.001f)
        {
            healthBarFill.fillAmount = Mathf.SmoothStep(healthBarFill.fillAmount, targetHealthBarFill, Time.deltaTime * healthBarFillSpeed);
        }
    }
    




    void Attack()
    {
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
        {
            Vector3 pos = transform.position;
            pos.x += -player.transform.localScale.x * 5;
            transform.position = pos;
            playerMovement.DisableMovement();
            playerMovement.velocityX = 0f;
        }
        
        
        // SQUARE COLLIDER
        Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(attackPoint.position, new Vector2(2.2f, 0.5f), 0, enemyLayer);
        Collider2D[] bossesHit = Physics2D.OverlapBoxAll(attackPoint.position, new Vector2(2.2f, 0.5f), 0, bossLayer);

        
        // SOUNDS
        if(enemiesHit.Length == 0 && bossesHit.Length == 0)
            bgMusic.noHitPunch.Play();

        else if(enemiesHit.Length == 1 && bossesHit.Length == 0)
            bgMusic.enemyHitPunches[Random.Range(0, bgMusic.enemyHitPunches.Length)].Play();
        
        else if(enemiesHit.Length == 0 && bossesHit.Length == 1)
            bgMusic.bossHitPunches[Random.Range(0, bgMusic.bossHitPunches.Length)].Play();
        
        else
            bgMusic.multiHitPunch.Play();
        
        
        // ENEMY HIT
        foreach (Collider2D enemy in enemiesHit)
        {
            if (enemy is CapsuleCollider2D) continue;
            enemy.GetComponent<Enemy>().onHitBleed.Play();
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage, false);
            enemy.GetComponent<Enemy>().Knockback();
        }

        // BOSS HIT
        foreach (Collider2D boss in bossesHit)
        {
            if (boss is CapsuleCollider2D) continue;
            boss.GetComponent<Boss>().onHitBleed.Play();
            boss.GetComponent<Boss>().TakeDamage(attackDamage);
            boss.GetComponent<Boss>().Knockback();
        }
        
        // SCREEN LIGHT UP
        if (enemiesHit.Length > 0 || bossesHit.Length > 0)
            StartCoroutine(ScreenLightUp());
    }
    

    // DRAW HIT SQUARE
    private void OnDrawGizmosSelected()
{
    if (attackPoint == null) return;
        Gizmos.DrawWireCube(player.transform.position, new Vector3(5f, 0.8f, 1f));
    //Gizmos.DrawWireCube(attackPoint.position, new Vector3(2.2f, 0.5f));
    //Gizmos.DrawWireCube(attackPoint.position, new Vector3(3f, 0.8f, 1f));
}

    
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth > 0)
            bgMusic.PlayPlayerHurtSound();
        UpdateHealthBar();
            
        // DEAD
        if(currentHealth <= 0)
        {
            playerMovement.canMove = false;
            if(!bgMusic.playerDeathSound.isPlaying)
                bgMusic.PlayPlayerDeathSound();
            heartbeat.Stop();
            
            waveNumber.SetActive(false);
            killCounter.SetActive(false);
            
            healthBarFill.fillAmount = 0;
            healthBarBorder.color = new Color32(132, 36, 36, 0);

            playerRigid.bodyType = RigidbodyType2D.Static;

            playerEye.SetActive(false);
            playerMovement.enabled = false;
            anim.enabled = false;
            IKControls.SetActive(false);
            
            RagdollOn();
            
            gameManager.GameOver();
            enabled = false;
        }
    }
    

    public void HealPlayer(int heal)
    {
        currentHealth += heal;
        
        if(currentHealth > maxHealth)
            currentHealth = maxHealth;

        UpdateHealthBar();
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
    

    public void UsePower()
    {
        if (powerPoints <= 4)
            return;
        else if (powerPoints <= 9)
        {
            // USE BETTER ATTACK
            powerPoints -= 5;
            Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(attackPoint.position, new Vector2(3f, 0.8f), 0, enemyLayer);
            Collider2D[] bossesHit = Physics2D.OverlapBoxAll(attackPoint.position, new Vector2(3f, 0.8f), 0, bossLayer);
            
            // ENEMY HIT
            foreach (Collider2D enemy in enemiesHit)
            {
                if (enemy is CapsuleCollider2D) continue;
                enemy.GetComponent<Enemy>().onHitBleed.Play();
                enemy.GetComponent<Enemy>().TakeDamage(attackDamage * 2, true);
                enemy.GetComponent<Enemy>().Knockback();
            }
            
            // BOSS HIT
            foreach (Collider2D boss in bossesHit)
            {
                if (boss is CapsuleCollider2D) continue;
                boss.GetComponent<Boss>().onHitBleed.Play();
                boss.GetComponent<Boss>().TakeDamage(attackDamage * 2);
                boss.GetComponent<Boss>().Knockback();
            }
        }
        else
        {
            // USE SUPER ATTACK
            powerPoints -= 10;
            Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(player.transform.position, new Vector2(5f, 0.8f), 0, enemyLayer);
            Collider2D[] bossesHit = Physics2D.OverlapBoxAll(player.transform.position, new Vector2(5f, 0.8f), 0, bossLayer);

            // ENEMY HIT
            foreach (Collider2D enemy in enemiesHit)
            {
                if (enemy is CapsuleCollider2D) continue;
                enemy.GetComponent<Enemy>().onHitBleed.Play();
                enemy.GetComponent<Enemy>().TakeDamage(attackDamage * 3, true);
                enemy.GetComponent<Enemy>().Knockback();
            }
            
            // BOSS HIT
            foreach (Collider2D boss in bossesHit)
            {
                if (boss is CapsuleCollider2D) continue;
                boss.GetComponent<Boss>().onHitBleed.Play();
                boss.GetComponent<Boss>().TakeDamage(attackDamage * 3);
                boss.GetComponent<Boss>().Knockback();
            }
        }
        UpdateUltimateBar();
    }


    private void UpdateHealthBar()
    {
        healthPercent = Mathf.Clamp01((float)currentHealth / maxHealth);
        targetHealthBarFill = healthPercent;

        playerLight.intensity = Mathf.Lerp(minPlayerLight, maxPlayerLight, healthPercent);

        // Change enemy eye lights based on player health
        ChangeEyesIntensity();

        // LOW PASS FILTER
        foreach (var filter in passFilters)
            filter.cutoffFrequency = Mathf.SmoothStep(400, filter.cutoffFrequency, healthPercent);

        heartbeat.Stop();

        // YELLOW HEALTH BAR
        if (healthPercent < 0.6f && healthPercent > 0.3f)
        {
            healthBarFill.color = new Color32(224, 193, 22, 255);
            healthBarBorder.color = new Color32(224, 193, 22, 255);
        }

        // RED HEALTH BAR
        else if (healthPercent < 0.3f)
        {
            healthBarFill.color = new Color32(132, 36, 36, 255);
            healthBarBorder.color = new Color32(132, 36, 36, 255);
            heartbeat.Play();
        }

        // GREEN HEAELTH BAR
        else
        {
            healthBarFill.color = new Color32(30, 152, 18, 255);
            healthBarBorder.color = new Color32(30, 152, 18, 255);
        }
    }
    
    
    public void ChangeEyesIntensity()
    {
        enemyEyes = GameObject.Find("Enemy Spawner").GetComponentsInChildren<Light2D>();
        enemyEyeTrails = GameObject.Find("Enemy Spawner").GetComponentsInChildren<TrailRenderer>();
        foreach (var eye in enemyEyes)
            eye.intensity = Mathf.Lerp(0.2f, 3, healthPercent);
        foreach (var trail in enemyEyeTrails)
        {
            Gradient gradient = trail.colorGradient;
            
            GradientAlphaKey[] newAlphas = new GradientAlphaKey[] {
                new GradientAlphaKey(1 - (1 - healthPercent) * 1.3f, 0.0f), // Alpha at Start of trail
                new GradientAlphaKey(1 - (1 - healthPercent) * 1.3f, 1.0f)  // Alpha at End of trail
            };

            gradient.SetKeys(gradient.colorKeys, newAlphas);
            trail.colorGradient = gradient;
        }
    }
    

    
    public void UpdateUltimateBar()
    {
        float ultimatePercent1 = 0f, ultimatePercent2 = 0f;
        if(powerPoints < 5)
        {
            ultimatePercent1 = Mathf.Clamp01((float)powerPoints / 5);
            ultimatePercent2 = 0f;
        }
        else
        {
            ultimatePercent1 = 1f;
            ultimatePercent2 = Mathf.Clamp01(((float)powerPoints - 5) / 5);
        }
        
        ultimateFill1.fillAmount = ultimatePercent1;
        ultimateFill2.fillAmount = ultimatePercent2;
    
        // KOLORY DLA PASKA 1 (Atak Specjalny)
        ultimateFill1.color = new Color32(0, 204, 255, 255); // Jasny Cyjan
        ultimateBorder1.color = new Color32(0, 102, 128, 255); // Ciemny Cyjan
        

        // KOLORY DLA PASKA 2 (Super Atak)
        ultimateFill2.color = new Color32(204, 0, 255, 255); // Intensywny Fiolet
        ultimateBorder2.color = new Color32(102, 0, 128, 255); // Ciemny Fiolet
        
        // Sygnalizacja gotowości (Opcjonalnie: zmiana koloru ramki na jaskrawy, gdy pasek jest pełny)
        if (ultimatePercent1 >= 1f)
        {
            // Pasek 1 pełny: ramka może jaśnieć lub pulsować
            ultimateBorder1.color = new Color32(255, 255, 255, 255); 
        }
        
        if (ultimatePercent2 >= 1f)
        {
            // Pasek 2 pełny: ramka może stać się jaskrawoczerwona/złota dla maksymalnego efektu
            ultimateBorder2.color = new Color32(255, 255, 255, 255); 
        }

    }
    
    // ENUMERATORS

    private IEnumerator ScreenLightUp()
    {
        globalLight.intensity = 0.13f;
        
        yield return new WaitForSeconds(0.1f);
        
        globalLight.intensity = 0;
    }
    
    
    private IEnumerator FreezeWhenKnockbacked()
    {
        // if(playerMovement.isGrounded())
        //     playerRigid.velocity = Vector2.zero;
        playerMovement.enabled = false;
        
        yield return new WaitForSeconds(0.5f);
        
        playerMovement.enabled = true;
    }
}
