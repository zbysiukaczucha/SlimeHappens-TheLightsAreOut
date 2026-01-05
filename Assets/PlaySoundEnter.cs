using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PlaySoundEnter : StateMachineBehaviour
{
    [SerializeField]
    private SoundType sound;

    //public bool playOnUpdate = false;
    public bool randomizePitch = false;

    [SerializeField, Range(0, 1)]
    private float volume = 1;
    private float pitch = 1;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (randomizePitch)
        {
            pitch = Random.Range(0.9f, 1f);
            Debug.Log(pitch);
        }

        AudioManager.PlaySound(sound, pitch, volume);
    }

}
