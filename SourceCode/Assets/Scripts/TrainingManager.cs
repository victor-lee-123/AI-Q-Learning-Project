/*
    File Name: TrainingManager.cs

    Authors: Dylan LAU (d.lau), Joel TEO (t.joel), Victor LEE (lee.v)
 
    File Description:
    This is the master script that orchestrates the entire reinforcement
    learning process. It spawns agents, manages their separate Q-Tables (brains),
    runs the main training loop, calculates rewards, and updates the UI.
    It is the central hub for all simulation logic. 
*/

using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class TrainingManager : MonoBehaviour
{
    // --- Public Enums ---
    public enum BehaviorType { Chase, Flee }

    // --- Public Fields (Editable in Inspector) ---
    [Header("Agent Setup")]
    public GameObject agentPrefab;
    public Transform playerDummy;
    public SpriteRenderer arenaFloor; // *** NEW: Reference to the floor sprite
    public float spawnPadding = 1f;   // *** NEW: Padding from the edge

    [Header("Training Parameters")]
    [Range(0f, 1f)] public float learningRate = 0.1f;
    [Range(0f, 1f)] public float discountFactor = 0.99f;
    [Range(0f, 1f)] public float explorationRate = 1.0f;
    public float explorationDecay = 0.001f;
    public int gridSize = 1;

    [Header("Behavior & Rewards")]
    public float reward_touchPlayer = 10f;
    public float reward_wallCollision = -5f;
    public float reward_moveCloser = 1f;
    public float reward_moveAway = -0.5f;
    public float reward_timePenalty = -0.1f;

    [Header("UI Display")]
    public TextMeshProUGUI qTableDebugText;

    // --- Private Fields ---
    private List<GameObject> agents = new List<GameObject>();
    private QTable chaseQTable; // Brain for the chaser
    private QTable fleeQTable;  // Brain for the fleer

    private Dictionary<GameObject, string> agentStates = new Dictionary<GameObject, string>();
    private Dictionary<GameObject, float> agentDistances = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, int> agentLastActions = new Dictionary<GameObject, int>();

    private const int NUM_ACTIONS = 5;

    void Start()
    {
        chaseQTable = new QTable(NUM_ACTIONS);
        fleeQTable = new QTable(NUM_ACTIONS);
        SpawnAgents();
    }

    void FixedUpdate()
    {
        foreach (GameObject agent in agents)
        {
            AgentInfo info = agent.GetComponent<AgentInfo>();
            QTable activeQTable = (info.behavior == BehaviorType.Chase) ? chaseQTable : fleeQTable;

            string oldState = GetState(agent);
            agentStates[agent] = oldState;

            int action = ChooseAction(oldState, activeQTable);
            agentLastActions[agent] = action;

            agent.GetComponent<AgentController>().Move(action);

            string newState = GetState(agent);
            float reward = CalculateReward(agent, info.behavior);

            UpdateQTable(oldState, action, reward, newState, activeQTable);
        }

        if (explorationRate > 0.01f)
        {
            explorationRate -= explorationDecay * Time.fixedDeltaTime;
        }

        UpdateUI();
    }

    void SpawnAgents()
    {
        // --- Spawn the Chaser ---
        GameObject chaser = Instantiate(agentPrefab, GetRandomSpawnPosition(), Quaternion.identity);
        chaser.name = "Chaser Agent";
        AgentInfo chaserInfo = chaser.GetComponent<AgentInfo>();
        chaserInfo.behavior = BehaviorType.Chase;
        chaser.GetComponent<SpriteRenderer>().color = Color.red;
        InitializeAgent(chaser);

        // --- Spawn the Fleer ---
        GameObject fleer = Instantiate(agentPrefab, GetRandomSpawnPosition(), Quaternion.identity);
        fleer.name = "Fleer Agent";
        AgentInfo fleerInfo = fleer.GetComponent<AgentInfo>();
        fleerInfo.behavior = BehaviorType.Flee;
        fleer.GetComponent<SpriteRenderer>().color = Color.blue;
        InitializeAgent(fleer);
    }

    Vector3 GetRandomSpawnPosition()
    {
        if (arenaFloor == null)
        {
            Debug.LogError("Arena Floor not assigned in TrainingManager! Spawning at center.");
            return Vector3.zero;
        }

        Bounds floorBounds = arenaFloor.bounds;
        float spawnX = Random.Range(floorBounds.min.x + spawnPadding, floorBounds.max.x - spawnPadding);
        float spawnY = Random.Range(floorBounds.min.y + spawnPadding, floorBounds.max.y - spawnPadding);

        return new Vector3(spawnX, spawnY, 0);
    }

    void InitializeAgent(GameObject agent)
    {
        agents.Add(agent);
        agentStates.Add(agent, GetState(agent));
        agentDistances.Add(agent, Vector2.Distance(agent.transform.position, playerDummy.position));
        agentLastActions.Add(agent, 4);
    }

    string GetState(GameObject agent)
    {
        int x = Mathf.RoundToInt(agent.transform.position.x / gridSize);
        int y = Mathf.RoundToInt(agent.transform.position.y / gridSize);
        Vector2 dirToPlayer = (playerDummy.position - agent.transform.position).normalized;
        int playerDirection = DiscretizeDirection(dirToPlayer);
        return $"{x}_{y}_{playerDirection}";
    }

    int DiscretizeDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        if (angle >= 337.5 || angle < 22.5) return 0;
        if (angle >= 22.5 && angle < 67.5) return 1;
        if (angle >= 67.5 && angle < 112.5) return 2;
        if (angle >= 112.5 && angle < 157.5) return 3;
        if (angle >= 157.5 && angle < 202.5) return 4;
        if (angle >= 202.5 && angle < 247.5) return 5;
        if (angle >= 247.5 && angle < 292.5) return 6;
        if (angle >= 292.5 && angle < 337.5) return 7;
        return 0;
    }

    int ChooseAction(string state, QTable qTable)
    {
        if (Random.value < explorationRate)
        {
            return Random.Range(0, NUM_ACTIONS);
        }
        else
        {
            return qTable.GetBestAction(state);
        }
    }

    float CalculateReward(GameObject agent, BehaviorType behavior)
    {
        float reward = reward_timePenalty;

        float oldDist = agentDistances[agent];
        float newDist = Vector2.Distance(agent.transform.position, playerDummy.position);
        agentDistances[agent] = newDist;

        if (behavior == BehaviorType.Chase)
        {
            if (newDist < oldDist) reward += reward_moveCloser;
            else reward += reward_moveAway;
        }
        else if (behavior == BehaviorType.Flee)
        {
            if (newDist > oldDist) reward += reward_moveCloser;
            else reward += reward_moveAway;
        }
        return reward;
    }

    public void ProcessCollision(GameObject agent, GameObject other)
    {
        AgentInfo info = agent.GetComponent<AgentInfo>();
        QTable activeQTable = (info.behavior == BehaviorType.Chase) ? chaseQTable : fleeQTable;

        float reward = 0;
        bool shouldReset = false;

        if (other.CompareTag("Player"))
        {
            reward = (info.behavior == BehaviorType.Chase) ? reward_touchPlayer : -reward_touchPlayer;
            shouldReset = true;
        }
        else if (other.CompareTag("Wall"))
        {
            reward = reward_wallCollision;
            shouldReset = true;
        }

        string state = agentStates[agent];
        int lastAction = agentLastActions[agent];
        UpdateQTable(state, lastAction, reward, state, activeQTable);

        if (shouldReset)
        {
            agent.transform.position = GetRandomSpawnPosition();
        }
    }

    void UpdateQTable(string oldState, int action, float reward, string newState, QTable qTable)
    {
        float oldQ = qTable.GetValue(oldState, action);
        float futureMaxQ = qTable.GetMaxQValue(newState);
        float newQ = oldQ + learningRate * (reward + discountFactor * futureMaxQ - oldQ);
        qTable.SetValue(oldState, action, newQ);
    }

    void UpdateUI()
    {
        if (qTableDebugText == null || agents.Count < 2) return;

        string debugText = $"Exploration Rate: {explorationRate:F4}\n\n";

        GameObject chaser = agents[0];
        string chaserState = agentStates[chaser];
        float[] chaserQValues = chaseQTable.table.ContainsKey(chaserState) ? chaseQTable.table[chaserState] : new float[NUM_ACTIONS];
        debugText += $"<color=red>-- CHASER --</color>\n";
        debugText += $"State: {chaserState}\n";
        debugText += $"Q(U):{chaserQValues[0]:F2} D:{chaserQValues[1]:F2} L:{chaserQValues[2]:F2} R:{chaserQValues[3]:F2} S:{chaserQValues[4]:F2}\n\n";

        GameObject fleer = agents[1];
        string fleerState = agentStates[fleer];
        float[] fleerQValues = fleeQTable.table.ContainsKey(fleerState) ? fleeQTable.table[fleerState] : new float[NUM_ACTIONS];
        // *** FIXED: Changed color name from "cyan" to "blue" for TextMeshPro
        debugText += $"<color=blue>-- FLEER --</color>\n";
        debugText += $"State: {fleerState}\n";
        debugText += $"Q(U):{fleerQValues[0]:F2} D:{fleerQValues[1]:F2} L:{fleerQValues[2]:F2} R:{fleerQValues[3]:F2} S:{fleerQValues[4]:F2}";

        qTableDebugText.text = debugText;
    }
}