using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    Animator animator;

    public float speed = 6f;
    public float runMultiplier = 1.5f;
    public float crouchMultiplier = 0.8f;

    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        bool crouching = Input.GetButton("Crouch");
        bool running = Input.GetButton("Run");

        float multiplier = 1f;

        if (crouching && !running)
        {
            multiplier = crouchMultiplier;

            RayBundleSensor.raycastLength = 25f;

            animator.SetBool("isCrouching", true);
        }
        else
        {
            RayBundleSensor.raycastLength = 30f;
            animator.SetBool("isCrouching", false);
        }

        if (running)
        {
            multiplier = runMultiplier;

            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            controller.Move(moveDir * speed * multiplier * Time.deltaTime);

            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }
}
