using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("##  SCREENS  ##")]
    private GameObject gameOverScreen;

    [Header("##  HEAL  ##")]
    [ShowOnly] public int healAmount;
    [ShowOnly] public int healUpChance;

    [ShowOnly] public bool gameOver = false;
    private CanvasGroup gameOverCanvas;
    private bool gameOverFadeIn = false;
    private Button restartButton;
    private Button mainMenuButton;
    private TextMeshProUGUI yourScore;
    private TextMeshProUGUI highScore;
    private TMP_Text killCount;
    private Animator fadeAnim;

    public long score;

    void Awake()
    {
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
        restartButton = gameOverScreen.GetComponentsInChildren<Button>()[0];
        mainMenuButton = gameOverScreen.GetComponentsInChildren<Button>()[1];
    }

    void Start()
    {
        restartButton.enabled = false;
        mainMenuButton.enabled = false;
    }

    void Update()
    {
        // GAME OVER
        if (gameOverFadeIn)
        {
            gameOverCanvas.alpha += Time.deltaTime * 1.5f;
            
            if (gameOverCanvas.alpha >= 0.3f)
            {
                restartButton.enabled = true;
                mainMenuButton.enabled = true;
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

        gameOver = true;
        gameOverScreen.SetActive(true);
        StartCoroutine(waitForGameOverFadeIn());
        // player.SetActive(false);
        // spawner.SetActive(false);
    }

    public void Restart()
    {
        StartCoroutine(RestartGame());
        fadeAnim.SetTrigger("In");
    }
    
    public void MainMenu()
    {
        StartCoroutine(TransitionToMainMenu());
        fadeAnim.SetTrigger("In");
    }
    
    IEnumerator waitForGameOverFadeIn()
    {   
        yield return new WaitForSeconds(0.1f);
        gameOverFadeIn = true;
    }
    
    IEnumerator TransitionToMainMenu()
    {   
        yield return new WaitForSeconds(0.9f);
        SceneManager.LoadScene("Main");
    }

    IEnumerator RestartGame()
    {   
        yield return new WaitForSeconds(0.9f);
        SceneManager.LoadScene("Game");
    }
}
