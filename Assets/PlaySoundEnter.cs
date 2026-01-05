using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PlaySoundEnter : StateMachineBehaviour
{
    [SerializeField]
    private SoundType sound;

    //public bool playOnUpdate = false;
    public bool playConstant = false; 
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

        if(playConstant)
        {
            AudioManager.PlayConstant(sound, pitch, volume);
        }
        else
        {
            AudioManager.PlaySound(sound, pitch, volume);

        }
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playConstant)
        {
            AudioManager.StopConstant();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    /*override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioManager.PlaySound(sound, 1, volume);
        *//*if (playOnUpdateRandom)
        {
            AudioManager.PlaySoundRandom(sounds, 1, volume);
        }*//*
        
    }*/
}
