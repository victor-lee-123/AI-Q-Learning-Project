/* 
    File Name: QTable.cs

    Author: Dylan LAU (d.lau), Joel TEO (t.joel), Victor LEE (lee.v)
  
    File Description:
    This is not a MonoBehaviour. It's a plain C# class that represents the
    "brain" of a Q-learning agent. It uses a Dictionary to store the
    Q-values for every state-action pair the agent has experienced. The
    TrainingManager uses this class to get, set, and find the best
    action for any given state.
*/

using System.Collections.Generic;

// A state is defined by the agent's position, and its relation to the player.
// We use a string representation as the key for our dictionary.
// Example key: "x_y_playerDir_playerVisible" -> "5_10_UpRight_True"

public class QTable
{
    // Our main table: Dictionary<State, ActionValues>
    // ActionValues is an array where the index corresponds to an action.
    public Dictionary<string, float[]> table = new Dictionary<string, float[]>();
    private int numActions;

    public QTable(int actions)
    {
        numActions = actions;
    }

    // Get the Q-value for a given state-action pair.
    public float GetValue(string state, int action)
    {
        EnsureStateExists(state);
        return table[state][action];
    }

    // Set the Q-value for a given state-action pair.
    public void SetValue(string state, int action, float value)
    {
        EnsureStateExists(state);
        table[state][action] = value;
    }

    // Get the best action (the one with the highest Q-value) for a given state.
    public int GetBestAction(string state)
    {
        EnsureStateExists(state);
        float[] actionValues = table[state];
        int bestAction = 0;
        float maxValue = float.MinValue;

        for (int i = 0; i < actionValues.Length; i++)
        {
            if (actionValues[i] > maxValue)
            {
                maxValue = actionValues[i];
                bestAction = i;
            }
        }
        return bestAction;
    }

    // Get the maximum Q-value for a given state (used in the Q-learning formula).
    public float GetMaxQValue(string state)
    {
        EnsureStateExists(state);
        float[] actionValues = table[state];
        float maxValue = float.MinValue;

        foreach (float value in actionValues)
        {
            if (value > maxValue)
            {
                maxValue = value;
            }
        }
        return maxValue;
    }

    // If a state has not been visited before, initialize its action values to 0.
    private void EnsureStateExists(string state)
    {
        if (!table.ContainsKey(state))
        {
            table[state] = new float[numActions];
        }
    }
}