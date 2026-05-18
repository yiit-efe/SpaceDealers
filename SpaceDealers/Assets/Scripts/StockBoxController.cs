using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class StockBoxController : MonoBehaviour
{
    public StockInfo info;


    public List<StockObject> objectsOnShelf;
    public List<Transform> bigDrinkPoints, cerealPoints, chipsTubePoints, fruitPoints, fruitLargePoints;

    public List<StockObject> stockInBox;

    public Rigidbody theRB;
    public Collider col;

    private bool isHeld;

    public float moveSpeed = 5f;

    public GameObject flap1, flap2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isHeld == true)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, moveSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, moveSpeed * Time.deltaTime);
        }
    }

    public void SetupBox(StockInfo stockType)
    {
        info = stockType;

        List<Transform> activePoints = new List<Transform>();

        switch (info.typeOfStock)
        {
            case StockInfo.StockType.bigDrink:

                activePoints = bigDrinkPoints;

                break;

            case StockInfo.StockType.cereal:

                activePoints = cerealPoints;

                break;

            case StockInfo.StockType.chipsTube:

                activePoints = chipsTubePoints;

                break;

            case StockInfo.StockType.fruit:

                activePoints = fruitPoints;

                break;

            case StockInfo.StockType.fruitLarge:

                activePoints = fruitLargePoints;

                break;
        }

        if (stockInBox.Count == 0)
        {
            for (int i = 0; i < activePoints.Count; i++)
            {
                StockObject stock = Instantiate(stockType.stockObject , activePoints[i]);
                stock.transform.localPosition = Vector3.zero;
                stock.transform.localRotation = Quaternion.identity;

                stockInBox.Add(stock);

                stock.PlaceInBox(); 
            }
        }

    }
    
    public void Pickup()
    {
        theRB.isKinematic = true;

        col.enabled = false;

        isHeld = true;
    }

    public void Release()
    {
        theRB.isKinematic = false;

        col.enabled = true;

        isHeld = false;
    }

    public void OpenClose()
    {
        if (flap1.activeSelf == true)
        {
            flap1.SetActive(false);
            flap2.SetActive(false);
        }
        else
        {
            flap1.SetActive(true);
            flap2.SetActive(true);
        }
    }

    public void PlaceStockOnShelf(ShelfSpaceController shelf)
    {
        if (stockInBox.Count > 0)
        {
            shelf.PlaceStock(stockInBox[stockInBox.Count - 1]);

            if (stockInBox[stockInBox.Count - 1].isPlaced == true)
            {
                stockInBox.RemoveAt(stockInBox.Count - 1);
            }
        }

        if (flap1.activeSelf ==true)
        {
            OpenClose();
        }

    }

    public int GetStockAmount(StockInfo.StockType type)
    {
        int toReturn = 0;

        switch (type)
        {
            case StockInfo.StockType.bigDrink:

                toReturn = bigDrinkPoints.Count;

                break;

            case StockInfo.StockType.cereal:

                toReturn = cerealPoints.Count;

                break;

            case StockInfo.StockType.chipsTube:

                toReturn = chipsTubePoints.Count;

                break;

            case StockInfo.StockType.fruit:

                toReturn = fruitPoints.Count;

                break;

            case StockInfo.StockType.fruitLarge:

                toReturn = fruitLargePoints.Count;

                break;
        }

        return toReturn;
    }

}
