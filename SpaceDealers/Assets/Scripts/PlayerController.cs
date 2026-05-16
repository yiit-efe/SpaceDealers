using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public InputActionReference moveAction;

    public InputActionReference jumpAction;

    public CharacterController charCon;

    public InputActionReference lookAction;

    public Camera theCam;

    public Transform holdPoint;

    public LayerMask whatIsStock;

    public LayerMask whatIsShelf;

    public float moveSpeed = 5f;

    public float lookSpeed = 10f;

    public float jumpForce = 5f;

    public float interactionRange;

    public float minLookAngle, maxLookAngle;

    private float ySpeed = 0f;

    private float horiRot, vertRot;

    private StockObject heldPickup;

    public float throwForce;



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
        theCam.transform.localRotation = Quaternion.Euler(vertRot, 0, 0);


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


        //check for pickup

        Ray ray = theCam.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
        RaycastHit hit;

        if (heldPickup == null)
        {


            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsStock))
                {
                    heldPickup = hit.collider.GetComponent<StockObject>();
                    heldPickup.transform.SetParent(holdPoint);
                    heldPickup.Pickup();
                }
            }
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsShelf))
                {
                    heldPickup = hit.collider.GetComponent<ShelfSpaceController>().GetStock();
                    
                    if (heldPickup != null)
                    {
                        heldPickup.transform.SetParent(holdPoint);
                        heldPickup.Pickup();
                    }
                }
            }




        }
        else
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsShelf))
                {
                    hit.collider.GetComponent<ShelfSpaceController>().PlaceStock(heldPickup);
                    if (heldPickup.isPlaced == true)
                    {
                        heldPickup = null;
                    }
                }
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                heldPickup.Release();

                heldPickup.theRB.AddForce(theCam.transform.forward * throwForce, ForceMode.Impulse);

                heldPickup.transform.SetParent(null);
                heldPickup = null;

            }
        }
    }
}
