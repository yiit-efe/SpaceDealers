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

    public LayerMask whatIsStockBox;

    public LayerMask whatIsBin;

    public float moveSpeed = 5f;

    public float lookSpeed = 10f;

    public float jumpForce = 5f;

    public float interactionRange;

    public float minLookAngle, maxLookAngle;

    private float ySpeed = 0f;

    public float throwForce;

    private float horiRot, vertRot;

    private StockObject heldPickup;

    public StockBoxController heldBox;

    public Transform boxHoldPoint;

    public float waitToPlaceStock = 0.1f;
    private float placeStockCounter;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (UIController.instance.updatePricePanel != null)
        {
           if (UIController.instance.updatePricePanel.activeSelf == true)
            {
                return;
            }
        }

        if (UIController.instance.buyMenuScreen != null)
        {
            if (UIController.instance.buyMenuScreen.activeSelf == true)
            {
                return;
            }
        }

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

        if (heldPickup == null && heldBox == null)
        {


            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsStock))
                {
                    heldPickup = hit.collider.GetComponentInParent<StockObject>();
                    heldPickup.transform.SetParent(holdPoint);
                    heldPickup.Pickup();

                    return;
                }

                if (Physics.Raycast(ray, out hit, interactionRange, whatIsStockBox))
                {
                    heldBox = hit.collider.GetComponentInParent<StockBoxController>();
                    heldBox.transform.SetParent(boxHoldPoint);
                    heldBox.Pickup();

                    if (heldBox.flap1.activeSelf == true)
                    {
                        heldBox.OpenClose();
                    }

                    return;
                }

            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsShelf))
                {
                    heldPickup = hit.collider.GetComponentInParent<ShelfSpaceController>().GetStock();

                    if (heldPickup != null)
                    {
                        heldPickup.transform.SetParent(holdPoint);
                        heldPickup.Pickup();
                    }

                    return;
                }

                if (Physics.Raycast(ray, out hit, interactionRange, whatIsStockBox))
                {
                    hit.collider.GetComponentInParent<StockBoxController>().OpenClose();
                }
            }


            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsShelf))
                {
                    ShelfSpaceController shelf = hit.collider.GetComponentInParent<ShelfSpaceController>();
                    if (shelf != null)
                    {
                        shelf.StartPriceUpdate();
                    }
                }
            }

        }
        else
        {
            if (heldPickup != null)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    if (Physics.Raycast(ray, out hit, interactionRange, whatIsShelf))
                    {
                        ShelfSpaceController shelf = hit.collider.GetComponentInParent<ShelfSpaceController>();

                        if (shelf != null)
                        {
                           shelf.PlaceStock(heldPickup);
                           
                           if (heldPickup.isPlaced == true)
                            {
                                heldPickup = null;
                            }

                        }
                    }
                }

                if (Mouse.current.rightButton.wasPressedThisFrame && heldPickup != null)
                {
                    heldPickup.Release();

                    heldPickup.theRB.AddForce(theCam.transform.forward * throwForce, ForceMode.Impulse);

                    heldPickup.transform.SetParent(null);
                    heldPickup = null;

                }
            }

            if (heldBox != null)
            {
                if (Mouse.current.rightButton.wasPressedThisFrame)
                {
                    heldBox.Release();
                    heldBox.theRB.AddForce(theCam.transform.forward * throwForce, ForceMode.Impulse);

                    heldBox.transform.SetParent(null);
                    heldBox = null;

                }

                if (Keyboard.current.eKey.wasPressedThisFrame)
                {
            
                    heldBox.OpenClose();

                }

                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    if (heldBox.stockInBox.Count > 0)
                    {
                        if (Physics.Raycast(ray, out hit, interactionRange, whatIsShelf))
                        {
                            ShelfSpaceController shelf = hit.collider.GetComponentInParent<ShelfSpaceController>();
                            if (shelf != null)
                            {
                                heldBox.PlaceStockOnShelf(shelf);
                                placeStockCounter = waitToPlaceStock;
                            }
                        }
                    } else
                    {
                        if (Physics.Raycast(ray, out hit, interactionRange, whatIsBin))
                        {
                            Destroy(heldBox.gameObject);

                            heldBox = null;
                        }
                    }
                    
                    
                }

                if (Mouse.current.leftButton.isPressed)
                {
                    placeStockCounter -= Time.deltaTime;

                    if (placeStockCounter <= 0)
                    {
                        if (Physics.Raycast(ray, out hit, interactionRange, whatIsShelf))
                        {
                            ShelfSpaceController shelf = hit.collider.GetComponentInParent<ShelfSpaceController>();
                            if (shelf != null)
                            {
                                heldBox.PlaceStockOnShelf(shelf);
                                placeStockCounter = waitToPlaceStock;
                            }
                        }
                    }
                }
            }
        }
    }
}
