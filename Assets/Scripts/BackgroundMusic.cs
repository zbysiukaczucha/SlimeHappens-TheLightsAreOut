using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    [ShowOnly] public AudioSource[] backgroundMusics;
    [ShowOnly] public AudioSource noHitPunch;
    [ShowOnly] public AudioSource[] enemyHitPunches;
    [ShowOnly] public AudioSource[] bossHitPunches;
    [ShowOnly] public AudioSource multiHitPunch;
    [ShowOnly] public AudioSource[] ambientSounds;
    [ShowOnly] public AudioSource[] playerHurtSounds;
    [ShowOnly] public AudioSource[] enemyDeathSounds;
    [ShowOnly] public AudioSource[] bossDeathSounds;
    [ShowOnly] public AudioSource playerDeathSound;
    [ShowOnly] public AudioSource dashSound;
    private int selectedAudio;
    private int previousAudio;
    private bool isPlaying;
    private GameObject mainMenuMusic;
    private AudioSource mainMenuMusicSource;
    
    private GameManager gameManager;

    
    void Start()
    {
        // Allow start from game
        mainMenuMusic = GameObject.Find("MainMenuMusic");
        if(mainMenuMusic != null)
            mainMenuMusicSource = mainMenuMusic.GetComponent<AudioSource>();
        
        backgroundMusics = GameObject.Find("BackgroundMusic").GetComponentsInChildren<AudioSource>();
        noHitPunch = GameObject.Find("NoHitPunch").GetComponent<AudioSource>();
        enemyHitPunches = GameObject.Find("EnemyHit").GetComponentsInChildren<AudioSource>();
        bossHitPunches = GameObject.Find("BossHit").GetComponentsInChildren<AudioSource>();
        multiHitPunch = GameObject.Find("MultipleHits").GetComponent<AudioSource>();
        ambientSounds = GameObject.Find("Ambients").GetComponentsInChildren<AudioSource>();
        playerHurtSounds = GameObject.Find("PlayerHurt").GetComponentsInChildren<AudioSource>();
        playerDeathSound = GameObject.Find("PlayerDeath").GetComponent<AudioSource>();
        enemyDeathSounds = GameObject.Find("EnemyDeath").GetComponentsInChildren<AudioSource>();
        bossDeathSounds = GameObject.Find("BossDeath").GetComponentsInChildren<AudioSource>();
        dashSound = GameObject.Find("DashSound").GetComponent<AudioSource>();
        StartCoroutine(AmbientSound());
        StartCoroutine(Thunder());
        selectedAudio = Random.Range(0, backgroundMusics.Length);
        previousAudio = selectedAudio;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            
        // Allow start from game
        if(mainMenuMusicSource != null)    
            StartCoroutine(StopMainMenuMusic());

        backgroundMusics[selectedAudio].Play();
    }
    




    void Update()
    {
        
        isPlaying = false;

        foreach (AudioSource audio in backgroundMusics)
        {
            if(audio.isPlaying)
                isPlaying = true;
        }

        if (!isPlaying)
        {
            selectedAudio = Random.Range(0, backgroundMusics.Length);
            
            while(selectedAudio == previousAudio)
                selectedAudio = Random.Range(0, backgroundMusics.Length);
                
            previousAudio = selectedAudio;
            backgroundMusics[selectedAudio].Play();
            backgroundMusics[selectedAudio].volume = PlayerPrefs.GetFloat("volume");
        }
    }
    
    public void PlayPlayerHurtSound(){
        playerHurtSounds[Random.Range(0, playerHurtSounds.Length)].Play();
    }
    public void PlayEnemyDeathSound(){
        enemyDeathSounds[Random.Range(0, enemyDeathSounds.Length)].Play();
    }
    public void PlayBossDeathSound(){
        bossDeathSounds[Random.Range(0, bossDeathSounds.Length)].Play();
    }
    
    public void PlayPlayerDeathSound(){
        playerDeathSound.Play();
    }


    IEnumerator AmbientSound()
    {
        int cooldown = Random.Range(5, 21);

        yield return new WaitForSeconds(cooldown);

        ambientSounds[Random.Range(0,ambientSounds.Length)].Play();
        StartCoroutine(AmbientSound());
    }
    
    IEnumerator Thunder()
    {
        int cooldown = Random.Range(10, 21);
        
        yield return new WaitForSeconds(cooldown);
        
        StartCoroutine(gameManager.Lightning());

        StartCoroutine(Thunder());
    }
    

    IEnumerator StopMainMenuMusic(){
        yield return new WaitForSeconds(0.3f);
        mainMenuMusicSource.volume *= 0.6f;
        yield return new WaitForSeconds(0.3f);
        mainMenuMusicSource.volume *= 0.6f;
        yield return new WaitForSeconds(0.3f);
        mainMenuMusicSource.volume *= 0.6f;
        Destroy(mainMenuMusic);
    }
}
