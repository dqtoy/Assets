﻿using UnityEngine;

public class AgentDecisionBehavior : MonoBehaviour
{
    public SIO_AttackListener attackListener;
    private AgentDecisionTree agentDecisionMaking;

    private void Start()
    {
        agentDecisionMaking = new AgentDecisionTree(attackListener);
    }

    public void MakeDecision()
    {
        agentDecisionMaking.MakeDecision();
    }
}
