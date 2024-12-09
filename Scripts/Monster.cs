using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {
    public float moveSpeed = 7;
    public float smoothMoveTime = .1f;
    public float turnSpeed = 8;
    public float jumpHeight = 2f;  // Height of the jump
    public float jumpDuration = 1f;  // Duration of the jump (time to reach the peak and come back down)
    
    private float angle;
    private float smoothInputMagnitude;
    private float smoothMoveVelocity;
    private Vector3 velocity;
    private Animator animator;
    private bool isJumping = false;  // Track if the monster is in the middle of a jump

    void Start() {
        animator = GetComponent<Animator>();
    }

    void Update() {
        Vector3 inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        float inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);

        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);

        velocity = moveSpeed * smoothInputMagnitude * transform.forward;

        // Cap velocity at a maximum of 7
        if (velocity.magnitude > 7) {
            velocity = velocity.normalized * 7;
        }

        // Stop movement if velocity is less than 0
        if (velocity.magnitude < 0.01f) {
            velocity = Vector3.zero;
        }

        animator.SetFloat("velocity", Mathf.Abs(velocity.magnitude));

        // Check if the space bar is pressed and the monster is not already jumping
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping) {
            StartCoroutine(Jump());
        }
    }

    void FixedUpdate() {
        // Rotate the monster
        transform.rotation = Quaternion.Euler(Vector3.up * angle);

        // Move the monster (if not jumping)
        if (!isJumping) {
            transform.position += velocity * Time.deltaTime;
        }
    }

    IEnumerator Jump() {
        isJumping = true;
        animator.SetBool("isjumping", true);  // Trigger the jump animation

        Vector3 startPosition = transform.position;
        Vector3 peakPosition = startPosition + Vector3.up * jumpHeight;

        float elapsedTime = 0f;

        // Move up to the peak of the jump
        while (elapsedTime < jumpDuration / 2f) {
            transform.position = Vector3.Lerp(startPosition, peakPosition, (elapsedTime / (jumpDuration / 2f)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Move back down to the starting position
        elapsedTime = 0f;
        while (elapsedTime < jumpDuration / 2f) {
            transform.position = Vector3.Lerp(peakPosition, startPosition, (elapsedTime / (jumpDuration / 2f)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = startPosition;  // Ensure it lands exactly at the start position
        isJumping = false;
        animator.SetBool("isjumping", false);  // End the jump animation
    }
}
