using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private AudioMixer mainMixer;
    private Slider sliderMain;
    private Slider sliderEffects;
    private Slider sliderMusic;
    private Animator crossfadeAnim;
    



    void Awake()
    {
        sliderMain = GameObject.Find("SliderMain").GetComponent<Slider>();
        sliderMain.value = PlayerPrefs.GetFloat("volume");
        sliderEffects = GameObject.Find("SliderEffects").GetComponent<Slider>();
        sliderEffects.value = PlayerPrefs.GetFloat("effectsvolume");
        sliderMusic = GameObject.Find("SliderMusic").GetComponent<Slider>();
        sliderMusic.value = PlayerPrefs.GetFloat("musicvolume");
        crossfadeAnim = GameObject.Find("Crossfade").GetComponent<Animator>();

    }

    
    


    public void SetMainVolume(float volume)
    {
        mainMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("volume", volume);
    }
    
    public void SetEffectsVolume(float volume)
    {
        mainMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("effectsvolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        mainMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicvolume", volume);
    }

    public void MainMenu()
    {
        crossfadeAnim.SetTrigger("In");
        StartCoroutine(ChangeSceneToMain());
    }
    
    private IEnumerator ChangeSceneToMain()
    {
        yield return new WaitForSeconds(0.9f);
        Destroy(GameObject.Find("MainMenuMusic"));
        SceneManager.LoadScene("Main");
    }
}
