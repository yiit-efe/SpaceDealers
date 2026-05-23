using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject updatePricePanel;

    public TMP_Text basePriceText, currentPriceText;

    public TMP_InputField priceInputField;

    private StockInfo activeStockInfo;

    public TMP_Text moneyText;

    public GameObject buyMenuScreen;

    public string mainMenuScene;

    public GameObject pauseScreen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            OpenCloseBuyMenu();
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            PauseUnpause();
        }
    }

    public void OpenUpdatePrice(StockInfo stockToUpdate)
    {
        updatePricePanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;

        basePriceText.text = "$" + stockToUpdate.price.ToString("F2");
        currentPriceText.text = "$" + stockToUpdate.currentPrice.ToString("F2");

        activeStockInfo = stockToUpdate;

        priceInputField.text = stockToUpdate.currentPrice.ToString();
    }

    public void CloseUpdatePrice()
    {
        updatePricePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ApplyPriceUpdate()
    {

        activeStockInfo.currentPrice = float.Parse(priceInputField.text);

        currentPriceText.text = "$" + activeStockInfo.currentPrice.ToString("F2");

        StockInfoController.instance.UpdatePrice(activeStockInfo.name, activeStockInfo.currentPrice);

        CloseUpdatePrice();

    }

    public void UpdateMoney(float currentMoney)
    {
        moneyText.text = "$" + currentMoney.ToString("F2");
    }

    public void OpenCloseBuyMenu()
    {
        if (buyMenuScreen.activeSelf == false)
        {
            buyMenuScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            buyMenuScreen.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);

        Time.timeScale = 1f;
    }

    public void PauseUnpause()
    {
        if (pauseScreen.activeSelf == false)
        {
            pauseScreen.SetActive(true);

            Cursor.lockState = CursorLockMode.None;

            Time.timeScale = 0f;
        }
        else
        {
            pauseScreen.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;

            Time.timeScale = 1f;
        }
    }
}
