/* 
    File Name: AgentController.cs

    Authors: Dylan LAU (d.lau), Joel TEO (t.joel), Victor LEE (lee.v)

    File Description:
    This script is attached to each agent and handles its physical actions.
    It receives a command (an integer representing an action) from the
    TrainingManager and translates it into movement by controlling the
    agent's Rigidbody2D. It also reports collisions back to the manager.
*/
using UnityEngine;

public class AgentController : MonoBehaviour
{
    public float moveSpeed = 3f;
    private Rigidbody2D rb;
    private TrainingManager manager;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        manager = FindObjectOfType<TrainingManager>();
    }

    public void Move(int action)
    {
        Vector2 direction = Vector2.zero;
        switch (action)
        {
            case 0: direction = Vector2.up; break;
            case 1: direction = Vector2.down; break;
            case 2: direction = Vector2.left; break;
            case 3: direction = Vector2.right; break;
            case 4: break; // Stay
        }
        rb.linearVelocity = direction * moveSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (manager != null)
        {
            manager.ProcessCollision(gameObject, collision.gameObject);
        }
    }
}
