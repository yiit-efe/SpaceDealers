using UnityEngine;

public class StockObject : MonoBehaviour
{
    public StockInfo info; 

    public float moveSpeed;

    public bool isPlaced;

    public Rigidbody theRB;
    public Collider col;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       info = StockInfoController.instance.GetInfo(info.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaced == true)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, moveSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, moveSpeed * Time.deltaTime);
        }
    }

    public void Pickup()
    {
        theRB.isKinematic = true;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        isPlaced = false;

        col.enabled = false;
    }

    public void MakePlaced()
    {
        theRB.isKinematic = true;
        
        isPlaced = true;

        col.enabled = false;
    }

    public void Release()
    {
        theRB.isKinematic = false;

        col.enabled = true;
    }

}
