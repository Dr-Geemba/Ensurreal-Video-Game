using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI healthText;
    private const int mainMenu = 0;
    public void ReturnToMenu()
    {
        SceneManager.LoadScene(mainMenu);
    }
    void Awake()
    {
        moneyText.text = "Money: " + CurrentData.Instance.playerMoney;
        healthText.text = "Health: " + CurrentData.Instance.playerHealth;
    }
    void OnEnable()
    {
        CurrentData.OnPlayerHealthChange += UpdateHealthCount;
        CurrentData.OnPlayerMoneyChange += UpdateMoneyCount;
    }
    void OnDisable()
    {
        CurrentData.OnPlayerHealthChange -= UpdateHealthCount;
        CurrentData.OnPlayerMoneyChange -= UpdateMoneyCount;
    }
    void UpdateMoneyCount(int newAmount)
    {
        moneyText.text = "Money: " + newAmount;
    }
    void UpdateHealthCount(int newAmount)
    {
        healthText.text = "Health: " + newAmount;
    }
}
