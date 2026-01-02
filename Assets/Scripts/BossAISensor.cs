using System;
using UnityEngine;
using Unity.Behavior;

public class BossAISensor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private BlackboardReference m_Blackboard;
    [SerializeField] private uint m_UpdateInterval = 10;
    private uint m_timer = 10;

    void Start()
    {
        BehaviorGraphAgent agent = GetComponent<BehaviorGraphAgent>();
        if (agent != null)
        {
            bool success = true;
            m_Blackboard = agent.BlackboardReference;
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            success = m_Blackboard.SetVariableValue("PlayerCharacter", player);

            if (success == false)
            {
                Debug.LogWarning(this.name + ": One or more Blackboard Variables failed to be set.");
            }
        }
        else
        {
            Debug.LogWarning(this.name + " has no BehaviorGraphAgent!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_timer == m_UpdateInterval)
        {
            m_timer = 0;
            
            //Update blackboard values
            bool success = true;
            float distanceFromPlayer = CalculateDistanceFromPlayer();
            float angleFromPlayer = CalculateAngleFromPlayer();
            
            success = m_Blackboard.SetVariableValue("DistanceFromPlayer", distanceFromPlayer);
            success &= m_Blackboard.SetVariableValue("AngleFromPlayer", angleFromPlayer);
            success &= m_Blackboard.SetVariableValue("RotationToPlayer", new Vector3(0f, angleFromPlayer, 0f));

            if (success == false)
            {
                Debug.LogWarning(this.name + ": One or more Blackboard Variables failed to be updated.");
            }
        }
        else
        {
            m_timer++;
        }
    }

    float CalculateDistanceFromPlayer()
    {
        bool success = true;
        success = m_Blackboard.GetVariableValue("PlayerCharacter", out GameObject player);
        if (success == false)
        {
            Debug.LogWarning(this.name + ": Failed to retrieve player from blackboard");
            return -1.0f;
        }

        return Vector3.Distance(player.transform.position, transform.position);
    }

    float CalculateAngleFromPlayer()
    {
        bool success = true;
        success = m_Blackboard.GetVariableValue("PlayerCharacter", out GameObject player);
        if (success == false)
        {
            Debug.LogWarning(this.name + ": Failed to retrieve player from blackboard");
            return -420.0f;
        }
        
        Vector3 correctedPlayerPosition = player.transform.position;
        correctedPlayerPosition.y = transform.position.y;
        return Vector3.SignedAngle(correctedPlayerPosition - transform.position, transform.forward, transform.forward);
    }
}