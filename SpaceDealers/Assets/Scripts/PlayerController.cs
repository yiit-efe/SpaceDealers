using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public InputActionReference moveAction;

    public InputActionReference jumpAction;

    public CharacterController charCon;

    public InputActionReference lookAction;

    public Transform theCam;

    public float moveSpeed = 5f;

    public float lookSpeed = 10f;

    public float jumpForce = 5f;

    public float minLookAngle, maxLookAngle;

    private float ySpeed = 0f;

    private float horiRot,vertRot;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

        horiRot += lookInput.x * Time.deltaTime * lookSpeed;
        transform.rotation = Quaternion.Euler(0, horiRot, 0);

        vertRot -= lookInput.y * Time.deltaTime * lookSpeed;
        vertRot = Mathf.Clamp(vertRot, minLookAngle, maxLookAngle);
        theCam.localRotation = Quaternion.Euler(vertRot, 0, 0);


        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();

        Vector3 vertMove = transform.forward * moveInput.y;
        Vector3 horiMove = transform.right * moveInput.x;

        Vector3 moveAmount = vertMove + horiMove;
        moveAmount = moveAmount.normalized;

        moveAmount = moveAmount * moveSpeed;

        if (charCon.isGrounded)
        {
            ySpeed = 0f;

            if (jumpAction.action.WasPressedThisFrame())
            {
                ySpeed = jumpForce;
            }

        }

        ySpeed += (Physics.gravity.y * Time.deltaTime);

        

        moveAmount.y = ySpeed;

        charCon.Move(moveAmount * Time.deltaTime);
    }
}
