using ShineHappens;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    AudioClip enchantingFinishedClip;
    [SerializeField]
    AudioClip gemUnstableClip;
    [SerializeField]
    AudioClip gemDisruptedClip;
    [SerializeField]
    AudioClip gemWaveringClip;
    [SerializeField]
    AudioClip gemStableClip;

    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayGemEnchantingClip(AudioSource audioSource, GemStabilityLevel gemLevel, float pitch=1f)
    {
        switch (gemLevel)
        {
            case GemStabilityLevel.Stable:
                audioSource.clip = gemStableClip;
                break;
            case GemStabilityLevel.Wavering:
                audioSource.clip = gemWaveringClip;
                break;
            case GemStabilityLevel.Disrupted:
                audioSource.clip = gemDisruptedClip;
                break;
            case GemStabilityLevel.Unstable:
                audioSource.clip = gemUnstableClip;
                break;
        }
        audioSource.pitch = pitch;
        audioSource.Play();
    }

    public void PlayFinishedEnchantingClip()
    {
        audioSource.clip = enchantingFinishedClip;
        audioSource.Play();
    }
}
