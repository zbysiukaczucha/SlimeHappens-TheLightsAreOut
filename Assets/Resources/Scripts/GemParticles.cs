using UnityEngine;

public class GemParticles : MonoBehaviour
{
    [SerializeField]
    ParticleSystem gemSlowStable;

    [SerializeField]
    ParticleSystem gemSuddenStable;

    [SerializeField]
    ParticleSystem gemDisrupted;

    [SerializeField]
    ParticleSystem gemUnstable;

    public void PlayGemSlowStable()
    {
        gemSlowStable.Play();
    }
    public void PlayGemSuddenStable()
    {
        gemSuddenStable.Play();
    }
    public void PlayGemDisrupted()
    {
        gemDisrupted.Play();
    }
    public void PlayGemUnstable()
    {
        gemUnstable.Play();
    }

}
