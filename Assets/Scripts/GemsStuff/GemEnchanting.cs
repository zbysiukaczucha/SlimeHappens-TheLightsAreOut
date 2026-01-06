using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShineHappens
{
    public class GemAnimationScript : MonoBehaviour
    {
        [SerializeField]
        public GemParticles gemParticles;

        public Animator animator;
        private GemStabilityLevel currentLevel;

        //AudioSource audioSource;
        //AudioManager audioManager;
        float pitch = 1;

        float comboPitchChange = 0.08f;

        private void Start()
        {
            //audioSource = GetComponent<AudioSource>();
            animator = GetComponent<Animator>();
            currentLevel = GemStabilityLevel.Stable;
            //audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        }

        public void switchAnimation(GemStabilityLevel level)
        {
            switch (currentLevel)
            {
                case GemStabilityLevel.Stable:
                    animator.SetBool("isStable", false);
                    break;
                case GemStabilityLevel.Wavering:
                    animator.SetBool("isWavering", false);
                    break;
                case GemStabilityLevel.Disrupted:
                    animator.SetBool("isDisrupted", false);
                    break;
                case GemStabilityLevel.Unstable:
                    animator.SetBool("isUnstable", false);
                    break;
            }

            playParticles(currentLevel, level);
            //GemStabilityLevel clipLevel = GemStabilityLevel.Stable;
            SoundType soundToPlay = SoundType.GemPowerUp;

            switch (level)
            {
                case GemStabilityLevel.Stable:
                    animator.SetBool("isStable", true);
                    if (currentLevel == GemStabilityLevel.Stable || currentLevel == GemStabilityLevel.Wavering)
                        pitch += comboPitchChange;
                    else 
                        pitch = 1f;
                    currentLevel = GemStabilityLevel.Stable;
                    soundToPlay = SoundType.GemPowerUp;
                    break;
                case GemStabilityLevel.Wavering:
                    animator.SetBool("isWavering", true);
                    if (currentLevel == GemStabilityLevel.Stable || currentLevel == GemStabilityLevel.Wavering)
                        pitch += comboPitchChange;
                    else
                        pitch = 1f;
                    currentLevel = GemStabilityLevel.Wavering;
                    soundToPlay = SoundType.GemPowerUp;
                    break;
                case GemStabilityLevel.Disrupted:
                    if (currentLevel == GemStabilityLevel.Unstable)
                    {
                        pitch = 1f;
                        soundToPlay = SoundType.GemPowerUp;
                    }
                    else
                    {
                        pitch = Random.Range(0.9f, 1.2f);
                        soundToPlay = SoundType.GemDisrupted;
                    }
                    animator.SetBool("isDisrupted", true);
                    currentLevel = GemStabilityLevel.Disrupted;
                    break;
                case GemStabilityLevel.Unstable:
                    animator.SetBool("isUnstable", true);
                    currentLevel = GemStabilityLevel.Unstable;
                    soundToPlay = SoundType.GemPoof;
                    pitch = Random.Range(0.9f, 1.2f);
                    break;
            }
            AudioManager.PlaySound(soundToPlay, pitch);
            //audioManager.PlayGemEnchantingClip(audioSource, clipLevel, pitch);
            //audioSource.Play();
        }

        void playParticles(GemStabilityLevel previousState, GemStabilityLevel newState)
        {
            switch (newState)
            {
                case GemStabilityLevel.Stable:
                case GemStabilityLevel.Wavering:
                    if (previousState == GemStabilityLevel.Unstable)
                    {
                        gemParticles.PlayGemSuddenStable();
                    }
                    else
                    {
                        gemParticles.PlayGemSlowStable();
                    }
                    break;
                case GemStabilityLevel.Disrupted:
                    if (previousState == GemStabilityLevel.Unstable)
                    {
                        gemParticles.PlayGemSlowStable();
                    }
                    else
                    {
                        gemParticles.PlayGemDisrupted();
                    }
                    break;
                case GemStabilityLevel.Unstable:
                    gemParticles.PlayGemUnstable();
                    break;
            }
        }

    }
}