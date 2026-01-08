using ShineHappens;
using UnityEngine;
using UnityEngine.Audio;
using System;

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
    SnailDodgeBack,
    SnailMove,
    SnailUltimate,
    YouDied,
    Yay,
    GemCut
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class AudioManager : MonoBehaviour
{

    [SerializeField]
    private SoundList[] soundList;

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
        //instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        instance.audioSource.PlayOneShot(randomClip, volume);
    }


#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundList, names.Length);
        for(int i =0; i < soundList.Length; i++)
        {
            soundList[i].name = names[i];
        }
    }
#endif
}

[Serializable]
public struct SoundList
{
    public AudioClip[] Sounds { get => sounds; }
    [HideInInspector]
    public string name;
    [SerializeField] 
    private AudioClip[] sounds;
}

