using UnityEngine;

public class ResetIsAttacking : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerCombat playerCombat = animator.GetComponent<PlayerCombat>();
        if (playerCombat != null)
        {
            playerCombat.OnAttackFinish();
        }
    }
}
