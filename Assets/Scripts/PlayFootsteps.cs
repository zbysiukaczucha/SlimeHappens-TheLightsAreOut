using UnityEngine;

public class PlayFootsteps : MonoBehaviour
{
    BossAISensor frogSensor;

    private void Start()
    {
        frogSensor = GetComponent<BossAISensor>();
    }

    public void PlayFrogJump()
    {
        if(frogSensor.bossDistance > 60)
        {
            return;
        }
        AudioManager.PlaySound(SoundType.FrogJumping, 1, (60 - frogSensor.bossDistance)/60);
    }

    public void PlaySnailMove()
    {
        //AudioManager.PlaySound(SoundType.SnailMove);
        print("Playing");
    }
}
