using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private UltimateGameManager ultimateGameManager;
    [Header("##  SCREENS  ##")]
    private GameObject gameOverScreen;

    [Header("##  HEAL  ##")]
    [ShowOnly] public int healAmount;
    [ShowOnly] public int healUpChance;

    [ShowOnly] public bool gameOver = false;
    private CanvasGroup gameOverCanvas;
    private bool gameOverFadeIn = false;
    private Button continueButton;
    private TextMeshProUGUI yourScore;
    private TextMeshProUGUI highScore;
    private TMP_Text killCount;
    private Animator fadeAnim;
    
    [ShowOnly] public AudioSource[] thunderSounds;
    private Light2D globalLight;
    

    public long score;

    void Awake()
    {
        ultimateGameManager = FindFirstObjectByType<UltimateGameManager>();
        killCount = GameObject.Find("Counter").GetComponent<TMP_Text>();
        yourScore = GameObject.Find("YourScore").GetComponent<TextMeshProUGUI>();
        highScore = GameObject.Find("HighScore").GetComponent<TextMeshProUGUI>();
        fadeAnim = GameObject.Find("Crossfade").GetComponent<Animator>();
        healAmount = 50;
        healUpChance = 5;
        score = 0;
        gameOverScreen = GameObject.Find("GameOverScreen");
        gameOverScreen.SetActive(false);
        gameOverCanvas = gameOverScreen.GetComponent<CanvasGroup>();
        continueButton = gameOverScreen.GetComponentsInChildren<Button>()[0];
        
        thunderSounds = GameObject.Find("Thunders").GetComponentsInChildren<AudioSource>();
        globalLight = GameObject.Find("GlobalLight2D").GetComponent<Light2D>();
    }

    void Start()
    {
        continueButton.enabled = false;
    }

    void Update()
    {
        // GAME OVER
        if (gameOverFadeIn)
        {
            gameOverCanvas.alpha += Time.deltaTime * 1.5f;
            
            if (gameOverCanvas.alpha >= 0.3f)
            {
                continueButton.enabled = true;
            }
            if (gameOverCanvas.alpha >= 1)
                gameOverFadeIn = false;
        }
    }
    
    public void GameOver()
    {
        if(int.Parse(killCount.text) > PlayerPrefs.GetInt("HighScore", 0))
            PlayerPrefs.SetInt("HighScore", int.Parse(killCount.text));

        highScore.text = "High Score: " + PlayerPrefs.GetInt("HighScore", 0).ToString();
        yourScore.text = "Your Score: " + killCount.text;
        
        ultimateGameManager.score = int.Parse(killCount.text);

        gameOver = true;
        gameOverScreen.SetActive(true);
        StartCoroutine(waitForGameOverFadeIn());
        // player.SetActive(false);
        // spawner.SetActive(false);
    }

    public void ExitNightmare()
    {
        StartCoroutine(Continue());
        fadeAnim.SetTrigger("In");
    }
    
    IEnumerator waitForGameOverFadeIn()
    {   
        yield return new WaitForSeconds(0.1f);
        gameOverFadeIn = true;
    }
    
    IEnumerator Continue()
    {
        yield return new WaitForSeconds(0.9f);
        Debug.Log("przejście do realnego świata");
        SceneManager.LoadScene("Level");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        UltimateGameManager.Instance.enableWallBreak = false;
    }
    
    public IEnumerator Lightning()
    {
        thunderSounds[Random.Range(0, thunderSounds.Length)].Play();

        int flickerCount = Random.Range(3, 6);
        float maxIntensity = Random.Range(0.3f, 0.7f);
        
        for (int i = 0; i < flickerCount; i++)
        {
            // Randomly choose a brightness for this specific flicker
            globalLight.intensity = Random.Range(maxIntensity * 0.5f, maxIntensity);

            // Wait a very short random time (creates the "fast" jagged look)
            yield return new WaitForSeconds(Random.Range(0.02f, 0.08f));
            
            // Turn light off briefly between flickers to create the strobe effect
            globalLight.intensity = 0f;
            yield return new WaitForSeconds(Random.Range(0.02f, 0.08f));
        }
        
        // 3. The "Main" Strike (Brightest flash)
        globalLight.intensity = maxIntensity;
        yield return new WaitForSeconds(0.08f);

        // 4. The "Smooth" Fade Out (prevents it from looking like a glitch)
        float fadeDuration = 0.2f;
        float currentIntensity = maxIntensity;
        
        while (currentIntensity > 0)
        {
            currentIntensity -= Time.deltaTime / fadeDuration; // Reduce intensity over time
            globalLight.intensity = currentIntensity;
            yield return null; // Wait for the next frame
        }
        
        // Ensure it's fully off
        globalLight.intensity = 0;
    }
}
