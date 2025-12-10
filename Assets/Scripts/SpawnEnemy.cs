using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

using UnityEngine.InputSystem;

public class SpawnEnemy : MonoBehaviour
{
    [Header("##  PREFABS  ##")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject bossPrefab;

    [SerializeField] private GameObject fireflyPrefab;
    
    [Header("##  GAME MANAGER  ##")]
    private GameManager gameManager;
    [SerializeField] private AudioSource[] audioSource;

    [Header("##  PUBLIC VARIABLES  ##")]
    private TMP_Text killCount;
    [ShowOnly] public float spawnInterval;
    [ShowOnly] public float spawnIntervalOffset;
    [ShowOnly] public float spawnIntervalInit;

    private WaveNumberController waveController;
    
    private float timer = 2;

    private float spawnPoint = 37;
    public InputActionAsset inputActions;
    private InputAction nextWaveAction;
    
    private int waveNumber = 1;
    private int enemiesToSpawn;
    private int enemiesSpawned = 0;
    private int enemiesAlive = 0;

    [SerializeField] private float timeBetweenWaves = 5f;
    private float waveTimer = 0f;

    private bool waitingForNextWave = false;

    private PlayerCombat playerCombat;


    void Awake()
    {
        var playerActionMap = inputActions.FindActionMap("Player");
        nextWaveAction = playerActionMap.FindAction("Start Wave");
        nextWaveAction.Enable();
    }


    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        killCount = GameObject.Find("Counter").GetComponent<TMP_Text>();
        spawnInterval = 1.5f;
        spawnIntervalInit = 1.5f;
        spawnIntervalOffset = 0.5f;
        enemiesToSpawn = Random.Range(8, 12);

        waveController = GameObject.Find("WaveNumber").GetComponent<WaveNumberController>();
        playerCombat = GameObject.Find("Player").GetComponent<PlayerCombat>();
        
        Debug.Log("Wave " + waveNumber + " started");
        StartCoroutine(gameManager.Lightning());
    }
    




    void Update()
    {
        if (gameManager.gameOver) return;

        if (waitingForNextWave)
        {
            waveTimer += Time.deltaTime;
            if (waveTimer >= timeBetweenWaves)
            {
                waitingForNextWave = false;
                waveTimer = 0f;
                StartWave();
                Debug.Log("Wave " + waveNumber + " started");
                StartCoroutine(gameManager.Lightning());
            }
            return;
        }

        if (enemiesSpawned < enemiesToSpawn)
        {


            if (timer < spawnInterval)
            {
                timer += Time.deltaTime;
            }
            else
            {
                spawnInterval = Random.Range(spawnIntervalInit - spawnIntervalOffset, spawnIntervalInit + spawnIntervalOffset);
                SpawnEnemyInstance();
                enemiesSpawned++;
                enemiesAlive++;
                timer = 0f;
            }
            if(waveNumber % 5 == 0 && waveNumber != 0 && enemiesSpawned == enemiesToSpawn)
            {
                for (int i = 0; i < waveNumber/5; i++)
                {
                    SpawnBossInstance();
                    enemiesSpawned++;
                }
                    
            }
        }

        // ALL ENEMIES SPAWNED AND DEFEATED
        if (enemiesSpawned == enemiesToSpawn && enemiesAlive == 0)
        {
            
            // NEXT WAVE
            waitingForNextWave = true;
            waveNumber++;
            enemiesToSpawn += 3;
            
            waveController.ChangeWave(waveNumber-1);
        }
    }


    public void OnEnemyKilled()
    {
        if (Random.Range(0, 100) < 5)
        {
            if (Random.Range(0, 10) % 2 == 1)
            Instantiate(fireflyPrefab, new Vector2(-spawnPoint, Random.Range(3f, 5f)), transform.rotation).transform.parent = transform;
            else
            Instantiate(fireflyPrefab, new Vector2(spawnPoint, Random.Range(3f, 5f)), transform.rotation).transform.parent = transform;
        }
        enemiesAlive--;
    }

    void StartWave()
    {
        enemiesSpawned = 0;
        timer = 0f;
    }

    



    
    void SpawnEnemyInstance()
    {
        // ODD NUMBERS - LEFT SPAWN
        if (Random.Range(0, 10) % 2 == 1)
            Instantiate(enemyPrefab, new Vector2(-spawnPoint, transform.position.y), transform.rotation).transform.parent = transform;
        
        // EVEN NUMBERS - RIGHT SPAWN
        else
            Instantiate(enemyPrefab, new Vector2(spawnPoint, transform.position.y), transform.rotation).transform.parent = transform;

        // To spawn just one enemy
        // this.enabled = false;
    }
    
    
    void SpawnBossInstance()
    {
        // ODD NUMBERS - LEFT SPAWN
        if (Random.Range(0, 10) % 2 == 1)
            Instantiate(bossPrefab, new Vector2(-spawnPoint, transform.position.y), transform.rotation).transform.parent = transform;
        
        // EVEN NUMBERS - RIGHT SPAWN
        else
            Instantiate(bossPrefab, new Vector2(spawnPoint, transform.position.y), transform.rotation).transform.parent = transform;
        
        audioSource[Random.Range(0, audioSource.Length)].Play();
        // To spawn just one boss
        // this.enabled = false;

        playerCombat.ChangeEyesIntensity();
    }
}
