/*
    File Name: AgentInfo.cs 
 
    Authors: Dylan LAU (d.lau), Joel TEO (t.joel), Victor LEE (lee.v)

    File Description:
    This script is a simple data container attached to an agent. Its only
    purpose is to hold the agent's assigned behavior such as Chasing or Fleeing
    so the TrainingManager knows how to handle it.
*/
using UnityEngine;

public class AgentInfo : MonoBehaviour
{
    public TrainingManager.BehaviorType behavior;
}