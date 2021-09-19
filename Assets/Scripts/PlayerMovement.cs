using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTransform; 
    private Rigidbody playerBody;
    private float distToGround = 2f;

    private Vector3 inputVector;
    private Vector2 mouseDelta;

    private float speed = 10;
    private float mouseSensitivity = 5;
    private float cameraPitch = 0;

    private float upwardForce = 0;

    public float boostAmount = 3;
    public float boostTime = 1;
    private float boostMultiplier = 1f;
    private bool canBoost = false;

    void Start()
    {
        playerBody = GetComponent<Rigidbody>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    bool IsGrounded() {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

    void jump ()
    {
        playerBody.velocity = new Vector3(
            playerBody.velocity.x,
            playerBody.velocity.y + 8f,
            playerBody.velocity.z
        );
    }
    void boost ()
    {
        boostMultiplier = boostAmount;
        playerBody.velocity = new Vector3(
            playerBody.velocity.x,
            playerBody.velocity.y + 15f,
            playerBody.velocity.z
        );
    }

    void Update()
    {
        mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        inputVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        inputVector.Normalize();

        if (IsGrounded()) {
            canBoost = true;
        }

        if (boostTapTimeLeft > 0) {
            boostTapTimeLeft -= Time.deltaTime;
        }else{
            boostTapTimeLeft = 0;
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (!IsGrounded()) {
                if (canBoost) {
                    boost();
                    canBoost = false;
                }
            }else{
                boostTapTimeLeft = 0.5f;
                jump();
            }
        }

        cameraPitch -= mouseDelta.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);
        
        transform.Rotate(Vector3.up * mouseDelta.x * mouseSensitivity);
        cameraTransform.localEulerAngles = Vector3.right * cameraPitch;

        var locVel = transform.InverseTransformDirection(playerBody.velocity);
        locVel.x = inputVector.x * speed * boostMultiplier;
        locVel.y = playerBody.velocity.y;
        locVel.z = inputVector.z * speed * boostMultiplier;
        playerBody.velocity = transform.TransformDirection(locVel);

        if (boostMultiplier > 1f) {
            boostMultiplier -= Time.deltaTime * (boostAmount/boostTime);
        }
        if (boostMultiplier < 1f) {
            boostMultiplier = 1f;
        }

    }
}
