using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Random = UnityEngine.Random;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack_PROWIZORKA", story: "[Agent] attacks badly and sets [IsAttacking] using [AttacksPossible]", category: "Action", id: "abc8e7bd59cf89b2f45add8ae75b3e72")]
public partial class AttackProwizorkaAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<bool> IsAttacking;
    [SerializeReference] public BlackboardVariable<List<string>> AttacksPossible;
    private Animator animator;

    protected override Status OnStart()
    {
        animator = Agent.Value.GetComponent<Animator>();
        
        int attack = Random.Range(0, AttacksPossible.Value.Count - 1);
        animator.SetBool("IsAttacking", true);
        animator.CrossFade(AttacksPossible.Value[attack], 0.2f);
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (animator.GetBool("IsAttacking"))
        {
            return Status.Running;
        }
        
        IsAttacking.Value = false;
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

