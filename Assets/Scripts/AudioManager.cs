using ShineHappens;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundType
{
    GemPowerUp,
    GemDisrupted,
    GemPoof,
    EnchantingFinished,
    FrogArmSmash,
    FrogFrontCharge,
    FrogTongueSlam,
    FrogTongueSnap,
    FrogTongueSweep,
    FrogRoll,
    FrogJumping,
    SnailLightAttack1,
    SnailLightAttack2,
    SnailLightAttack3alt,
    SnailDodgeBack
}

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    /*[SerializeField]
    AudioClip enchantingFinishedClip;
    [SerializeField]
    AudioClip gemUnstableClip;
    [SerializeField]
    AudioClip gemDisruptedClip;
    [SerializeField]
    AudioClip gemWaveringClip;
    [SerializeField]
    AudioClip gemStableClip;

    [SerializeField]
    AudioClip frogThumpAttackClip;
    [SerializeField]
    AudioClip frogTongueAttackClip;
    [SerializeField]
    AudioClip frogFlipAttackClip;*/



    [SerializeField]
    private AudioClip[] soundList;

    private static AudioManager instance;
    AudioSource audioSource;

    public void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType sound, float pitch = 1, float volume = 1)
    {
        instance.audioSource.pitch= pitch;
        instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
        //instance.audioSource.pitch = 1;
    }

    public static void PlayConstant(SoundType sound, float pitch = 1, float volume = 1)
    {
        instance.audioSource.clip = instance.soundList[(int)sound];
        instance.audioSource.pitch = pitch;
        instance.audioSource.volume = volume;
        instance.audioSource.loop = true;
        instance.audioSource.Play();
    }

    public static void StopConstant()
    {
        instance.audioSource.Stop();
        instance.audioSource.clip = null;
        instance.audioSource.loop = false;
    }


}
