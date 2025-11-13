using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.VisualScripting;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FaceTarget", story: "Face [target] over [duration] seconds", category: "Action", id: "c9ccfaf8fc35b227776dc014bb00457a")]
public partial class FaceTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> Duration;
    [CreateProperty] private float m_Progress;
    [CreateProperty] private Quaternion m_StartRotation;
    private Quaternion m_EndRotation;
    protected override Status OnStart()
    {
        if (Target.Value == null)
        {
            LogFailure("No Target set");
            return Status.Failure;
        }
        
        Transform selfTransform = this.GameObject.transform;
        Vector3 flatTargetPos = Target.Value.transform.position;
        if (Duration.Value <= 0.0f)
        {
            // this.GameObject.transform.LookAt(flatTargetPos, selfTransform.up );
            
            Quaternion targetRotation = Quaternion.LookRotation(flatTargetPos - selfTransform.position, selfTransform.up);

            this.GameObject.transform.rotation = Quaternion.RotateTowards(selfTransform.rotation, targetRotation, 1f);
        }
        
        
        // m_StartRotation = selfTransform;
        flatTargetPos.y = 0.0f;
        m_EndRotation = Quaternion.LookRotation(flatTargetPos - this.GameObject.transform.position);
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

