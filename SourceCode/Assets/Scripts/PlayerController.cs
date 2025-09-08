/*
    File Name: PlayerController.cs

    Authors: Dylan LAU (d.lau), Joel TEO (t.joel), Victor LEE (lee.v)    

    File Description:
    This script allows the player to control a GameObject using keyboard input.
    It reads horizontal and vertical axes (WASD/Arrow Keys) in Update()
    and applies movement to the object's Rigidbody2D in FixedUpdate()
    for smooth, physics-based motion.
*/

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get input from WASD or Arrow Keys
        float moveX = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right arrows
        float moveY = Input.GetAxisRaw("Vertical");   // W/S or Up/Down arrows

        moveInput = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        // Apply movement in the physics update
        rb.linearVelocity = moveInput * moveSpeed;
    }
}