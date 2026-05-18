using UnityEngine;

public class StoreController : MonoBehaviour
{
    public static StoreController instance;

    private void Awake()
    {
        instance = this;
    }

    public float currentMoney = 1000f;

    public Transform stockSpawnPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIController.instance.UpdateMoney(currentMoney);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddMoney(float amountToAdd)
    {
        currentMoney += amountToAdd;

        UIController.instance.UpdateMoney(currentMoney);

    }

    public void SpendMoney(float amountToSpend)
    {
        currentMoney -= amountToSpend;

        if (currentMoney < 0)
        {
            currentMoney = 0;
        }

        UIController.instance.UpdateMoney(currentMoney);

    }

    public bool CheckMoneyAvailable(float amountToCheck)
    {
        bool hasEnough = false;

        if (currentMoney >= amountToCheck)
        {
            hasEnough = true;
        }

        return hasEnough;
    }

}
