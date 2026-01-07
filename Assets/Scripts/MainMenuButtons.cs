using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    private AudioSource mainMenuMusic;
    private Animator anim;
    private Animator crossfadeAnim;
    private GameObject player;
    [SerializeField] private AudioMixer mixer;
    private GameObject music;
    




    void Start()
    {
        mixer.SetFloat("Volume",Mathf.Log10(PlayerPrefs.GetFloat("volume")) * 20);
        mixer.SetFloat("Music",Mathf.Log10(PlayerPrefs.GetFloat("musicvolume")) * 20);
        mixer.SetFloat("SFX",Mathf.Log10(PlayerPrefs.GetFloat("effectsvolume")) * 20);
    }
    




    void Awake()
    {
        music = GameObject.Find("MainMenuMusic");
        DontDestroyOnLoad(music);
        mainMenuMusic = GameObject.Find("MainMenuMusic").GetComponent<AudioSource>();
        player = GameObject.Find("Player");
        crossfadeAnim = GameObject.Find("Crossfade").GetComponent<Animator>();
        anim = player.GetComponent<Animator>();
        crossfadeAnim.SetTrigger("In");
        if(mainMenuMusic != null)
            mainMenuMusic.volume = PlayerPrefs.GetFloat("volume");
    }
    




    public void StartButton()
    {
        anim.SetTrigger("Arise");
        StartCoroutine(ChangeSceneToGame());
        crossfadeAnim.SetTrigger("Out");
    }
    

    public void SettingsButton()
    {
        StartCoroutine(ChangeSceneToSettings());
        crossfadeAnim.SetTrigger("Out");
    }
    

    public void ExitButton()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
    
    
    
    
    
//  IENUMERATORS

    private IEnumerator ChangeSceneToGame()
    {
        // Fade out music
        if(mainMenuMusic != null)
        {
            float startVolume = mainMenuMusic.volume;
            float duration = 0.9f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                mainMenuMusic.volume = Mathf.Lerp(startVolume, 0, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            mainMenuMusic.volume = 0;
        }
        yield return new WaitForSeconds(0.9f);
        SceneManager.LoadScene("StartingScene");
    }
    
    
    private IEnumerator ChangeSceneToSettings()
    {
        yield return new WaitForSeconds(0.9f);
        SceneManager.LoadScene("Settings");
    }
}
