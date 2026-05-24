using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public List<NavPoint> points = new List<NavPoint>();

    public float moveSpeed;
    private float currentWaitTime = 0;

    public Animator anim;

    public float browseTime;

    public FurnitureController currentShelfCase;

    public GameObject shoppingBag;
    private bool hasGrabbed;

    public float waitAfterGrabbing = .5f;

    private List<StockObject> stockInBag = new List<StockObject>();

    [Header("Potion settings")]
    public StockObject enlargePotionPrefab;
    public StockObject shrinkPotionPrefab;
    public float perEnlargeAmount = 0.1f;
    public float perShrinkAmount = 0.1f;

    [Header("Scale animation")]
    public float scaleLerpDuration = 0.5f;
    private Vector3 baseScale;
    private Coroutine scaleCoroutine;

    private Vector3 queuePoint;

    public enum CustomerState
    {
        entering,
        browsing,
        queueing,
        atCheckout,
        leaving,
    }

    public CustomerState currentState;

    public int maxBrowsePoints = 5;
    private int browsePointsRemain;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        points.Clear();
        points.AddRange(CustomerManager.instance.GetEntryPoints());

        if (points.Count > 0)
        {
            transform.position = points[0].point.position;

            currentWaitTime = points[0].waitTime;
        }

        // record the initial scale so potion scaling is relative
        baseScale = transform.localScale;


    }

    // Update is called once per frame
    void Update()
    {

        switch (currentState)
        {
            case CustomerState.entering:

                if (points.Count > 0)
                {
                    MoveToPoint();
                }
                else
                {
                    if (StoreController.instance.shelvingCases.Count > 0)
                    {
                        currentState = CustomerState.browsing;

                        browsePointsRemain = Random.Range(1, maxBrowsePoints + 1);
                        browsePointsRemain = Mathf.Clamp(browsePointsRemain, 1, StoreController.instance.shelvingCases.Count);

                        GetBrowsePoint();
                    } else
                    {
                        StartLeaving();
                    }

                    
                }
                break;

            case CustomerState.browsing:

                MoveToPoint();

                if (points.Count == 0)
                {
                    if (!hasGrabbed)
                    {
                        GrabStock();
                    }
                    else
                    {
                        hasGrabbed = false;

                        browsePointsRemain--;
                        if (browsePointsRemain > 0)
                        {
                            GetBrowsePoint();
                        }
                        else
                        {
                            if (stockInBag.Count > 0)
                            {
                                Checkout.Instance.AddCustomerToQueue(this);
                                currentState = CustomerState.queueing;
                            } else
                            {
                                StartLeaving();
                            }
                        }
                    }

                }

                break;


            case CustomerState.queueing:

                transform.position = Vector3.MoveTowards(transform.position, queuePoint, moveSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.position, queuePoint) > 0.1f)
                {
                   anim.SetBool("isMoving", true);
                } else
                {
                    anim.SetBool("isMoving", false);
                }
                    break;


            case CustomerState.atCheckout:

                break;

            case CustomerState.leaving:


                if (points.Count > 0)
                {
                    MoveToPoint();
                }
                else
                {
                    Destroy(gameObject);
                }

                break;
        }

    }

    public void MoveToPoint()
    {
        if (points.Count > 0)
        {

            bool isMoving = true;

            Vector3 targetPosition = new Vector3(points[0].point.position.x, transform.position.y, points[0].point.position.z);

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            transform.LookAt(targetPosition);

            if (Vector3.Distance(transform.position, targetPosition) < 0.25f)
            {
                isMoving = false;

                currentWaitTime -= Time.deltaTime;

                if (currentWaitTime <= 0)
                {
                    StartNextPoint();
                }

            }

            anim.SetBool("isMoving", isMoving);

        } else
        {
            StartNextPoint();
        }
    }

    public void StartNextPoint()
    {
        if (points.Count > 0)
        {
            points.RemoveAt(0);

            if (points.Count > 1)
            {
                currentWaitTime = points[0].waitTime;
            }
        }
    }

    public void StartLeaving()
    {
        currentState = CustomerState.leaving;

        points.Clear();

        points.AddRange(CustomerManager.instance.GetExitPoints());
    }

    void GetBrowsePoint()
    {
        points.Clear();

        int selectedShelf = Random.Range(0, StoreController.instance.shelvingCases.Count);

        points.Add(new NavPoint());
        points[0].point = StoreController.instance.shelvingCases[selectedShelf].standPoint;

        points[0].waitTime = browseTime * Random.Range(0.75f, 1.25f);

        currentWaitTime = points[0].waitTime;

        currentShelfCase = StoreController.instance.shelvingCases[selectedShelf];
    }

    public void GrabStock()
    {
     
        hasGrabbed = true;

        int shelf = Random.Range(0, currentShelfCase.shelves.Count);

        StockObject stock = currentShelfCase.shelves[shelf].GetStock();

        if (stock != null)
        {
            stock.transform.SetParent(shoppingBag.transform);
            stockInBag.Add(stock);
            stock.PlaceInBag();

            shoppingBag.SetActive(true);

            points.Clear();
            points.Add(new NavPoint());
            points[0].point = currentShelfCase.standPoint;
            points[0].waitTime = waitAfterGrabbing * Random.Range(0.75f, 1.25f);
            currentWaitTime = points[0].waitTime;
        }
    }

    public void UpdateQueuePoint(Vector3 newPoint)
    {
        queuePoint = newPoint;
        transform.LookAt(queuePoint);
    }

    public float GetTotalSpend()
    {
        float total= 0f;

        foreach (StockObject stock in stockInBag)
        {
            total += stock.info.currentPrice;
        }


        return total;
    }

    // Adjust the customer's scale after checkout based on shopping bag contents
    public void ApplyCheckoutScale()
    {
        // Use the configured potion prefabs (StockObject prefab references) to identify potions in the bag.
        int enlargeCount = 0;
        int shrinkCount = 0;

        foreach (StockObject stock in stockInBag)
        {
            if (stock == null || stock.info == null) continue;

            // StockInfo.stockObject holds the prefab reference used when instantiating this stock.
            if (enlargePotionPrefab != null && stock.info.stockObject == enlargePotionPrefab) enlargeCount++;
            if (shrinkPotionPrefab != null && stock.info.stockObject == shrinkPotionPrefab) shrinkCount++;
        }

        float scaleMultiplier = 1f + (enlargeCount * perEnlargeAmount) - (shrinkCount * perShrinkAmount);
        scaleMultiplier = Mathf.Clamp(scaleMultiplier, 0.2f, 3.0f);

        Vector3 targetScale = baseScale * scaleMultiplier;

        // start animated scale change
        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(AnimateScale(transform.localScale, targetScale, scaleLerpDuration));
    }

    private IEnumerator AnimateScale(Vector3 from, Vector3 to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(from, to, t / duration);
            yield return null;
        }

        transform.localScale = to;
        scaleCoroutine = null;
    }
}

[System.Serializable]
public class NavPoint
{
    public Transform point;
    public float waitTime;
}