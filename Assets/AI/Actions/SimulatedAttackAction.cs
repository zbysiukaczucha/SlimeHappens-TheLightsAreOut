using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SimulatedAttack", story: "Simulates Attack Over [duration] and sets [isAttacking]", category: "Action", id: "86ecbb91d4ee78f0877d2dd1905600ad")]
public partial class SimulatedAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<float> Duration;
    [SerializeReference] public BlackboardVariable<bool> IsAttacking;
    private float m_StartTime;

    protected override Status OnStart()
    {
        IsAttacking.Value = true;
        m_StartTime = Time.realtimeSinceStartup;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Time.realtimeSinceStartup - m_StartTime >= Duration.Value)
        {
            IsAttacking.Value = false;
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

